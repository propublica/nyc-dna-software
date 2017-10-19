using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FST.Common;
using System.Data;
using System.Threading;
using System.Configuration;
using System.IO;

namespace FST.Web
{
    public partial class frmManualEntry : System.Web.UI.Page
    {
        ComparisonData comparisonData;
        Business_Interface bi = new Business_Interface();

        protected void Page_Load(object sender, EventArgs e)
        {
            comparisonData = (ComparisonData)Session["ComparisonData"];
            if (comparisonData == null)
                Response.Redirect("~/frmDefault.aspx");

            if (IsPostBack)
            {
                // get our comparison configuration data from the page
                // we replace double quotes with single quotes because we don't htmlencode our fields
                // read comments in Support/HtmlAttributeEncodingNot.cs for info on fixing this
                comparisonData.FB1 = this.txFBNo.Text = this.txFBNo.Text.Replace("\"", "''");
                comparisonData.Comparison = this.txSuspectNo.Text = this.txSuspectNo.Text.Replace("\"", "''");
                comparisonData.FB2 = this.txFB2No.Text = this.txFB2No.Text.Replace("\"", "''");
                comparisonData.Item = this.txItemNo.Text = this.txItemNo.Text.Replace("\"", "''");

                comparisonData.DNAAmount = decimal.Parse(txDropout.Text == string.Empty ? "0" : txDropout.Text);
                comparisonData.Deducible = dlDeducible.Text == "Yes";
                comparisonData.BulkType = dlTypes.Text == "From File" ? ComparisonData.enBulkType.FromFile : dlTypes.Text == "Lab Types" ? ComparisonData.enBulkType.LabTypes : ComparisonData.enBulkType.Population;
            }
            else
            {
                this.Title = "FST - " + comparisonData.CompareMethodLongName;
                lblMenuLevel1.Text = comparisonData.CompareMethodLongName;
                lblMenuLevel2.Text = comparisonData.CompareMethodLongName + " (Edit)";

                // we only have the type selection for bulk searches
                lblTypes.Visible = comparisonData.Bulk;
                dlTypes.Visible = comparisonData.Bulk;

                // our case data is only for invididual searches
                tblCaseData.Visible = !comparisonData.Bulk;

                // won't change but i wanted to keep the generation of the headers consistent
                lblEvidence.Text = GetAccordionHeader("Evidence:");

                eviAlleles.SetEvidenceDictionary(comparisonData.EvidenceAlleles.Count > 0 ? comparisonData.EvidenceAlleles : null);

                // we always have a primary comparison, unless this is a bulk search
                lblComparison1.Visible = !comparisonData.Bulk;
                proComparison1Alleles.Visible = lblComparison1.Visible;
                lblComparison1.Text = comparisonData.Comparison1Name != string.Empty ? GetAccordionHeader(string.Format("{0} (Comparison):", comparisonData.Comparison1Name)) : GetAccordionHeader(string.Format("Comparison{0}:", comparisonData.NumeratorProfiles.ComparisonCount > 1 ? " 1" : string.Empty));
                proComparison1Alleles.SetProfileDictionary(!comparisonData.Bulk && comparisonData.ComparisonAlleles.ContainsKey(1) ? comparisonData.ComparisonAlleles[1] : null);

                // we check to see which profiles we are using for this comaprison from the comparison scenario configuration data
                // the comparison profiles are only found in the numerator (so far), so we only check the count in the numerator
                // hopefully we can do this stuff below in a loop at some point so we can auto-generate comparison
                // scenarios from the configuration information alone
                lblComparison2.Visible = comparisonData.NumeratorProfiles.ComparisonCount >= 2;
                proComparison2Alleles.Visible = lblComparison2.Visible;
                lblComparison2.Text = comparisonData.Comparison2Name != string.Empty ? GetAccordionHeader(string.Format("{0} (Comparison):", comparisonData.Comparison2Name)) : GetAccordionHeader(string.Format("Comparison 2:"));
                proComparison2Alleles.SetProfileDictionary(comparisonData.NumeratorProfiles.ComparisonCount >= 2 && comparisonData.ComparisonAlleles.ContainsKey(2) ? comparisonData.ComparisonAlleles[2] : null);

                lblComparison3.Visible = comparisonData.NumeratorProfiles.ComparisonCount >= 3;
                proComparison3Alleles.Visible = lblComparison3.Visible;
                lblComparison3.Text = comparisonData.Comparison3Name != string.Empty ? GetAccordionHeader(string.Format("{0} (Comparison):", comparisonData.Comparison3Name)) : GetAccordionHeader(string.Format("Comparison 3:"));
                proComparison3Alleles.SetProfileDictionary(comparisonData.NumeratorProfiles.ComparisonCount >= 3 && comparisonData.ComparisonAlleles.ContainsKey(3) ? comparisonData.ComparisonAlleles[3] : null);

                lblComparison4.Visible = comparisonData.NumeratorProfiles.ComparisonCount >= 4;
                proComparison4Alleles.Visible = lblComparison4.Visible;
                lblComparison4.Text = comparisonData.Comparison4Name != string.Empty ? GetAccordionHeader(string.Format("{0} (Comparison):", comparisonData.Comparison4Name)) : GetAccordionHeader(string.Format("Comparison 4:"));
                proComparison4Alleles.SetProfileDictionary(comparisonData.NumeratorProfiles.ComparisonCount >= 4 && comparisonData.ComparisonAlleles.ContainsKey(4) ? comparisonData.ComparisonAlleles[4] : null);

                // for the known profiles we check the numerator and the denominator because there may be known profiles in the denominator
                // that do not appear in the numerator
                bool showKnown1 = comparisonData.NumeratorProfiles.KnownCount >= 1 || comparisonData.DenominatorProfiles.KnownCount >= 1;
                lblKnown1.Visible = showKnown1;
                proKnown1Alleles.Visible = lblKnown1.Visible;
                lblKnown1.Text = comparisonData.Known1Name != string.Empty ? GetAccordionHeader(string.Format("{0} (Known):", comparisonData.Known1Name)) : GetAccordionHeader(string.Format("Known{0}:", showKnown1 ? " 1" : string.Empty));
                proKnown1Alleles.SetProfileDictionary(showKnown1 && comparisonData.KnownsAlleles.ContainsKey(1) ? comparisonData.KnownsAlleles[1] : null);

                bool showKnown2 = comparisonData.NumeratorProfiles.KnownCount >= 2 || comparisonData.DenominatorProfiles.KnownCount >= 2;
                lblKnown2.Visible = showKnown2;
                proKnown2Alleles.Visible = lblKnown2.Visible;
                lblKnown2.Text = comparisonData.Known2Name != string.Empty ? GetAccordionHeader(string.Format("{0} (Known):", comparisonData.Known2Name)) : GetAccordionHeader(string.Format("Known 2:"));
                proKnown2Alleles.SetProfileDictionary(showKnown2 && comparisonData.KnownsAlleles.ContainsKey(2) ? comparisonData.KnownsAlleles[2] : null);

                bool showKnown3 = comparisonData.NumeratorProfiles.KnownCount >= 3 || comparisonData.DenominatorProfiles.KnownCount >= 3;
                lblKnown3.Visible = showKnown3;
                proKnown3Alleles.Visible = lblKnown3.Visible;
                lblKnown3.Text = comparisonData.Known3Name != string.Empty ? GetAccordionHeader(string.Format("{0} (Known):", comparisonData.Known3Name)) : GetAccordionHeader(string.Format("Known 3:"));
                proKnown3Alleles.SetProfileDictionary(showKnown3 && comparisonData.KnownsAlleles.ContainsKey(3) ? comparisonData.KnownsAlleles[3] : null);

                bool showKnown4 = comparisonData.NumeratorProfiles.KnownCount >= 4 || comparisonData.DenominatorProfiles.KnownCount >= 4;
                lblKnown4.Visible = showKnown4;
                proKnown4Alleles.Visible = lblKnown4.Visible;
                lblKnown4.Text = comparisonData.Known4Name != string.Empty ? GetAccordionHeader(string.Format("{0} (Known):", comparisonData.Known4Name)) : GetAccordionHeader(string.Format("Known 4:"));
                proKnown4Alleles.SetProfileDictionary(showKnown4 && comparisonData.KnownsAlleles.ContainsKey(4) ? comparisonData.KnownsAlleles[4] : null);

                // if we're coming from the file upload page, we want to set these
                // we replace double quotes with single quotes because we don't htmlencode our fields
                // read comments in Support/HtmlAttributeEncodingNot.cs for info on fixing this
                this.txFBNo.Text = (comparisonData.FB1 ?? string.Empty).Replace("\"", "''");
                this.txSuspectNo.Text = (comparisonData.Comparison ?? string.Empty).Replace("\"", "''");
                this.txFB2No.Text = (comparisonData.FB2 ?? string.Empty).Replace("\"", "''");
                this.txItemNo.Text = (comparisonData.Item ?? string.Empty).Replace("\"", "''");

                txDropout.Text = comparisonData.DNAAmount.ToString();
                dlTypes.SelectedValue = comparisonData.BulkType == ComparisonData.enBulkType.FromFile ? "From File" : comparisonData.BulkType == ComparisonData.enBulkType.LabTypes ? "Lab Types" : "Population";

                // this is disabled for comparisons where both the numerator and the denominator have only one contributor
                if (1 == comparisonData.NumeratorProfiles.ComparisonCount + comparisonData.NumeratorProfiles.KnownCount + comparisonData.NumeratorProfiles.UnknownCount
                    && 1 == comparisonData.DenominatorProfiles.ComparisonCount + comparisonData.DenominatorProfiles.KnownCount + comparisonData.DenominatorProfiles.UnknownCount)
                {
                    this.dlDeducible.Text = Convert.ToString("Yes");
                    this.dlDeducible.Enabled = false;
                }
                else
                {
                    dlDeducible.SelectedValue = comparisonData.Deducible ? "Yes" : "No";
                    this.dlDeducible.Enabled = true;
                }
            }
        }

