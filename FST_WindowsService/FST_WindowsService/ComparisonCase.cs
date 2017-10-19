using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using System.Threading;
using FST.Common;
using System.Data.OleDb;

namespace FST_WindowsService
{
    /// <summary>
    /// This class is a comparison case instance, and is associated with a thread running a comparison. 
    /// </summary>
    class ComparisonCase
    {
        FST.Common.Business_Interface bi;
        ComparisonData comparisonData = null;
        string recordID = string.Empty;
        string strUserId = "";
        string strHp_Head = "";
        string strHd_Head = "";
        string strLab_Popultn_Type = "";
        string strUserName = "";
        string strReportFilePath = "";

        /// <summary>
        /// Constructor takes the DataRow returned from the database and creates a case.
        /// </summary>
        /// <param name="dr">DataRow with case data</param>
        public ComparisonCase(DataRow dr)
        {
            bi = FSTService.Instance.bi;

            if (dr == null)
                return;

            // get our record ID, the Comparison Data, and set up some variables that we use to process the report
            comparisonData = ComparisonData.Deserialize(dr["ComparisonXML"].ToString());
            recordID = dr["RecordID"].ToString();
            strUserId = comparisonData.UserName;
            strHp_Head = comparisonData.HpHead;
            strHd_Head = comparisonData.HdHead;
            strLab_Popultn_Type = comparisonData.BulkType == ComparisonData.enBulkType.FromFile ? "From File" : comparisonData.BulkType == ComparisonData.enBulkType.LabTypes ? "Lab Types" : "Population";
            strUserName = comparisonData.UserName.Substring(comparisonData.UserName.LastIndexOf('\\') + 1);

            // spawn thread
            ThreadPool.QueueUserWorkItem(new WaitCallback(ComparisonWorkerThreadMain));
        }

        /// <summary>
        /// This method is the main method of the Comparison Case child thread. Here, we determine the type of comparison we are running, run that comparsion,
        /// print the report, and send out an e-mail, close the case, and remove ourselves from the running threads list.
        /// </summary>
        /// <param name="o"></param>
        void ComparisonWorkerThreadMain(object o)
        {
            try
            {
                // set case to "processing"
                FSTService.CreateDailyLogEntry("Case Started");
                string message = bi.UpdateCaseStatus(recordID, "P");
                if (String.IsNullOrEmpty(message))
                    FSTService.CreateDailyLogEntry(message + Environment.NewLine);

                if (comparisonData.Bulk)
                {   // process bulk comparison
                    FSTService.CreateDailyLogEntry("Bulk Search Started");
                    StartBulkSearch();
                    FSTService.CreateDailyLogEntry("Bulk Search Ended and sending Email");
                }
                else
                {   // process individual comparison
                    FSTService.CreateDailyLogEntry("Individual Comparison Started");
                    StarIndividualComparison();
                    FSTService.CreateDailyLogEntry("Individual Comparison Ended and sending Email");
                }

                // set status to "processed"
                message = bi.UpdateCaseStatus(recordID, "Y");
                if (String.IsNullOrEmpty(message))
                    FSTService.CreateDailyLogEntry(message + Environment.NewLine);
                FSTService.CreateDailyLogEntry("Case Ended");

                // remove ourselves from the running cases list
                if (comparisonData.Bulk && (comparisonData.CompareMethodID == 13 || comparisonData.CompareMethodID == 17 || comparisonData.CompareMethodID == 14))
                    FSTService.removeLongRunningCase(this);
                else
                    FSTService.removeRunningCase(this);

                SendEmail();
                FSTService.CreateDailyLogEntry("Email Sent");
            }
            catch (Exception ex)
            {
                // if we experience any error, we log it and remove ourselves from the running cases list
                FSTService.CreateDailyLogEntry(ex.Message + Environment.NewLine + ex.StackTrace);
                if (comparisonData.Bulk && (comparisonData.CompareMethodID == 13 || comparisonData.CompareMethodID == 14 || comparisonData.CompareMethodID == 17))
                    FSTService.removeLongRunningCase(this);
                else
                    FSTService.removeRunningCase(this);
            }
        }

        /// <summary>
        /// We process an individual comparison
        /// </summary>
        private void StarIndividualComparison()
        {
            // special string signifying a test
            bool isTest = (comparisonData.Comparison ?? string.Empty) == "\tes\t";
            string testId = isTest ? comparisonData.FB1 : string.Empty;

            if (!isTest) Log.Info(strUserId, comparisonData.ToString(), "Comparison Start", strHp_Head + " / " + strHd_Head);

            FST.Common.Comparison comparison = new Comparison(comparisonData);

            Dictionary<string, float> result = comparison.DoCompare();

            if (!isTest)
                strReportFilePath = FSTService.Instance.IndividualPrinter.Print(comparisonData);
            else
                bi.WriteTestResults(testId, comparisonData.Item ?? string.Empty, result["Asian"].ToString(), result["Black"].ToString(), result["Caucasian"].ToString(), result["Hispanic"].ToString());

            if (!isTest) Log.Info(strUserId, comparisonData.ToString(), "Comparison End", strHp_Head + " / " + strHd_Head);
        }

