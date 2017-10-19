using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FST.Common;
using System.Text;
using System.Data;
using System.Threading;
using System.IO;

namespace FST.Web
{
    public partial class frmUpload : System.Web.UI.Page
    {
        ComparisonData comparisonData;
        FST.Common.Business_Interface bi = new Business_Interface();

        protected void Page_Load(object sender, EventArgs e)
        {
            comparisonData = (ComparisonData)Session["ComparisonData"];
            if (comparisonData == null)
                Response.Redirect("~/frmDefault.aspx");

            if (IsPostBack)
            {
                // get our comparison configuration data from the page
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

                // we only have the type selection for bulk searches
                lblTypes.Visible = comparisonData.Bulk;
                dlTypes.Visible = comparisonData.Bulk;
                pnlPopulationUpload.Visible = comparisonData.Bulk;
                // and our case data is only for invididual searches
                tblCaseData.Visible = !comparisonData.Bulk;

                // show only the relevant upload rows
                (tblFileUploads.FindControl("trComparison1") as TableRow).Visible = !comparisonData.Bulk;
                (tblFileUploads.FindControl("trComparison2") as TableRow).Visible = comparisonData.NumeratorProfiles.ComparisonCount >= 2;
                (tblFileUploads.FindControl("trComparison3") as TableRow).Visible = comparisonData.NumeratorProfiles.ComparisonCount >= 3;
                (tblFileUploads.FindControl("trComparison4") as TableRow).Visible = comparisonData.NumeratorProfiles.ComparisonCount >= 4;
                (tblFileUploads.FindControl("trKnown1") as TableRow).Visible = comparisonData.NumeratorProfiles.KnownCount >= 1 || comparisonData.DenominatorProfiles.KnownCount >= 1;
                (tblFileUploads.FindControl("trKnown2") as TableRow).Visible = comparisonData.NumeratorProfiles.KnownCount >= 2 || comparisonData.DenominatorProfiles.KnownCount >= 2;
                (tblFileUploads.FindControl("trKnown3") as TableRow).Visible = comparisonData.NumeratorProfiles.KnownCount >= 3 || comparisonData.DenominatorProfiles.KnownCount >= 3;
                (tblFileUploads.FindControl("trKnown4") as TableRow).Visible = comparisonData.NumeratorProfiles.KnownCount >= 4 || comparisonData.DenominatorProfiles.KnownCount >= 4;

                // if we're coming back here from the manual entry page then set these
                this.txFBNo.Text = comparisonData.FB1;
                this.txSuspectNo.Text = comparisonData.Comparison;
                this.txFB2No.Text = comparisonData.FB2;
                this.txItemNo.Text = comparisonData.Item;

                // this is disabled for comparisons where both the numerator and the denominator have only one contributor
                if (1 == comparisonData.NumeratorProfiles.ComparisonCount + comparisonData.NumeratorProfiles.KnownCount + comparisonData.NumeratorProfiles.UnknownCount
                    && 1 == comparisonData.DenominatorProfiles.ComparisonCount + comparisonData.DenominatorProfiles.KnownCount + comparisonData.DenominatorProfiles.UnknownCount)
                {
                    this.dlDeducible.Text = Convert.ToString("Yes");
                    this.dlDeducible.Enabled = false;
                }
                else
                {
                    this.dlDeducible.Text = Convert.ToString("No");
                    this.dlDeducible.Enabled = true;
                }
            }
        }

        /// <summary>
        /// If the user clicks edit, send them to the manual entry page so they an edit the data from the files they uploaded, or manually enter data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/frmManualEntry.aspx");
        }

