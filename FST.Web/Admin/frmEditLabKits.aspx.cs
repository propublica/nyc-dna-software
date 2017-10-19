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
using System.Data.OleDb;
using System.Collections.Generic;
using System.Data.SqlClient;
using FST.Common;
using System.Globalization;

public partial class frmEditLabKits : System.Web.UI.Page
{
    FST.Common.Business_Interface bi = new FST.Common.Business_Interface();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // get options for number of persons involved from the database and bind to our dropdown
            DataTable dt = bi.GetNoOfPersonsInvolved();
            this.ddlNoOfPersons.DataSource = dt;
            DataRow row = dt.NewRow();
            row["FieldName"] = DBNull.Value;
            row["FieldValue"] = DBNull.Value;
            dt.Rows.Add(row);
            this.ddlNoOfPersons.DataTextField = "FieldName";
            this.ddlNoOfPersons.DataValueField = "FieldValue";
            this.ddlNoOfPersons.DataBind();
            this.ddlNoOfPersons.Text = "";
            dt.Clear();

            // get dropout rate types from the database and bind to our dropdown (PHET1, PHET2, PHOM)
            dt = bi.GetDropOutTypes();
            this.ddlDropOutType.DataSource = dt;
            row = dt.NewRow();
            row["FieldName"] = DBNull.Value;
            row["FieldValue"] = DBNull.Value;
            dt.Rows.Add(row);
            this.ddlDropOutType.DataTextField = "FieldName";
            this.ddlDropOutType.DataValueField = "FieldValue";
            this.ddlDropOutType.DataBind();
            this.ddlDropOutType.Text = "";
            dt.Clear();

            // get drop out options from the database and bind to our dropdown 
            // these are the rate points, so 6.25pg, 12.5pg, 25pg, etc. and also refered to as DNA template amount in the UI
            dt = bi.getDropOutOptions();
            this.ddlDropOutOption.DataSource = dt;
            row = dt.NewRow();
            row["FieldName"] = DBNull.Value;
            row["FieldValue"] = DBNull.Value;
            dt.Rows.Add(row);
            this.ddlDropOutOption.DataTextField = "FieldName";
            this.ddlDropOutOption.DataValueField = "FieldValue";
            this.ddlDropOutOption.DataBind();
            this.ddlDropOutOption.Text = "";
            dt.Clear();

            // get the labk kits from the database and bind to our dropdown
            dt = bi.GetLabKits();
            this.gvLabKits.DataSource = dt;
            this.gvLabKits.DataBind();
            dt.Clear();

            // get the list of loci from the database and bind to our dropdown
            dt = bi.GetLocus(Guid.Empty);
            this.ddlLocus.DataSource = dt;
            row = dt.NewRow();
            row["FieldName"] = DBNull.Value;
            row["FieldValue"] = DBNull.Value;
            dt.Rows.Add(row);
            this.ddlLocus.DataTextField = "FieldName";
            this.ddlLocus.DataValueField = "FieldValue";
            this.ddlLocus.DataBind();
            this.ddlLocus.Text = "";