        private void StartBulkSearch()
        {
            // special string signifying a test
            bool isTest = comparisonData.Comparison == "\tes\t";
            string testId = isTest ? comparisonData.FB1 : string.Empty;

            if (!isTest) Log.Info(strUserId, comparisonData.ToString(), "Comparison Start", strHp_Head + " / " + strHd_Head);

            // frequencies cache
            DataTable dtFrequencies = new Database().GetFrequencyData();
            // create denominator cache
            Dictionary<string, Dictionary<string, double>> perRaceLocusDenominatorCache = new Dictionary<string, Dictionary<string, double>>();
            foreach (DataRow drEth in new Database().getAllEthnics().Rows)
                perRaceLocusDenominatorCache.Add(drEth["EthnicName"].ToString(), new Dictionary<string, double>());
            foreach (string key in perRaceLocusDenominatorCache.Keys)
                foreach (DataRow drLoc in bi.GetLocusInOrder(comparisonData.LabKitID).Rows)
                    perRaceLocusDenominatorCache[key].Add(drLoc["LocusName"].ToString().ToUpper(), double.NaN);
            // end create denominator cache
            // dropout cache
            Dictionary<int, Dictionary<string, Dictionary<string, float>>> perReplicateDropOutCache = new Dictionary<int, Dictionary<string, Dictionary<string, float>>>();
            for (int i = 0; i < 3; i++)
                foreach (DataRow drLoc in bi.GetLocusInOrder(comparisonData.LabKitID).Rows)
                {
                    if (!perReplicateDropOutCache.ContainsKey(i))
                        perReplicateDropOutCache.Add(i, new Dictionary<string, Dictionary<string, float>>());

                    perReplicateDropOutCache[i].Add(drLoc["LocusName"].ToString().ToUpper(), new Dictionary<string, float>());
                }
            // end dropout cache
            // start frequency cache
            Dictionary<string, Dictionary<string, Dictionary<string, float>>> perRaceLocusFrequencyCache = new Dictionary<string, Dictionary<string, Dictionary<string, float>>>();
            foreach (DataRow drEth in new Database().getAllEthnics().Rows)
                perRaceLocusFrequencyCache.Add(drEth["EthnicName"].ToString(), new Dictionary<string, Dictionary<string, float>>());
            foreach (string key in perRaceLocusDenominatorCache.Keys)
                foreach (DataRow drLoc in bi.GetLocusInOrder(comparisonData.LabKitID).Rows)
                    perRaceLocusFrequencyCache[key].Add(drLoc["LocusName"].ToString().ToUpper(), new Dictionary<string, float>());
            // end frequency cache
            // start bulk permutation caches
            // all we need to do is send these in as not null
            Dictionary<string, Dictionary<int, List<Comparison.AllelesPair>>> numeratorPermutationCache = new Dictionary<string, Dictionary<int, List<Comparison.AllelesPair>>>();
            Dictionary<string, Dictionary<int, List<Comparison.AllelesPair>>> denominatorPermutationCache = new Dictionary<string, Dictionary<int, List<Comparison.AllelesPair>>>();
            // end bulk permutation cache

            Dictionary<string, Dictionary<string, float>> Results = new Dictionary<string, Dictionary<string, float>>();

            // gets "lab types," "population," or "from file" comparison list from the database
            DataTable dtKnownProfile = bi.GetKnown_Profile(strLab_Popultn_Type, comparisonData.FromFileGuid);
            for (int i = 0; i < dtKnownProfile.Rows.Count; i++)
            {
                try
                {
                    // reads datarow to a dictionary
                    comparisonData.ComparisonAlleles = ReadBulkComparison(dtKnownProfile, i);
                    FST.Common.Comparison comparison = new Comparison(comparisonData);

                    Dictionary<string, float> Result = comparison.DoCompare( // caches
                                                                            dtFrequencies,
                                                                            perRaceLocusDenominatorCache,
                                                                            perRaceLocusFrequencyCache,
                                                                            perReplicateDropOutCache,
                                                                            numeratorPermutationCache,
                                                                            denominatorPermutationCache
                                                                            );

                    // if we're testing, write test results. otherwise, add results to the Results dictionary (gets passed to the bulk printer below)
                    if (isTest)
                        bi.WriteTestResults(testId, dtKnownProfile.Rows[i]["ID"].ToString(), Result["Asian"].ToString(), Result["Black"].ToString(), Result["Caucasian"].ToString(), Result["Hispanic"].ToString());
                    else
                        Results.Add(dtKnownProfile.Rows[i]["ID"].ToString(), Result);
                }
                catch
                {
                    continue;
                }
            }

            if (!isTest)
                strReportFilePath = FSTService.Instance.BulkPrinter.Print(Results, dtKnownProfile, comparisonData);

            if (!isTest) Log.Info(strUserId, comparisonData.ToString(), "Comparison End", strHp_Head + " / " + strHd_Head);
        }