        protected void btnLoadData_Click(object sender, EventArgs e)
        {
            FST.Common.Database db = new FST.Common.Database();
            DataTable locusSortOrder = db.getLocusSortOrder(Guid.Parse(Session["LabKitID"].ToString()));
            // clear any data we had before
            comparisonData.ComparisonAlleles.Clear();
            comparisonData.KnownsAlleles.Clear();

            // we don't read in the comparison profile for a bulk because it comes from either the 'from file' upload, or from the lab types/population database
            if (!comparisonData.Bulk && comparisonData.NumeratorProfiles.ComparisonCount >= 1)
            {
                // read our posted file to a dictionary and add it to our comparisonData class
                Dictionary<string, string> val = readKnownFileToDictionary(txtFileSuspectInput.PostedFile, locusSortOrder);
                if (val == null) return;
                else comparisonData.ComparisonAlleles.Add(1, val);
                // convert it to a table and set it as the source for the associated gridview
                gvSuspect.DataSource = convertKnownFromDictionaryToTable(comparisonData.ComparisonAlleles, locusSortOrder, 1);
                gvSuspect.DataBind();
                // set our client-side filename for printing in the report later
                comparisonData.Comparison1FileName = txtFileSuspectInput.PostedFile.FileName;
                // set up our name and show the grid view
                if (comparisonData.Comparison1Name.Trim() == string.Empty)
                    this.lblSuspect.Text = Convert.ToString("Profile of (Comparison 1)");
                else
                    this.lblSuspect.Text = Convert.ToString("Profile of ") + comparisonData.Comparison1Name;
                pnlSuspect.Visible = true;
            }
            if (comparisonData.NumeratorProfiles.ComparisonCount >= 2)
            {
                Dictionary<string, string> val = readKnownFileToDictionary(txtFileSuspect2Input.PostedFile, locusSortOrder);
                if (val == null) return;
                else comparisonData.ComparisonAlleles.Add(2, val);
                gvSuspect2.DataSource = convertKnownFromDictionaryToTable(comparisonData.ComparisonAlleles, locusSortOrder, 2);
                gvSuspect2.DataBind();
                comparisonData.Comparison2FileName = txtFileSuspect2Input.PostedFile.FileName;
                if (comparisonData.Comparison2Name.Trim() == string.Empty)
                    this.lblSuspect2.Text = Convert.ToString("Profile of (Comparison 2)");
                else
                    this.lblSuspect2.Text = Convert.ToString("Profile of ") + comparisonData.Comparison2Name;
                pnlSuspect2.Visible = true;
            }
            if (comparisonData.NumeratorProfiles.ComparisonCount >= 3)
            {
                Dictionary<string, string> val = readKnownFileToDictionary(txtFileSuspect3Input.PostedFile, locusSortOrder);
                if (val == null) return;
                else comparisonData.ComparisonAlleles.Add(3, val);
                gvSuspect3.DataSource = convertKnownFromDictionaryToTable(comparisonData.ComparisonAlleles, locusSortOrder, 3);
                gvSuspect3.DataBind();
                comparisonData.Comparison3FileName = txtFileSuspect3Input.PostedFile.FileName;
                if (comparisonData.Comparison3Name.Trim() == string.Empty)
                    this.lblSuspect3.Text = Convert.ToString("Profile of (Comparison 3)");
                else
                    this.lblSuspect3.Text = Convert.ToString("Profile of ") + comparisonData.Comparison3Name;
                pnlSuspect3.Visible = true;
            }
            if (comparisonData.NumeratorProfiles.ComparisonCount >= 4)
            {
                Dictionary<string, string> val = readKnownFileToDictionary(txtFileSuspect4Input.PostedFile, locusSortOrder);
                if (val == null) return;
                else comparisonData.ComparisonAlleles.Add(4, val);
                gvSuspect4.DataSource = convertKnownFromDictionaryToTable(comparisonData.ComparisonAlleles, locusSortOrder, 4);
                gvSuspect4.DataBind();
                comparisonData.Comparison4FileName = txtFileSuspect4Input.PostedFile.FileName;
                if (comparisonData.Comparison4Name.Trim() == string.Empty)
                    this.lblSuspect4.Text = Convert.ToString("Profile of (Comparison 4)");
                else
                    this.lblSuspect4.Text = Convert.ToString("Profile of ") + comparisonData.Comparison4Name;
                pnlSuspect4.Visible = true;
            }
            if (comparisonData.NumeratorProfiles.KnownCount >= 1 || comparisonData.DenominatorProfiles.KnownCount >= 1)
            {
                Dictionary<string, string> val = readKnownFileToDictionary(txtFileSuspect5Input.PostedFile, locusSortOrder);
                if (val == null) return;
                else comparisonData.KnownsAlleles.Add(1, val);
                gvSuspect5.DataSource = convertKnownFromDictionaryToTable(comparisonData.KnownsAlleles, locusSortOrder, 1);
                gvSuspect5.DataBind();
                comparisonData.Known1FileName = txtFileSuspect5Input.PostedFile.FileName;
                if (comparisonData.Known1Name.Trim() == string.Empty)
                    this.lblSuspect5.Text = Convert.ToString("Profile of (Known 1)");
                else
                    this.lblSuspect5.Text = Convert.ToString("Profile of ") + comparisonData.Known1Name;
                pnlSuspect5.Visible = true;
            }
            if (comparisonData.NumeratorProfiles.KnownCount >= 2 || comparisonData.DenominatorProfiles.KnownCount >= 2)
            {
                Dictionary<string, string> val = readKnownFileToDictionary(txtFileSuspect6Input.PostedFile, locusSortOrder);
                if (val == null) return;
                else comparisonData.KnownsAlleles.Add(2, val);
                gvSuspect6.DataSource = convertKnownFromDictionaryToTable(comparisonData.KnownsAlleles, locusSortOrder, 2);
                gvSuspect6.DataBind();
                comparisonData.Known2FileName = txtFileSuspect6Input.PostedFile.FileName;
                if (comparisonData.Known2Name.Trim() == string.Empty)
                    this.lblSuspect6.Text = Convert.ToString("Profile of (Known 2)");
                else
                    this.lblSuspect6.Text = Convert.ToString("Profile of ") + comparisonData.Known2Name;
                pnlSuspect6.Visible = true;
            }
            if (comparisonData.NumeratorProfiles.KnownCount >= 3 || comparisonData.DenominatorProfiles.KnownCount >= 3)
            {
                Dictionary<string, string> val = readKnownFileToDictionary(txtFileSuspect7Input.PostedFile, locusSortOrder);
                if (val == null) return;
                else comparisonData.KnownsAlleles.Add(3, val);
                gvSuspect7.DataSource = convertKnownFromDictionaryToTable(comparisonData.KnownsAlleles, locusSortOrder, 3);
                gvSuspect7.DataBind();
                comparisonData.Known3FileName = txtFileSuspect7Input.PostedFile.FileName;
                if (comparisonData.Known3Name.Trim() == string.Empty)
                    this.lblSuspect7.Text = Convert.ToString("Profile of (Known 3)");
                else
                    this.lblSuspect7.Text = Convert.ToString("Profile of ") + comparisonData.Known3Name;
                pnlSuspect7.Visible = true;
            }
            if (comparisonData.NumeratorProfiles.KnownCount >= 4 || comparisonData.DenominatorProfiles.KnownCount >= 4)
            {
                Dictionary<string, string> val = readKnownFileToDictionary(txtFileSuspect8Input.PostedFile, locusSortOrder);
                if (val == null) return;
                else comparisonData.KnownsAlleles.Add(4, val);
                gvSuspect8.DataSource = convertKnownFromDictionaryToTable(comparisonData.KnownsAlleles, locusSortOrder, 4);
                gvSuspect8.DataBind();
                comparisonData.Known4FileName = txtFileSuspect8Input.PostedFile.FileName;
                if (comparisonData.Known4Name.Trim() == string.Empty)
                    this.lblSuspect8.Text = Convert.ToString("Profile of (Known 4)");
                else
                    this.lblSuspect8.Text = Convert.ToString("Profile of ") + comparisonData.Known4Name;
                pnlSuspect8.Visible = true;
            }

            // read the evidence into a dictionary and add it to the comparisonData class
            Dictionary<string, Dictionary<int, string>> lan = readEvidenceFileToDictionary(txtFileUnknownInput.PostedFile, locusSortOrder);
            if (lan == null) return;
            else comparisonData.EvidenceAlleles = lan;
            // set our client-side filename for printing in the report later
            comparisonData.EvidenceFileName = txtFileUnknownInput.PostedFile.FileName;
            // convert it to a table and set it as the source for the associated gridview
            gvUnknown.DataSource = ConvertEvidenceFromDictionaryToDataTable(lan, locusSortOrder);
            gvUnknown.DataBind();
            // set our label text (done this way for consistency with code above) and show the gridview
            this.lblUnknown.Text = Convert.ToString("Evidence ");
            pnlUnknown.Visible = true;

            // this checks to see that the data that was successfully loaded from the uploaded files matches the profiles in the comparisonData class
            // if it does not, we do not show the "Compare" button because it would cause the Comparison class to generate a false comparison
            if (
                (gvSuspect.Rows.Count > 0 || comparisonData.Bulk)
                &&
                (gvSuspect2.Rows.Count > 0 || comparisonData.NumeratorProfiles.ComparisonCount < 2)
                &&
                (gvSuspect3.Rows.Count > 0 || comparisonData.NumeratorProfiles.ComparisonCount < 3)
                &&
                (gvSuspect4.Rows.Count > 0 || comparisonData.NumeratorProfiles.ComparisonCount < 4)
                &&
                (gvSuspect5.Rows.Count > 0 || comparisonData.NumeratorProfiles.KnownCount < 1 || comparisonData.DenominatorProfiles.KnownCount < 1)
                &&
                (gvSuspect6.Rows.Count > 0 || comparisonData.NumeratorProfiles.KnownCount < 2 || comparisonData.DenominatorProfiles.KnownCount < 2)
                &&
                (gvSuspect7.Rows.Count > 0 || comparisonData.NumeratorProfiles.KnownCount < 3 || comparisonData.DenominatorProfiles.KnownCount < 3)
                &&
                (gvSuspect8.Rows.Count > 0 || comparisonData.NumeratorProfiles.KnownCount < 4 || comparisonData.DenominatorProfiles.KnownCount < 4)
                &&
                gvUnknown.Rows.Count > 0)
            {
                btnRead.Visible = true;
            }

            // if comparison profiles were update for a bulk search using the 'from file' upload functionaly, we process it here
            lblKnownFileName.Text = "";
            if (dlTypes.Text == "From File" && comparisonData.Bulk)
            {
                // get a datatable and store it in the session (read Business_Interface.GetProfilesFromFile() for more information on the format)
                Session["Known_FromFile"] = bi.GetProfilesFromFile(fuPopulationUpload, Server.MapPath("~/Admin/Upload/"), comparisonData.LabKitID);
                Session["Known_FromFileName"] = fuPopulationUpload.FileName;
                // notify the user that we are running against this file so they are aware of whether this is happening or not.
                if (Session["Known_FromFile"] != null)
                    lblKnownFileName.Text = "Running against population from file: " + fuPopulationUpload.FileName;
                if (Session["Known_FromFile"] == null)
                    btnRead.Style["visibility"] = "hidden";
                else
                    btnRead.Style["visibility"] = "visible";
            }
            else btnRead.Style["visibility"] = "visible";
        }