            this.ddlDeducible.Text = "";
        }
    }

    #region Lab Kit Controls
    /// <summary>
    /// Handles clicks for the New Lab Kit button. Shows the appropriate panels (file upload)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnNewLabKit_Click(object sender, EventArgs e)
    {
        this.pnlUploadNew.Visible = true;
        this.pnlSearch.Visible = false;
        this.pnlGrid.Visible = false;
        this.pnlDropIn.Visible = false;
    }

    /// <summary>
    /// Handles Lab Kit delete button click. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lnkDelete_Click(object sender, EventArgs e)
    {
        // this is a hidden field in the table with a Lab Kit ID
        string Guid = ((sender as Control).Parent.Parent.Controls[1] as DataControlFieldCell).Text;

        // since we're deleting the Lab Kit, we get rid of the session variable. we're no longer editing anything
        if (!string.IsNullOrEmpty((string)Session["EditLabKitID"]) && Guid == Session["EditLabKitID"].ToString()) Session["EditLabKitID"] = string.Empty;

        // delete the lab kit and log this
        try
        {
            bi.DeleteLabKit(Guid);
            Log.Info(Context.User.Identity.Name, Request.FilePath, Session, "Delete Lab Kit", Guid);
        }
        catch (Exception ex)
        {
            Log.Error(Context.User.Identity.Name, Request.FilePath, Session, "Delete Lab Kit Failed", string.Empty, ex);
        }

        // refresh the lab kits
        DataTable dt = null;
        dt = bi.GetLabKits();
        this.gvLabKits.DataSource = dt;
        this.gvLabKits.DataBind();

        // show the right panels
        this.pnlSearch.Visible = false;
        this.pnlGrid.Visible = false;
        this.pnlDropIn.Visible = false;
    }

    /// <summary>
    /// Handles the Lab Kit edit button click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lnkEdit_Click(object sender, EventArgs e)
    {
        // get Guid from our hidden field in the table.. 
        string Guid = ((sender as Control).Parent.Parent.Controls[1] as DataControlFieldCell).Text;
        // get the name from the table too
        string Name = ((sender as Control).Parent.Parent.Controls[0] as DataControlFieldCell).Text;
        
        this.lblLabKitNameEdit.Text = "Currently Editing: " + Name;
        // we set this so the other parts of the page know which lab kit we are editing
        Session["EditLabKitID"] = Guid;

        // populate the grids with data for this labkit
        PopulateDropOutGrid();
        PopulateDropInGrid();

        // show the right panels
        this.pnlGrid.Visible = true;
        this.pnlSearch.Visible = true;
        this.pnlDropIn.Visible = true;
        this.pnlUploadNew.Visible = false;
    }
    #endregion

    #region Lab Kit Upload
    /// <summary>
    /// Handles the upload button event. We save the file to disk, attempt to read a Lab Kit from it, and attempt to save it to the DB
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnLabKitUpload_Click(object sender, EventArgs e)
    {
        DataTable dt = null;
        List<int> labKitLoci = new List<int>();

        // make sure we have a lab kit name
        if (string.IsNullOrEmpty(txtLabKitName.Text))
        {
            MessageBox.Show("Please enter a name for the Lab Kit.");
            return;
        }

        // make sure a file has been uploaded with this post
        if (string.IsNullOrEmpty(FileUpload1.FileName))
        {
            MessageBox.Show("Please select a Lab Kit file.");
            return;
        }

        try
        {
            string connString = string.Empty;
            string extension = string.Empty;
            // determine file type
            if (FileUpload1.FileName.EndsWith(".xlsx")) extension = ".xlsx";
            else if (FileUpload1.FileName.EndsWith(".xls")) extension = ".xls";
            // save file
            string filename = DateTime.Now.Ticks.ToString() + extension;
            FileUpload1.SaveAs(Server.MapPath("~/Admin/Upload/" + filename));

            // create connection string
            switch (extension)
            {
                case ".xlsx":
                    connString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/Admin/Upload/") + filename + ";Extended Properties='Excel 12.0;HDR=No;IMEX=1';");
                    break;
                case ".xls":
                    connString = string.Format("Provider=Microsoft.Jet.OLEDB.12.0;Data Source=" + Server.MapPath("~/Admin/Upload/") + filename + ";Extended Properties='Excel 8.0;HDR=No;IMEX=1';");
                    break;
            }

            // create a dropout rate table
            DataTable dtDropouts = new DataTable();
            dtDropouts.Columns.Add(new DataColumn("LabKitID", typeof(Guid)));
            dtDropouts.Columns.Add(new DataColumn("LocusID", typeof(int)));
            dtDropouts.Columns.Add(new DataColumn("TypeID", typeof(int)));
            dtDropouts.Columns.Add(new DataColumn("DropOptionID", typeof(int)));
            dtDropouts.Columns.Add(new DataColumn("DropoutRate", typeof(double)));
            dtDropouts.Columns.Add(new DataColumn("NoOfPersonsInvolvd", typeof(int)));
            dtDropouts.Columns.Add(new DataColumn("Deducible", typeof(string)));

            DataTable dtLocus = bi.GetLocus(Guid.Empty);

            // get drop out rates
            // for 1 through 4 people, obviously
            for (int i = 1; i <= 4; i++)
            {
                // open the file and read the data from the sheet which we are working (named "1 person" and so on)
                using (OleDbConnection cn = new OleDbConnection(connString))
                {
                    cn.Open();
                    string query = string.Empty;

                    switch (extension)
                    {
                        case ".xlsx": query = "SELECT * FROM [" + i + " person$]"; break;
                        case ".xls": query = "SELECT * FROM [" + i + " person$]"; break;
                    }
                    // read file
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, cn);
                    dt = new DataTable();
                    adapter.Fill(dt);
                }

                string deducible = string.Empty;

                List<DropOutRow> vals = new List<DropOutRow>();

                // get loci
                int rowIdx = 2, colIdx = 2;

                List<string> locusNames = new List<string>();
                while (colIdx < dt.Columns.Count && dt.Rows[rowIdx][colIdx].ToString().Trim() != string.Empty)
                    locusNames.Add(dt.Rows[rowIdx][colIdx++].ToString().Trim());

                Dictionary<string, int> locusIDMap = new Dictionary<string, int>();
                foreach (DataRow dr in dtLocus.Rows)
                    if (locusNames.Contains(dr["FieldName"].ToString(), StringComparer.Create(CultureInfo.CurrentCulture, true)))
                        locusIDMap.Add(dr["FieldName"].ToString().ToUpper(), Convert.ToInt32(dr["FieldValue"].ToString()));

                locusNames.Clear();
                foreach (string locus in locusIDMap.Keys)
                    locusNames.Add(locus);

                // set up the lab kit loci. we will be sending these to the database later
                if (labKitLoci.Count == 0)
                    foreach (string locus in locusIDMap.Keys)
                        labKitLoci.Add(locusIDMap[locus]);

                // get the deducible, PHET1, for persons i (which will be 1 through 4, see above)
                int firstRow = 3, firstCol = 2, type = 1, persons = i;
                deducible = "Yes";
                vals.AddRange(CreateRows(dt, deducible, locusNames, locusIDMap, firstRow, firstCol, type, persons));

                // get the deducible, PHET2, for persons i (which will be 1 through 4, see above)
                firstRow = 14;
                firstCol = 2;
                type = 2;
                persons = i;
                deducible = "Yes";
                vals.AddRange(CreateRows(dt, deducible, locusNames, locusIDMap, firstRow, firstCol, type, persons));

                // get the deducible, PHOM1, for persons i (which will be 1 through 4, see above)
                firstRow = 25;
                firstCol = 2;
                type = 3;
                persons = i;
                deducible = "Yes";
                vals.AddRange(CreateRows(dt, deducible, locusNames, locusIDMap, firstRow, firstCol, type, persons));

                // if we're doing more than one person, then get the non-deducible too
                if (i > 1)
                {
                    // get the non-deducible, PHET1, for persons i (which will be 1 through 4, see above)
                    firstRow = 38;
                    firstCol = 2;
                    type = 1;
                    persons = i;
                    deducible = "No";
                    vals.AddRange(CreateRows(dt, deducible, locusNames, locusIDMap, firstRow, firstCol, type, persons));

                    // get the non-deducible, PHET2, for persons i (which will be 1 through 4, see above)
                    firstRow = 49;
                    firstCol = 2;
                    type = 2;
                    persons = i;
                    deducible = "No";
                    vals.AddRange(CreateRows(dt, deducible, locusNames, locusIDMap, firstRow, firstCol, type, persons));

                    // get the non-deducible, PHOM1, for persons i (which will be 1 through 4, see above)
                    firstRow = 60;
                    firstCol = 2;
                    type = 3;
                    persons = i;
                    deducible = "No";
                    vals.AddRange(CreateRows(dt, deducible, locusNames, locusIDMap, firstRow, firstCol, type, persons));
                }
                
                // go through our DropOutRows we created from the Excel file and add them to our table
                foreach (DropOutRow drow in vals)
                {
                    DataRow dr = dtDropouts.NewRow();
                    dr["LocusID"] = drow.LocusID;
                    dr["TypeID"] = drow.TypeID;
                    dr["DropOptionID"] = drow.DropOptionID;
                    dr["DropoutRate"] = drow.DropoutRate;
                    dr["NoOfPersonsInvolvd"] = drow.NoOfPersonsInvolvd;
                    dr["Deducible"] = drow.Deducible;
                    dtDropouts.Rows.Add(dr);
                }
                vals.Clear();
            }

            // create a dropins table
            DataTable dtDropins = new DataTable();
            dtDropins.Columns.Add(new DataColumn("LabKitID", typeof(Guid)));
            dtDropins.Columns.Add(new DataColumn("DropInRateID", typeof(string)));
            dtDropins.Columns.Add(new DataColumn("Type", typeof(string)));
            dtDropins.Columns.Add(new DataColumn("DropInRate", typeof(double)));

            // get drop in rates
            // open the file and get the data from the dropins table
            using (OleDbConnection cn = new OleDbConnection(connString))
            {
                cn.Open();
                string query = string.Empty;

                switch (extension)
                {
                    case ".xlsx": query = "SELECT * FROM [Drop-in$]"; break;
                    case ".xls": query = "SELECT * FROM [Drop-in$]"; break;
                }
                // read file
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, cn);
                dt = new DataTable();
                adapter.Fill(dt);
            }

            // go through the specific areas in the file
            for (int i = 2; i <= 7; i++)
            {
                DataRow dr = dtDropins.NewRow();
                dr["DropinRateID"] = dt.Rows[i][0];
                dr["Type"] = dt.Rows[i][1];
                dr["DropInRate"] = dt.Rows[i][2];
                dtDropins.Rows.Add(dr);
            }

            // save the lab kit name and get back a GUID for it
            string guid = bi.SaveLabKit(txtLabKitName.Text, "");
            Log.Info(Context.User.Identity.Name, Request.FilePath, Session, "Uploaded Lab Kit", guid);

            // add that GUID to the tables of drop-ins and drop-outs
            foreach (DataRow dr in dtDropouts.Rows)
                dr["LabKitID"] = guid;
            foreach (DataRow dr in dtDropins.Rows)
                dr["LabKitID"] = guid;

            // save the drop-ins and drop-outs to the database
            bi.SaveLabKitData(guid, dtDropouts, dtDropins, labKitLoci);

            // refresh the list of lab kits (should now include the new one)
            DataTable dt2 = bi.GetLabKits();
            this.gvLabKits.DataSource = dt2;
            this.gvLabKits.DataBind();
            txtLabKitName.Text = string.Empty;
        }
        catch (Exception ex)
        {
            // handle errors
            Log.Error(Context.User.Identity.Name, Request.FilePath, Session, "Error Adding Lab Kit", "File Parsing Error", ex);
            MessageBox.Show("There was an error reading the uploaded file. Please try uploading an Excel file in the correct format.");
            return;
        }
    }

    /// <summary>
    /// This method takes a bunch of parameters and does goes through a drop-out rate table to create new DropOutRow instances which we eventually insert into the DB
    /// </summary>
    /// <param name="dt">DataTable with our source data</param>
    /// <param name="deducible">Whether we're working on Deducible drop-outs ("Yes" or "No", goes right into the DropOutRow)</param>
    /// <param name="locusNames">Names of the loci for which we try to pull drop-out rates (this is to be reconciled between loci in the system and the loci in the spreadsheet in the caller method)</param>
    /// <param name="locusIDMap">A map of locus name strings to locus ID integers</param>
    /// <param name="firstRow">Row index at which the table starts in the source DataTable</param>
    /// <param name="firstCol">Column index at which the table starts in the source DataTable</param>
    /// <param name="type">The drop out rate type (will be 1,2,3 representing PHET1, PHET2, PHOM</param>
    /// <param name="persons">Number of persons involved</param>
    /// <returns></returns>
    private List<DropOutRow> CreateRows(DataTable dt, string deducible, List<string> locusNames, Dictionary<string, int> locusIDMap, int firstRow, int firstCol, int type, int persons)
    {
        List<DropOutRow> val = new List<DropOutRow>();

        for (int colIdx = firstCol; colIdx < firstCol + locusNames.Count; colIdx++)
        {
            // count through all the options (there are 9, hence the 9 in the loop below)
            int option = 1;
            for (int rowIdx = firstRow; rowIdx < firstRow + 9; rowIdx++)
            {
                // fill out the class withour parameters and associated data. We pull the rate from the table and the locusID from the map. everything else is passed in
                DropOutRow dor = new DropOutRow
                {
                    Deducible = deducible,
                    DropOptionID = option,
                    LocusID = locusIDMap[dt.Rows[firstRow - 1][colIdx].ToString().Trim().ToUpper()],
                    DropoutRate = Convert.ToDouble(dt.Rows[rowIdx][colIdx].ToString()),
                    NoOfPersonsInvolvd = persons,
                    TypeID = type
                };
                val.Add(dor);
                option++;
            }
        }

        return val;
    }

    /// <summary>
    /// This class has the fields required to fill out a drop-out row in the database. it is used as an intermediary between the excel sheet and the final data table
    /// </summary>
    public class DropOutRow
    {
        public string LabKitID;
        public int LocusID;
        public int TypeID;
        public int DropOptionID;
        public double DropoutRate;
        public int NoOfPersonsInvolvd;
        public string Deducible;
    }
    #endregion

    #region Drop-out rate editing
    public void PopulateDropOutGrid()
    {
        if (!string.IsNullOrEmpty(Session["EditLabKitID"].ToString()))
        {
            // get drop out data for our lab kit
            DataTable dt = bi.GetDropOutData(Session["EditLabKitID"].ToString());
            using (DataView dv = new DataView(dt))
            {
                // create our filter
                string strRowFilter = "";
                if (!String.IsNullOrEmpty(this.ddlNoOfPersons.SelectedValue))
                    strRowFilter = "NoOfPersonsInvolvd = '" + this.ddlNoOfPersons.SelectedValue + "'";
                if (!String.IsNullOrEmpty(this.ddlDropOutType.SelectedValue))
                {
                    if (!String.IsNullOrEmpty(strRowFilter))
                        strRowFilter += " and ";
                    strRowFilter += "typeID = '" + this.ddlDropOutType.SelectedValue + "'";
                }
                if (!String.IsNullOrEmpty(this.ddlDeducible.SelectedValue))
                {
                    if (!String.IsNullOrEmpty(strRowFilter))
                        strRowFilter += " and ";
                    strRowFilter += "Deducible = '" + this.ddlDeducible.SelectedValue + "'";
                }
                if (!String.IsNullOrEmpty(this.ddlDropOutOption.SelectedValue))
                {
                    if (!String.IsNullOrEmpty(strRowFilter))
                        strRowFilter += " and ";
                    strRowFilter += "dropOptionID = '" + this.ddlDropOutOption.SelectedValue + "'";
                }
                if (!String.IsNullOrEmpty(this.ddlLocus.SelectedValue))
                {
                    if (!String.IsNullOrEmpty(strRowFilter))
                        strRowFilter += " and ";
                    strRowFilter += "LocusID = '" + this.ddlLocus.SelectedValue + "'";
                }

                // filter our data view
                if (!String.IsNullOrEmpty(strRowFilter))
                    dv.RowFilter = strRowFilter;

                // set source and display
                this.gvDropOut.DataSource = dv;
                this.gvDropOut.DataBind();
            }
        }
    }

    /// <summary>
    /// Refreshes the grid with an up-to-date search criteria based on the UI selections
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        PopulateDropOutGrid();
    }


    protected void gvDropOut_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvDropOut.PageIndex = e.NewPageIndex;
        PopulateDropOutGrid();
    }

    /// <summary>
    /// Saves the drop out data, if any changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        SaveDropOutData();
    }

    public void SaveDropOutData()
    {
        if (Session["EditLabKitID"] != null)
        {
            // go through the grid view rows
            for (int i = 0; i < gvDropOut.Rows.Count; i++)
            {
                // find our hidden fields for old values
                string strDropoutRateID = ((Label)gvDropOut.Rows[i].FindControl("lblDropoutRateID")).Text;
                string strDropOutRateNew = ((TextBox)gvDropOut.Rows[i].FindControl("txtDropOutRate")).Text;
                string strDropOutRateOld = ((Label)gvDropOut.Rows[i].FindControl("lblOldDropOutRate")).Text;

                // if any changed, save the rate
                if (strDropOutRateNew != strDropOutRateOld)
                {
                    string message = bi.UpdateDropOutRates(Session["EditLabKitID"].ToString(), strDropoutRateID, strDropOutRateNew);
                    Log.Info(Context.User.Identity.Name, Request.FilePath, Session, "Edited Drop Out Rate", "LabKit:" + Session["EditLabKitID"].ToString() + ",ID:" + strDropoutRateID + ",OldValue:" + strDropOutRateOld + ",NewValue:" + strDropOutRateNew);
                }

            }
        }
    }
    #endregion

    #region Drop-in rate editing
    /// <summary>
    /// Populates drop-ins for this lab kit from the db
    /// </summary>
    public void PopulateDropInGrid()
    {
        if(!string.IsNullOrEmpty(Session["EditLabKitID"].ToString()))
        {
            DataTable dt = bi.GetDropInData(Session["EditLabKitID"].ToString());
            this.gvDropIn.DataSource = dt;
            this.gvDropIn.DataBind();
        }
    }

    protected void btnSaveDropIns_Click(object sender, EventArgs e)
    {
        SaveDropInData();
    }

    public void SaveDropInData()
    {
        // go throught the grid view rows
        for (int i = 0; i < gvDropIn.Rows.Count; i++)
        {
            // find our hidden fields for old values
            string strID = ((Label)gvDropIn.Rows[i].FindControl("lblID")).Text;
            string strDropInRateNew = ((TextBox)gvDropIn.Rows[i].FindControl("txtDropInRate")).Text;
            string strDropInRateOld = ((Label)gvDropIn.Rows[i].FindControl("lblOldDropInRate")).Text;
            if (strDropInRateNew != strDropInRateOld)
            {
                // if any changed, save the rate
                string message = bi.UpdateDropInRates(strID, strDropInRateNew, Session["EditLabKitID"].ToString());
                Log.Info(Context.User.Identity.Name, Request.FilePath, Session, "Edited Drop In Rate", "LabKit:" + Session["EditLabKitID"].ToString() + ",ID:" + strID + ",OldValue:" + strDropInRateOld + ",NewValue:" + strDropInRateNew);
            }

        }

        // refresh our grid
        PopulateDropInGrid();
    }
    #endregion
}