        /// <summary>
        /// Reads current DataRow to dictionary
        /// </summary>
        /// <param name="dtKnownProfile">DataTable containing our Comparison profiles from the DB</param>
        /// <param name="intRowNum">Index of the current row</param>
        /// <returns>Dictionary with comparison profile</returns>
        private Dictionary<int, Dictionary<string, string>> ReadBulkComparison(DataTable dtKnownProfile, int intRowNum)
        {
            Dictionary<int, Dictionary<string, string>> val = new Dictionary<int, Dictionary<string, string>>();
            for (int i = 0; i < dtKnownProfile.Columns.Count - 1; i++)
            {
                string locus = dtKnownProfile.Columns[i].ColumnName.Trim();
                string alleles = dtKnownProfile.Rows[intRowNum][i].ToString().Trim();
                alleles = AddCommaToGenotypeSeparatedBySpace(alleles);

                if (val.ContainsKey(1))
                    val[1].Add(locus, alleles);
                else
                {
                    Dictionary<string, string> s = new Dictionary<string, string>();
                    s.Add(locus, alleles);
                    val.Add(1, s);
                }
            }

            return val;
        }

        /// <summary>
        /// If we have alleles separated by a space, we add a common between them. This really shouldn't happen, but it does with some old profiles.
        /// </summary>
        /// <param name="alleles">CSV string with alleles</param>
        /// <returns>CSV string with comma between alleles separated by space</returns>
        private static string AddCommaToGenotypeSeparatedBySpace(string alleles)
        {
            if (!alleles.Contains(",") && alleles.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                alleles = alleles.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0] + "," +
                    alleles.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[1];
            return alleles;
        }

        /// <summary>
        /// E-mail the comparison report to the user, then delete the comparison report. This method assumes that the comparison report is already written
        /// to the file pointed to by strReportFilePath
        /// </summary>
        private void SendEmail()
        {
            try
            {
                string strSubject = "";
                string strBody = "";
                if (!comparisonData.Bulk)
                {
                    strSubject = "Forensic Comparison Report Started at " + comparisonData.ReportDate.ToString() + " is ready for viewing";
                    strBody = "The Forensic Comparison Report of Hp: " + comparisonData.HpHead +
                         " and Hd: " + comparisonData.HdHead + " is ready for viewing";
                }
                else
                {
                    strSubject = "Bulk Comparison Started at " + comparisonData.ReportDate.ToString() + " is ready for viewing";
                    strBody = "The Bulk Comparison of Hp: " + comparisonData.HpHead +
                         " and Hd: " + comparisonData.HdHead + " using " +
                         comparisonData.BulkType.ToString() + " database is ready for viewing";
                }
                DataTable dtEmailId = bi.GetEmailId(comparisonData.UserName.ToString());
                using (MailMessage msg = new MailMessage())
                {
                    msg.From = new MailAddress(ConfigurationManager.AppSettings["FST_SERVICE_EMAIL_ADDRESS_FROM"], ConfigurationManager.AppSettings["FST_SERVICE_EMAIL_ADDRESS_NAME"]);
                    msg.To.Add(new MailAddress(dtEmailId.Rows[0]["Email"].ToString().Trim(), comparisonData.UserName.Trim()));

                    Attachment atchmt;

                    atchmt = new Attachment(strReportFilePath);

                    msg.Attachments.Add(atchmt);

                    msg.Subject = strSubject;
                    msg.Body = strBody;
                    //SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["FST_SERVICE_EMAIL_SERVER"]);
                    SmtpClient client = new SmtpClient((ConfigurationManager.AppSettings["FST_SERVICE_EMAIL_SERVER"]), Convert.ToInt32(ConfigurationManager.AppSettings["FST_SERVICE_EMAIL_SERVER_PORT"]));
                    using (client)
                    {
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        string username = ConfigurationManager.AppSettings["FST_SERVICE_EMAIL_SERVER_USERNAME"];
                        string password = ConfigurationManager.AppSettings["FST_SERVICE_EMAIL_SERVER_PASSWORD"];
                        client.Credentials = new NetworkCredential(username, password);
                        //client.Credentials = new NetworkCredential("username","password");
                        client.Send(msg);
                    }
                }
                dtEmailId.Clear();
                dtEmailId.Dispose();

                File.Delete(strReportFilePath);
            }
            catch (Exception ex)
            {
                FSTService.CreateDailyLogEntry(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}