        /// <summary>
        /// This method wraps a string in a pre-defined format for our "accordion" control. This control requires a link inside an H3 tag set
        /// </summary>
        /// <param name="arg">The string which will be the name shown for this profile</param>
        /// <returns>An HTML string which is formatted according to the rules used by the JS accordion control</returns>
        private string GetAccordionHeader(string arg)
        {
            return string.Format(@"<h3><a href=""#""><span>{0}</span></a></h3>", arg);
        }

        /// <summary>
        /// This method is called from either the UI or the frmTestCenter page, and uses the ComparisonData class and potentially the TestCenter class 
        /// to run a comparison. If this is an individual comparison that is being run on this page, this method runs the comparison here. Othewrise,
        /// the ProcessInBackgroundService() method is called to handle the case in the Windows Service component.
        /// </summary>
        /// <param name="sender">The button that was clicked, or the frmTestCenter class.</param>
        /// <param name="e"></param>
        public void btnCompare_Click(object sender, EventArgs e)
        {
            // this is used to get information about whether this is being called from this page or from the tesing center, and the associated data if it's from the testing center
            bool isTest = sender is FST.Web.Admin.frmTestCenter;
            FST.Web.Admin.frmTestCenter testPage = isTest ? sender as FST.Web.Admin.frmTestCenter : null;
            if (isTest) comparisonData = testPage.comparisonData;

            comparisonData.ReportDate = DateTime.Now;

            // validate form
            if (!isTest && !FieldChecks()) return;

            // if we're not running a test (and we don't already have data), we get the data from the controls on the page
            if (!isTest)
            {
                // get the evidence from the eviAlleles Evidence control via the GetEvidenceDictionary() method. check it for details
                comparisonData.EvidenceAlleles = eviAlleles.GetEvidenceDictionary();

                // we only get comaprison data if we're not doing a bulk, otherwise the comparison data comes from an alternate source.
                // this source is the Knowns table in DB where files using the 
                if (!comparisonData.Bulk)
                {
                    comparisonData.ComparisonAlleles.Clear();

                    // we check for comparison profiles in the denominator too. it was easier to c/p the code, and it's ready in case comparisons ever make it into the denominator
                    // the data comes from the proComparison1Alleles (or whichever) Profile control's GetProfileDictionary() method. check it for details
                    // we do the same for the known profiles below
                    if (comparisonData.NumeratorProfiles.ComparisonCount >= 1 || comparisonData.DenominatorProfiles.ComparisonCount >= 1)
                        comparisonData.ComparisonAlleles.Add(1, proComparison1Alleles.GetProfileDictionary());

                    if (comparisonData.NumeratorProfiles.ComparisonCount >= 2 || comparisonData.DenominatorProfiles.ComparisonCount >= 2)
                        comparisonData.ComparisonAlleles.Add(2, proComparison2Alleles.GetProfileDictionary());

                    if (comparisonData.NumeratorProfiles.ComparisonCount >= 3 || comparisonData.DenominatorProfiles.ComparisonCount >= 3)
                        comparisonData.ComparisonAlleles.Add(3, proComparison3Alleles.GetProfileDictionary());

                    if (comparisonData.NumeratorProfiles.ComparisonCount >= 4 || comparisonData.DenominatorProfiles.ComparisonCount >= 4)
                        comparisonData.ComparisonAlleles.Add(4, proComparison4Alleles.GetProfileDictionary());
                }

                comparisonData.KnownsAlleles.Clear();

                if (comparisonData.NumeratorProfiles.KnownCount >= 1 || comparisonData.DenominatorProfiles.KnownCount >= 1)
                    comparisonData.KnownsAlleles.Add(1, proKnown1Alleles.GetProfileDictionary());

                if (comparisonData.NumeratorProfiles.KnownCount >= 2 || comparisonData.DenominatorProfiles.KnownCount >= 2)
                    comparisonData.KnownsAlleles.Add(2, proKnown2Alleles.GetProfileDictionary());

                if (comparisonData.NumeratorProfiles.KnownCount >= 3 || comparisonData.DenominatorProfiles.KnownCount >= 3)
                    comparisonData.KnownsAlleles.Add(3, proKnown3Alleles.GetProfileDictionary());

                if (comparisonData.NumeratorProfiles.KnownCount >= 4 || comparisonData.DenominatorProfiles.KnownCount >= 4)
                    comparisonData.KnownsAlleles.Add(4, proKnown4Alleles.GetProfileDictionary());
            }

            // go to the background service if we're doing a bulk or an individual search that runs long
            if (comparisonData.Bulk || comparisonData.RunIndividualOnService)
            {
                ProcessInBackgroundService(testPage);
            }
            else // otherwise we process here
            {
                if (!isTest) Log.Info(Context.User.Identity.Name, Request.FilePath, Session, "Comparison Start", comparisonData.HpHead + "/" + comparisonData.HdHead);

                // instantiate the comparison class with our comparison data
                FST.Common.Comparison comparison = new Comparison(comparisonData);

                // if we're using DEBUGOUT functionality, set up the debugData dictionary and the username. the debugOut functionality runs if the debugData dictionary != null
                if ("true" == ConfigurationManager.AppSettings.Get("DEBUGOUT"))
                {
                    comparison.debugData = new Dictionary<string, Comparison.DebugData>();
                    comparison.UserName = User.Identity.Name;
                }

                Dictionary<string, float> result = comparison.DoCompare();

                if (!isTest) Log.Info(Context.User.Identity.Name, Request.FilePath, Session, "Comparison End", comparisonData.HpHead + "/" + comparisonData.HdHead);

                // if we're not testing we print a PDF report, otherwise we write the test results to the DB
                if (!isTest)
                    Print();
                else
                    bi.WriteTestResults(
                        testPage.subTestID.ToString(), 
                        testPage.comparisonID.ToString(), 
                        result["Asian"].ToString(), 
                        result["Black"].ToString(), 
                        result["Caucasian"].ToString(), 
                        result["Hispanic"].ToString()
                    );
            }
        }