        /// <summary>
        /// Generates a list with the locus sort order from the DataTable returned by the database
        /// </summary>
        /// <param name="locusSortOrder"></param>
        /// <returns></returns>
        private static List<string> LocusSortOrder(DataTable locusSortOrder)
        {
            List<string> val = new List<string>();

            foreach (DataRow dr in locusSortOrder.Rows)
                val.Add(dr["LocusName"].ToString());

            return val;
        }

        /// <summary>
        /// Generates a DataTable with Evidence replicates from the dictionary provided by either the Evidence control or the ComparisonData class
        /// while only using the columns provided by locusSortOrder which are usually Lab Kit dependent.
        /// </summary>
        /// <param name="evidence">Dictionary from which we take the per-replicate locus alleles.</param>
        /// <param name="locusSortOrder">DataTable containing the loci being used for this Lab Kit</param>
        /// <returns>A DataTable with the evidence alleles</returns>
        public static DataTable ConvertEvidenceFromDictionaryToDataTable(Dictionary<string, Dictionary<int, string>> evidence, DataTable locusSortOrder)
        {
            DataTable unknownTable = new DataTable();

            // add one row per replicate
            for (int replicate = 1; replicate <= 3; replicate++)
                unknownTable.Rows.Add(unknownTable.NewRow());

            // for each locus found in the lab kit
            foreach (string locus in LocusSortOrder(locusSortOrder))
            {
                // add the column to the table
                unknownTable.Columns.Add(locus, typeof(string));

                // add the alleles at each replicate
                for (int replicate = 1; replicate <= 3; replicate++)
                    if (evidence.ContainsKey(locus.ToUpper()))
                        unknownTable.Rows[replicate - 1][locus] = evidence[locus.ToUpper()][replicate].Trim();
            }

            return unknownTable;
        }

