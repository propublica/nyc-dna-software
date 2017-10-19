using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Data.OleDb;
using FST.Common;
using System.Threading;

public partial class Admin_frmKnownProfile : System.Web.UI.Page
{
    FST.Common.Business_Interface bi = new FST.Common.Business_Interface();

    protected override void OnPreInit(EventArgs e)
    {
        Session["LabKitID"] = Guid.Empty.ToString();
        base.OnPreInit(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["SortExpression"] == null)
            {
                Session["SortExpression"] = "ID";
                Session["SortDirection"] = SortDirection.Ascending;
            }

            PopulateGrid();
        }
    }

    protected void gvKnownProfile_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';this.style.textDecoration='underline';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.gvKnownProfile, "Select$" + e.Row.RowIndex);
        }
    }

    protected void gvKnownProfile_Sorted(object sender, EventArgs e)
    {
    }

    protected void gvKnownProfile_Sorting(object sender, GridViewSortEventArgs e)
    {
        DataTable dt = bi.GetMasterKnownProfile();
        if (dt != null)
        {
            using (DataView dv = new DataView(dt))
            {
                SortDirection sdSortDirection = SortDirection.Ascending;
                if (Session["SortExpression"] != null)
                    if (Session["SortExpression"].ToString() == e.SortExpression)
                    {
                        if ((SortDirection)Session["SortDirection"] == SortDirection.Ascending)
                            sdSortDirection = SortDirection.Descending;
                        else
                            sdSortDirection = SortDirection.Ascending;
                    }
                    else
                        sdSortDirection = SortDirection.Ascending;

                dv.Sort = e.SortExpression + " " + ConvertSortDirectionToSql(sdSortDirection);
                Session["SortExpression"] = e.SortExpression;
                Session["SortDirection"] = sdSortDirection;

                this.gvKnownProfile.DataSource = dv;
                this.gvKnownProfile.DataBind();
            }
        }
    }

    private string ConvertSortDirectionToSql(SortDirection sortDirection)
    {
        string newSortDirection = String.Empty;

        switch (sortDirection)
        {
            case SortDirection.Ascending:
                newSortDirection = "ASC";
                break;

            case SortDirection.Descending:
                newSortDirection = "DESC";
                break;
        }
        return newSortDirection;
    }

    protected void gvKnownProfile_SelectedIndexChanged(object sender, EventArgs e)
    {
        knownProfile.Reset();
        Session["SortExpression"] = null;
        SelectedRowData();
        Session["New"] = "False";
    }

    public void SelectedRowData()
    {
        string keyId = Convert.ToString(gvKnownProfile.DataKeys[gvKnownProfile.SelectedIndex].Value, CultureInfo.CurrentCulture);
        DataTable dt = bi.GetDetailKnownProfile(keyId);
        this.lblId.Text = keyId;
        this.tID.Text = keyId;

        foreach (DataColumn dc in dt.Columns)
            if (dc.ColumnName != "ID" && dc.ColumnName != "Active")
                knownProfile.Alleles[dc.ColumnName] = dt.Rows[0][dc.ColumnName].ToString();

        this.dlActive.Text = dt.Rows[0]["Active"].ToString();
        this.tID.ReadOnly = true;
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        Session["SortExpression"] = null;
        this.lblId.Text = "";
        this.tID.Text = "";

        knownProfile.Reset();
        this.tID.ReadOnly = false;

        this.dlActive.Text = Convert.ToString("Yes", CultureInfo.CurrentCulture);
        Session["New"] = Convert.ToString("True", CultureInfo.CurrentCulture);
    }

    public void PopulateGrid()
    {
        DataTable dt = bi.GetMasterKnownProfile();
        using (DataView dv = new DataView(dt))
        {
            if (Session["SortExpression"] == null)
            {
                Session["SortExpression"] = "ID";
                Session["SortDirection"] = SortDirection.Ascending;
            }

            dv.Sort = Session["SortExpression"].ToString() + " " + ConvertSortDirectionToSql((SortDirection)Session["SortDirection"]);

            this.gvKnownProfile.DataSource = dv;
            this.gvKnownProfile.DataBind();
        }
    }

    protected void gvKnownProfile_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvKnownProfile.PageIndex = e.NewPageIndex;
        //PopulateGrid();
        DataTable dt = bi.GetMasterKnownProfile();
        if (dt != null)
        {
            using (DataView dv = new DataView(dt))
            {
                if (Session["SortExpression"] != null)
                {
                    dv.Sort = Session["SortExpression"].ToString() + " " + ConvertSortDirectionToSql((SortDirection)Session["SortDirection"]);
                }
                this.gvKnownProfile.DataSource = dv;
                this.gvKnownProfile.DataBind();
            }
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (tID.Text.Trim() == string.Empty)
        {
            MessageBox.Show("You must enter an ID for the new profile before submitting.");
            return;
        }
        Session["SortExpression"] = null;
        SaveData();
    }

    public void SaveData()
    {
        if (Session["New"] == null)
            return;
        string[] strParameters = new string[17];
        strParameters[0] = this.tID.Text;

        string strErrMsg = bi.SaveKnownProfile(Session["New"].ToString(), tID.Text, dlActive.Text, knownProfile.GetProfileDictionary());

        if (String.IsNullOrEmpty(strErrMsg))
        {
            if (Session["New"].ToString() == "False")
            {
                Log.Info(Context.User.Identity.Name, Request.FilePath, Session, "Edited Lab Type", this.tID.Text);
                PopulateGrid();
                SelectedRowData();
            }
            else
            {
                Log.Info(Context.User.Identity.Name, Request.FilePath, Session, "Added Lab Type", this.tID.Text);
                PopulateGrid();
            }
        }
        else
        {
            MessageBox.Show(strErrMsg);
        }
    }

    protected void chkActive_CheckedChanged(object sender, EventArgs e)
    {
        Session["SortExpression"] = null;
        string strErrMsg = bi.UpdateKnownProfileStatus(chkActive.Checked == true ? "Yes" : "No");
        PopulateGrid();
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        List<string> toDelete = new List<string>();
        bool isChecked = false;
        string curID = string.Empty;

        foreach (GridViewRow gvr in gvKnownProfile.Rows)
        {
            isChecked = false;
            foreach (Control c in gvr.Controls)
                foreach (Control child in c.Controls)
                {
                    if (child.ID == "chkDelete" && (child as CheckBox).Checked)
                    {
                        curID = string.Empty;
                        isChecked = true;
                    }
                    if (child.ID == "lblID")
                        curID = (child as Label).Text;
                    if (isChecked && curID != string.Empty && !toDelete.Contains(curID))
                        toDelete.Add(curID);
                }
        }
        foreach (string strID in toDelete)
        {
            bi.DeleteKnownProfileByID(strID);
            Log.Info(Context.User.Identity.Name, Request.FilePath, Session, "Deleted Lab Type", strID);
        }

        PopulateGrid();
    }

    protected void btnOn_Click(object sender, EventArgs e)
    {
        bool enabled = true;
        List<string> activated = ActivateDeactivateProfile(enabled);
        PopulateGrid();
        //RecheckActivateDeactivateCheckboxesAfterRefresh(activated);
    }

    protected void btnOff_Click(object sender, EventArgs e)
    {
        bool enabled = false;
        List<string> activated = ActivateDeactivateProfile(enabled);
        PopulateGrid();
        //RecheckActivateDeactivateCheckboxesAfterRefresh(activated);
    }

    //private void RecheckActivateDeactivateCheckboxesAfterRefresh(List<string> activated)
    //{
    //    string curID = string.Empty;
    //    foreach (string ID in activated)
    //        foreach (GridViewRow gvr in gvKnownProfile.Rows)
    //            foreach (Control c in gvr.Controls)
    //            {
    //                foreach (Control child in c.Controls)
    //                {
    //                    if (child.ID == "lblID")    // order matters here, so we set isChecked to null until we get a value for it
    //                    {
    //                        curID = (child as Label).Text;
    //                    }
    //                    if (child.ID == "chkActivateDeactivate" && curID == ID)
    //                    {
    //                        (child as CheckBox).Checked = true;
    //                    }
    //                }
    //            }
    //}

    private List<string> ActivateDeactivateProfile(bool enabled)
    {
        List<string> toActivate = new List<string>();
        bool isChecked;
        string curID = string.Empty;

        foreach (GridViewRow gvr in gvKnownProfile.Rows)
            foreach (Control c in gvr.Controls)
            {
                isChecked = false;
                foreach (Control child in c.Controls)
                {
                    if (child.ID == "lblID")    // order matters here, so we set isChecked to null until we get a value for it
                    {
                        curID = (child as Label).Text;
                    }
                    if (child.ID == "chkActivateDeactivate" && (child as CheckBox).Checked)
                    {
                        isChecked = true;
                    }
                    if (curID != string.Empty && isChecked && !toActivate.Contains(curID))
                        toActivate.Add(curID);
                }
            }
        foreach (string keyID in toActivate)
            bi.UpdateKnownProfileStatusByID(keyID, enabled);
        return toActivate;
    }

    protected void Upload_Click(object sender, EventArgs e)
    {
        string strErrMsg = string.Empty;
        DataTable dt = null;

        try
        {
            string connString = string.Empty;
            string extension = string.Empty;
            // determine file type
            if (FileUpload1.FileName.EndsWith(".csv")) extension = ".csv";
            else if (FileUpload1.FileName.EndsWith(".xlsx")) extension = ".xlsx";
            else if (FileUpload1.FileName.EndsWith(".xls")) extension = ".xls";
            // save file
            string filename = DateTime.Now.Ticks.ToString() + extension;
            FileUpload1.SaveAs(Server.MapPath("~/Admin/Upload/" + filename));

            // create connection string
            switch (extension)
            {
                case ".csv":
                    connString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/Admin/Upload/") + ";Extended Properties='text;HDR=YES;FMT=CSVDelimited';");
                    break;
                case ".xlsx":
                    connString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/Admin/Upload/") + filename + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=2';");
                    break;
                case ".xls":
                    connString = string.Format("Provider=Microsoft.Jet.OLEDB.12.0;Data Source=" + Server.MapPath("~/Admin/Upload/") + filename + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1';");
                    break;
            }

            // open the file
            using (OleDbConnection cn = new OleDbConnection(connString))
            {
                cn.Open();
                string query = string.Empty;

                string sheetName = "Sheet1$";

                DataTable dtTables = cn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);
                if (dtTables.Rows.Count == 1)
                    sheetName = (string)dtTables.Rows[0]["TABLE_NAME"];

                switch (extension)
                {
                    case ".csv": query = "SELECT * FROM [" + filename + "]"; break;
                    case ".xlsx": query = "SELECT * FROM [" + sheetName + "]"; break;
                    case ".xls": query = "SELECT * FROM [" + sheetName + "]"; break;
                }
                // read file
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, cn);
                dt = new DataTable();
                adapter.Fill(dt);
            }
        }
        catch (Exception ex)
        {
            // handle errors
            MessageBox.Show("There was an error reading the uploaded file. Please try uploading an Excel or CSV file in the correct format.");
            return;
        }

        int added = 0;
        int noId = 0;
        List<string> noAlleles = new List<string>();
        List<string> alreadyInDB = new List<string>();
        string otherErrors = string.Empty;

        try
        {
            // get number of records
            int numberOfProfiles = dt.Rows.Count;

            int errorCount = 0;

            // write to DB
            foreach (DataRow dr in dt.Rows)
            {
                bool noAllelesAtAnyLocus = true;
                foreach (DataColumn dc in dt.Columns)
                    if (dc.ColumnName != "ID")
                        if (dr[dc.ColumnName].ToString().Trim() != string.Empty)
                            noAllelesAtAnyLocus = false;
                if (noAllelesAtAnyLocus)
                {
                    // add to error list
                    errorCount++;
                    noAlleles.Add(dr["ID"].ToString());
                    continue;
                }

                if (dr["ID"].ToString() == string.Empty)
                {
                    // add to error list
                    noId++;
                    errorCount++;
                    continue;
                }

                // break on cells where there are no commas between values
                foreach (DataColumn dc in dt.Columns)
                    if (dc.ColumnName.ToUpper() != "ID")
                    {
                        dr[dc.ColumnName] = dr[dc.ColumnName].ToString().Trim();
                        if (dr[dc.ColumnName].ToString().IndexOf(",") == -1 && dr[dc.ColumnName].ToString().Contains(' ')) // no commas, values separated by space
                        {
                            string[] arr = dr[dc.ColumnName].ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                            if (arr.Length != 2)
                            {
                                MessageBox.Show("There were too many values in one of the data cells.");
                                return;
                            }

                            dr[dc.ColumnName] = arr[0] + "," + arr[1];
                        }

                        if (dr[dc.ColumnName].ToString().IndexOf(",") != dr[dc.ColumnName].ToString().LastIndexOf(","))  // too many commas
                        {
                            MessageBox.Show("There were too many commas one of the data cells.");
                            return;
                        }
                    }

                // if we have a single value then we duplicate it (sorry for this)
                foreach (DataColumn dc in dt.Columns)
                    if (dc.ColumnName != "ID")
                        if (!dr[dc.ColumnName].ToString().Contains(",")
                            || string.IsNullOrEmpty(dr[dc.ColumnName].ToString().Trim().Split(new char[] { ',' })[1]))
                            if (!dr[dc.ColumnName].ToString().Contains(",") || string.IsNullOrEmpty(dr[dc.ColumnName].ToString().Trim().Split(new char[] { ',' })[1])) dr[dc.ColumnName] = dr[dc.ColumnName].ToString().Trim().Replace(",", "") + "," + dr[dc.ColumnName].ToString().Trim().Replace(",", "");

                foreach (DataColumn dc in dt.Columns)
                    if (dc.ColumnName.ToUpper() != "ID")
                    {
                        string vallele = dr[dc.ColumnName].ToString();
                        string[] valleles = vallele.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        dr[dc.ColumnName] = valleles.Length > 0 ? valleles[0].Trim() + "," + valleles[1].Trim() : string.Empty;
                    }

                Dictionary<string, string> knownAlleles = new Dictionary<string, string>();
                foreach (DataColumn dc in dt.Columns)
                    if (dc.ColumnName != "ID")
                        knownAlleles.Add(dc.ColumnName, dr[dc.ColumnName].ToString());

                string error = bi.SaveKnownProfile("True", dr["ID"].ToString(), "Yes", knownAlleles);

                // add to error list
                if (!string.IsNullOrEmpty(error))
                {
                    if (error.Contains("Violation of PRIMARY KEY constraint"))
                        alreadyInDB.Add(dr["ID"].ToString());
                    else
                        otherErrors += error;
                    errorCount++;
                }
                else
                    added++;
            }

            // write back message
            lblUploadResult.Text = "\r\n" + (numberOfProfiles - errorCount) + " added to the Lab Types.";


            string errorStr = added + " added to DB. " + errorCount + " not added to DB.";

            if (noId > 0)
                errorStr += " " + noId + " rows were found with no ID value. ";

            if (alreadyInDB.Count > 0)
            {
                errorStr += "The following IDs were already in the DB: ";
                foreach (string ID in alreadyInDB)
                    errorStr += ID + ", ";
                errorStr = errorStr.Substring(0, errorStr.Length - 2) + ". ";
            }

            if (noAlleles.Count > 0)
            {
                errorStr += "The following IDs had no allele values in the file: ";
                foreach (string ID in noAlleles)
                    errorStr += ID + ", ";
                errorStr = errorStr.Substring(0, errorStr.Length - 2) + ". ";
            }

            if (!string.IsNullOrEmpty(otherErrors))
                errorStr += " The following other errors were also encountered: " + otherErrors;

            lblUploadResult.Text = errorStr;

            if (added > 0)
            {
                Log.Info(Context.User.Identity.Name, Request.FilePath, Session, "Mass Added Profiles", errorStr);
            }
            else
            {
                MessageBox.Show("No profiles were uploaded. Please check the page for error messages.");
                Log.Error(Context.User.Identity.Name, Request.FilePath, Session, "Error Mass Adding Profiles", errorStr, new Exception("Error Mass Adding Profiles"));
            }
        }
        catch (Exception ex)
        {
            // handle errors
            Log.Error(Context.User.Identity.Name, Request.FilePath, Session, "Error Mass Adding Profiles", "File Parsing Error", ex);
            MessageBox.Show("There was an error reading the uploaded file. Please try uploading an Excel or CSV file in the correct format.");
        }
    }

    public void lnkDownload_Click(object sender, EventArgs e)
    {
        string strErrMsg = string.Empty;
        string connString = string.Empty;
        string extension = ".xlsx";
        // save file
        string filename = DateTime.Now.Ticks.ToString() + extension;
        string sheetName = "Profiles$";

        File.Copy(Server.MapPath("~/Reports/ProfileDownloadBlank.xlsx"), Server.MapPath("~/Admin/Upload/" + filename));

        // create connection string
        connString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/Admin/Upload/") + filename + ";Extended Properties='Excel 12.0;HDR=No';");

        DataTable dt = bi.GetKnown_Profile("Lab Types", Guid.Empty);
        dt.Columns["ID"].SetOrdinal(0);

        // open the file
        using (OleDbConnection cn = new OleDbConnection(connString))
        {
            cn.Open();
            string query = string.Empty;

            DataTable dtTables = cn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);
            if (dtTables.Rows.Count == 1)
                sheetName = (string)dtTables.Rows[0]["TABLE_NAME"];

            query = "SELECT * FROM [" + sheetName + "]";

            OleDbDataAdapter adapter = new OleDbDataAdapter(query, cn);
            DataTable dt2 = new DataTable();
            adapter.Fill(dt2);

            dt2.Columns["ID"].SetOrdinal(1);

            int x = 1;
            foreach (DataColumn dc in dt.Columns)
            {
                query = "UPDATE [" + sheetName + "] SET F" + x.ToString() + " = '" + dc.ColumnName + "'";
                OleDbCommand cmd = new OleDbCommand(query, cn);
                cmd.ExecuteNonQuery();
                x++;
            }

            foreach (DataRow dr in dt.Rows)
            {
                OleDbCommand cmd = new OleDbCommand();

                string cmdText = "INSERT INTO [" + sheetName + "] (";
                for (int i = 1; i <= dt.Columns.Count; i++)
                {
                    cmdText += "F" + i.ToString() + ",";
                }
                cmdText = cmdText.Substring(0, cmdText.Length - 1);
                cmdText += ") VALUES (";
                int j = 1;
                foreach (DataColumn dc in dt.Columns)
                {
                    cmdText += "@F" + j.ToString() + ",";
                    j++;
                    cmd.Parameters.AddWithValue("@F" + j.ToString(), dr[dc.ColumnName].ToString());
                }
                cmdText = cmdText.Substring(0, cmdText.Length - 1);
                cmdText += ")";

                cmd.CommandText = cmdText;
                cmd.Connection = cn;

                cmd.ExecuteNonQuery();
            }

            // read file
            query = "SELECT * FROM [" + sheetName + "]";
            adapter = new OleDbDataAdapter(query, cn);
            dt = new DataTable();
            adapter.Fill(dt);

            cn.Close();

            GC.Collect();
        }

        // we check for whether we can send out the file by seeing if we can write to the file
        while (IsFileLocked(new FileInfo(Server.MapPath("~/Admin/Upload/" + filename))))
        {
            GC.Collect();   // do not remove this, the ACE OLEDB has GC issues and invoking the collector causes it to flush its buffer to disk
            Thread.Sleep(1000);
        }

        Response.Clear();
        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        Response.AddHeader("Content-Disposition", "attachment; filename=LabTypes.xlsx");
        Response.BinaryWrite(File.ReadAllBytes(Server.MapPath("~/Admin/Upload/" + filename)));
        Response.End();
        File.Delete(filename);
    }

    /// <summary>
    /// Check if a certain file is locked by try/catch'ing a file open with exclusive write access
    /// </summary>
    /// <param name="file">FileInfo class describing the file we are checking</param>
    /// <returns>Boolean value that describes whether we are unable to gain exclusive access to the file</returns>
    private bool IsFileLocked(FileInfo file)
    {
        FileStream stream = null;

        try
        {
            stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }
        catch (IOException)
        {
            return true;
        }
        finally
        {
            if (stream != null)
                stream.Close();
        }
        return false;
    }
}