        /// <summary>
        /// This method writes the test results to a PDF using the IndividualReportPrinter class, and then writes the file as the response resulting in the client downloading the file.
        /// </summary>
        private void Print()
        {
            Response.Clear();
            Response.ContentType = "application/pdf";
            // this cookie is set to tell the spinner to disappear. on the page we generate a GUID in JS and set the txSpinningIndicatorGuid control to that alue
            // then we wait in a loop for the ComparisonDone cookie to come back with that value to make the indicator disappear.
            Response.AppendCookie(new HttpCookie("ComparisonDone", txSpinningIndicatorGuid.Text));
            Response.AddHeader("Content-Disposition", "attachment; filename=FSTReport.pdf");

            // this class prints a PDF to a file for us using ReportViewer and the ComparisonData.Print() method
            FST.Common.IndividualReportPrinter print = new IndividualReportPrinter(Server.MapPath("~/Admin/Upload/"), Server.MapPath("~/Reports/FSTResultReport.rdlc"));
            string filepath = print.Print(comparisonData);

            // we write the file to the Response stream
            Response.BinaryWrite(File.ReadAllBytes(filepath));
            Response.End();
            // delete the file and log if there's an error
            try { File.Delete(filepath); }
            catch { Log.Error(Context.User.Identity.Name, Request.FilePath, Session, "Temporary Report File Deletion Error", filepath, null); }
        }