        /// <summary>
        /// Generates a DataTable with Profile alleles from the dictionary provided by either the Profile control or the ComparisonData class
        /// while only using the columns provided by locusSortOrder which are usually Lab Kit dependent.
        /// </summary>
        /// <param name="known">Dictionary from which we take the per-locus alleles.</param>
        /// <param name="locusSortOrder">DataTable containing the loci being used for this Lab Kit</param>
        /// <param name="intProfileNo">Key of the profile we are grabbing from the dictionary</param>
        /// <returns></returns>
        public static DataTable convertKnownFromDictionaryToTable(Dictionary<int, Dictionary<string, string>> known, DataTable locusSortOrder, int intProfileNo)
        {
            DataTable knownTable = new DataTable();

            // add a single row
            knownTable.Rows.Add(knownTable.NewRow());

            // for each locus found in the lab kit
            foreach (string locus in LocusSortOrder(locusSortOrder))
            {
                // add the locus as a column
                knownTable.Columns.Add(locus, typeof(string));

                // add the alleles at this locus, if any
                if (known[intProfileNo].ContainsKey(locus.ToUpper()))
                    knownTable.Rows[0][locus] = known[intProfileNo][locus.ToUpper()].Trim();
            }

            return knownTable;
        }

        /// <summary>
        /// This method gets called when the user clicks the "Compare" button on the UI. The data loading process does not actually happen here, 
        /// so take a look in the btnLoadData_Click() method (Preview button Click handler) for details.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRead_Click(object sender, EventArgs e)
        {
            // NOTE: on this page the values for the ComparisonData class get read in when you hit the "Preview" button

            bool isTest = sender is FST.Web.Admin.frmTestCenter;
            FST.Web.Admin.frmTestCenter testPage = isTest ? sender as FST.Web.Admin.frmTestCenter : null;
            if (isTest) throw new NotImplementedException("The File Upload functionality was never finished for the testing center on the new pages. If this must be tested independently of the manual entry, there shouldn't be that much work to do. Copy the way this works from the manual entry page. Most of it should already be in place.");//comparisonData = testPage.comparisonData;

            comparisonData.ReportDate = DateTime.Now;

            // validate form
            if (!isTest && !FieldChecks()) return;

            // go to the background service if we're doing a bulk or an individual search that runs long
            if (comparisonData.Bulk || comparisonData.RunIndividualOnService)
            {
                ProcessInBackgroundService(testPage);
            }
            else
            {
                if (!isTest) Log.Info(Context.User.Identity.Name, Request.FilePath, Session, "Comparison Start", comparisonData.HpHead + "/" + comparisonData.HdHead);

                // instantiate the comparison class with our comparison data
                FST.Common.Comparison comparison = new Comparison(comparisonData);

                Dictionary<string, float> result = comparison.DoCompare(null, null, null, null, null, null);

                if (!isTest) Log.Info(Context.User.Identity.Name, Request.FilePath, Session, "Comparison End", comparisonData.HpHead + "/" + comparisonData.HdHead);

                // print a PDF report
                Print();
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
        private void ProcessInBackgroundService(Admin.frmTestCenter testPage)
        {
            bool isTest = testPage != null;

            DataTable fromFileTable = null;
            Guid jobGuid = Guid.Empty;
            Guid fromFileID = Guid.Empty;

            // if we're doing a bulk comparison or we're doing a test
            if (comparisonData.BulkType == ComparisonData.enBulkType.FromFile || isTest)
            {
                // this is the GUID we send to the database that identifies this set of profiles as associated with this comparison
                fromFileID = Guid.NewGuid();
                comparisonData.FromFileGuid = fromFileID;
                comparisonData.FromFileName = Session["Known_FromFileName"].ToString();
                fromFileTable = (DataTable)Session["Known_FromFile"];
                // if we don't have a GUID column, add it.
                if (!fromFileTable.Columns.Contains("GUID"))
                    fromFileTable.Columns.Add(new DataColumn("GUID", typeof(Guid)));
                // set our GUID for each of the rows
                foreach (DataRow dr in fromFileTable.Rows)
                    dr["GUID"] = fromFileID;
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
                    webSvcClient.RunJob(jobGuid.ToString());
                }
                catch (Exception ex)
                {
                    Log.Error(User.Identity.Name.ToString(), (string)o, isTest ? null : Session, "Web Service Exception", string.Empty, ex);
                }
            }), isTest ? null : (object)Request.FilePath);

            // notify the user that the comparison will run in the background
            if (!isTest)
                MessageBox.Show("Your comparison will run in the background, and you will receive an e-mail with the results.");
        }

        private Dictionary<string, string> readKnownFileToDictionary(HttpPostedFile file, DataTable locusSortOrder)
        {
            if (!String.IsNullOrEmpty(file.FileName))
            {
                int FileLen;
                System.IO.Stream MyStream;

                Dictionary<string, string> val = new Dictionary<string, string>();

                try
                {
                    FileLen = file.ContentLength;
                    byte[] input = new byte[FileLen];

                    // Initialize the stream.
                    MyStream = file.InputStream;

                    // Read the file into the byte array.
                    MyStream.Read(input, 0, FileLen);
                    StringBuilder strUploadedContent = new StringBuilder("");
                    strUploadedContent.Append(Encoding.ASCII.GetString(input, 0, FileLen));

                    string txtOutPut = Server.HtmlEncode(strUploadedContent.ToString());
                    string[] list = txtOutPut.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (list.Length > 0)
                    {
                        if (list[0].ToUpper().Contains("REPLICATE"))
                        {
                            //replicate data
                            throw new Exception("Wrong Suspect1 File");
                        }
                        else
                        {
                            //suspect1 data
                            for (int i = 1; i < list.Length; i++)
                            {
                                list[i] = list[i].Trim();
                                string[] curAllele = list[i].Split('\t');
                                if (curAllele.Length >= 1)
                                {
                                    StringBuilder thisAllele = new StringBuilder("");
                                    for (int j = 1; j < curAllele.Length; j++)
                                    {
                                        thisAllele.Append(curAllele[j].ToString());
                                        thisAllele.Append(",");
                                    }
                                    if (!String.IsNullOrEmpty(thisAllele.ToString()))
                                    {
                                        if (thisAllele[thisAllele.Length - 1] == ',')
                                        {
                                            thisAllele.Remove(thisAllele.Length - 1, 1);
                                        }
                                    }
                                    string locus = curAllele[0].ToUpper();
                                    val.Add(locus, thisAllele.ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception eFile)
                {
                    MessageBox.Show("Error reading file: '" + file.FileName + "'. Please make sure you select a file in the correct format.");

                    return null;
                }

                List<string> loci = LocusSortOrder(locusSortOrder);

                foreach (string locus in loci)
                    if (!val.ContainsKey(locus.ToUpper()))
                        val.Add(locus.ToUpper(), string.Empty);

                return val;
            }
            return null;
        }

        private Dictionary<string, Dictionary<int, string>> readEvidenceFileToDictionary(HttpPostedFile file, DataTable locusSortOrder)
        {
            Dictionary<string, Dictionary<int, string>> val = new Dictionary<string, Dictionary<int, string>>();

            if (String.IsNullOrEmpty(file.FileName))
            {
                return val;
            }
            else
            {
                if (!String.IsNullOrEmpty(file.FileName))
                {
                    int FileLen;
                    System.IO.Stream MyStream;


                    try
                    {
                        FileLen = file.ContentLength;
                        byte[] input = new byte[FileLen];

                        // Initialize the stream.
                        MyStream = file.InputStream;

                        // Read the file into the byte array.
                        MyStream.Read(input, 0, FileLen);
                        StringBuilder strUploadedContent = new StringBuilder("");
                        strUploadedContent.Append(Encoding.ASCII.GetString(input, 0, FileLen));

                        string txtOutPut = Server.HtmlEncode(strUploadedContent.ToString());
                        string[] list = txtOutPut.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        if (list.Length > 0)
                        {
                            if (list[0].ToUpper().Contains("REPLICATE"))
                            {
                                //replicate data
                                val.Clear();

                                for (int i = 1; i < list.Length; i++)
                                {
                                    list[i] = list[i].Trim();
                                    string[] curAllele = list[i].Split('\t');
                                    //clean curAllele, remove all empty allele

                                    if (curAllele.Length >= 2)
                                    {
                                        StringBuilder thisAllele = new StringBuilder("");
                                        if (curAllele.Length > 2) //no allele
                                        {
                                            for (int j = 2; j < curAllele.Length; j++)
                                            {
                                                thisAllele.Append(curAllele[j].ToString());
                                                thisAllele.Append(",");
                                            }
                                        }
                                        //remove last ","
                                        if (!String.IsNullOrEmpty(thisAllele.ToString()))
                                        {
                                            if (thisAllele[thisAllele.Length - 1] == ',')
                                                thisAllele.Remove(thisAllele.Length - 1, 1);
                                        }

                                        if (val.ContainsKey(curAllele[0]))
                                            val[curAllele[0]].Add(Convert.ToInt32(curAllele[1].ToString()), thisAllele.ToString());
                                        else
                                        {
                                            Dictionary<int, string> newReplicate = new Dictionary<int, string>();
                                            newReplicate.Add(Convert.ToInt32(curAllele[1].ToUpper().ToString()), thisAllele.ToString());
                                            val.Add(curAllele[0], newReplicate);
                                        }


                                    }
                                }
                            }
                            else
                            {
                                throw new Exception("Wrong Unknown File");
                                return null;
                            }
                        }

                        List<string> loci = LocusSortOrder(locusSortOrder);
                        Dictionary<int, string> emptyLoci = new Dictionary<int, string>();
                        for (int replicate = 1; replicate <= 3; replicate++)
                            emptyLoci.Add(replicate, string.Empty);

                        foreach (string locus in loci)
                            if (!val.ContainsKey(locus.ToUpper()))
                                val.Add(locus.ToUpper(), emptyLoci);

                        return val;
                    }
                    catch (Exception eFile)
                    {
                        MessageBox.Show("Error reading file: '" + file.FileName + "'. Please make sure you select a file in the correct format.");
                        return null;
                    }
                }
            }
            return null;
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