        /// <summary>
        /// This method is called if we're running a report in the Windows Serivce. It checks if a comparisons file was uploaded for a bulk report
        /// using the "From File" method and processes that if we have one. Then we save the serialized comparison data along with the file to
        /// the database, and we call the web service which the windows service extends to notify it of the new job to be processed.
        /// </summary>
        /// <param name="testPage">If this is a test, we pass it in so we can get our comparison data.</param>
        private void ProcessInBackgroundService(FST.Web.Admin.frmTestCenter testPage)
        {
            bool isTest = testPage != null;

            DataTable fromFileTable = null;
            Guid jobGuid = Guid.Empty;

            // if we're doing a bulk comparison or we're doing a test
            if (comparisonData.BulkType == ComparisonData.enBulkType.FromFile || isTest)
            {
                // this is the GUID we send to the database that identifies this set of profiles as associated with this comparison
                Guid fromFileID = Guid.NewGuid();
                // if this isn't the test, process the uploaded file. read the Business_Interfae.GetProfilesFromFile() method for details on file format and other peculiarities
                fromFileTable = isTest ? testPage.comparisonProfiles : bi.GetProfilesFromFile(fuPopulationUpload, Server.MapPath("~/Admin/Upload/"), comparisonData.LabKitID);
                comparisonData.FromFileName = isTest ? string.Empty : fuPopulationUpload.FileName;
                if (fromFileTable == null) return; // the error message comes from Business_Interface.GetProfilesFromFile((, don't worry
                fromFileTable.Columns.Add(new DataColumn("GUID", typeof(Guid)));
                // set our GUID for each of the rows
                foreach (DataRow dr in fromFileTable.Rows)
                    dr["GUID"] = fromFileID;
                // set our GUID so we know how to get the data back when we get to the Windows Service
                comparisonData.FromFileGuid = fromFileID;
            }

            // serialize the ComparisonData class
            string xml = comparisonData.Serialize();
            // write the "Case" which is the ComparisonData class, a set of associated parameters, and the associated 'from file' data, if any.
            bi.SaveCase(
                xml,
                Session["FST_VERSION"].ToString(),
                comparisonData.DNAAmount.ToString(),
                comparisonData.FB1,
                comparisonData.Comparison,
                comparisonData.FB2,
                comparisonData.Item,
                comparisonData.UserName,
                comparisonData.Theta.ToString(),
                comparisonData.CompareMethodID,
                comparisonData.Deducible ? "Yes" : "No",
                comparisonData.Degradation == ComparisonData.enDegradation.None ? "ND" : comparisonData.Degradation == ComparisonData.enDegradation.Mild ? "MD" : "SD",
                string.Empty,
                comparisonData.HdHead,
                comparisonData.HpHead,
                comparisonData.Bulk ? "B" : "N",
                comparisonData.BulkType == ComparisonData.enBulkType.FromFile ? "From File" : comparisonData.BulkType == ComparisonData.enBulkType.LabTypes ? "Lab Types" : "Population",
                "N",
                comparisonData.LabKitID.ToString(),
                comparisonData.FromFileGuid,
                fromFileTable,
                out jobGuid);

            // call the webservice in a separate thread to notify it that we're trying to run a comparison
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object o)
            {
                try
                {
                    FSTWebService.FSTWebServiceClient webSvcClient = new FSTWebService.FSTWebServiceClient();
                    webSvcClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(ConfigurationManager.AppSettings["FST_SERVICE_ADDRESS"]);
                    webSvcClient.RunJob(jobGuid.ToString());
                }
                catch (Exception ex)
                {
                    Log.Error(User.Identity.Name.ToString(), (string)o, isTest ? null : Session, "Web Service Exception", string.Empty, ex);
                }
            }), isTest ? null : (object)Request.FilePath);

            // notify the user that the comparison will run in the background
            if(!isTest)
                MessageBox.Show("Your comparison will run in the background, and you will receive an e-mail with the results.");
        }

        /// <summary>
        /// This method resets all the fields on the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnReset_Click(object sender, EventArgs e)
        {
            eviAlleles.Reset();
            proComparison1Alleles.Reset();
            proComparison2Alleles.Reset();
            proComparison3Alleles.Reset();
            proComparison4Alleles.Reset();
            proKnown1Alleles.Reset();
            proKnown2Alleles.Reset();
            proKnown3Alleles.Reset();
            proKnown4Alleles.Reset();
            dlDeducible.SelectedIndex = 0;
            dlTypes.SelectedIndex = 0;
            txDropout.Text = string.Empty;
        }

        /// <summary>
        /// Sends the user back to the Upload page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmUpload.aspx");
        }

        /// <summary>
        /// This method handles the SelectedIndex changing on the dlTypes drop-down. If the selected index falls on the "From File" option, then
        /// we show the associated file upload control. Otherwise, we hide it. If you're wondering why we don't have this on the FileUpload page,
        /// it is because we do it in JS. The reason is that postbacks clear our file uploads which would make the page cumbersome to use.
        /// </summary>
        /// <param name="sender">The dlTypes control.</param>
        /// <param name="e">The EventArgs</param>
        protected void dlTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlPopulationUpload.Visible = (dlTypes.Text == "From File");
        }

        /// <summary>
        /// This method validates the form fields (which, right now, means the DNA template amount field)
        /// </summary>
        /// <returns>Whether the form is valid.</returns>
        public bool FieldChecks()
        {
            // can't leave it empty
            if (String.IsNullOrEmpty(this.txDropout.Text))
            {
                MessageBox.Show("DNA Template Amount cannot be left blank");
                return false;
            }

            // it must be a number, at most having units (pg, picograms) 
            try
            {
                string dropoutOption = this.txDropout.Text.Replace("pg", "").Trim();
                decimal tempDropout = Convert.ToDecimal(dropoutOption);
            }
            catch (FormatException e)
            {
                MessageBox.Show("Invalid DNA Template Amount" + e.Message);
                return false;
            }

            // it cannot be less than 6.25 picograms
            try
            {
                string dropoutOption = this.txDropout.Text.Replace("pg", "").Trim();
                decimal tempDropout = Convert.ToDecimal(dropoutOption);
                if (tempDropout < 6.25m)
                {
                    MessageBox.Show("DNA Template Amount cannot be less than 6.25");
                    return false;
                }
            }
            catch (FormatException e)
            {
                MessageBox.Show("Invalid DNA Template Amount" + e.Message);
                return false;
            }

            return true;
        }
    }
}