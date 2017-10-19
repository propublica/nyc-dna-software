using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using FST.Common;

public partial class frmEditFrequency : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            FST.Common.Business_Interface bi = new FST.Common.Business_Interface();

            // load the races (or ethnicities) into the page
            DataTable dt = bi.GetRaces();
            this.ddlRace.DataSource = dt;
            DataRow row = dt.NewRow();
            row["FieldName"] = DBNull.Value;
            row["FieldValue"] = DBNull.Value;
            dt.Rows.Add(row);
            this.ddlRace.DataTextField = "FieldName";
            this.ddlRace.DataValueField = "FieldValue";
            this.ddlRace.DataBind();
            this.ddlRace.Text = "";
            dt.Clear();

            // get the loci (for all lab kits)
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
            dt.Clear();

            // get the alleles for the selected locus
            dt = bi.GetAlleles(this.ddlLocus.SelectedValue);
            this.ddlAlleleNo.DataSource = dt;
            row = dt.NewRow();
            row["FieldName"] = DBNull.Value;
            row["FieldValue"] = DBNull.Value;
            dt.Rows.Add(row);
            this.ddlAlleleNo.DataTextField = "FieldName";
            this.ddlAlleleNo.DataValueField = "FieldValue";
            this.ddlAlleleNo.DataBind();
            this.ddlAlleleNo.Text = "";
            dt.Clear();

            // get the frequencies
            dt = bi.GetFrequencyData();
            this.gvFreqView.DataSource = dt;
            this.gvFreqView.DataBind();
        }
    }

    /// <summary>
    /// Applies sort and search to the grid
    /// </summary>
    public void PopulateGrid()
    {
        FST.Common.Business_Interface bi = new FST.Common.Business_Interface();
        // get all the frequencies
        DataTable dt = bi.GetFrequencyData();

		using (DataView dv = new DataView(dt))
		{
            // apply race filter
			string strRowFilter = "";
			if (!String.IsNullOrEmpty(this.ddlRace.SelectedValue))
				strRowFilter = "EthnicID = '" + this.ddlRace.SelectedValue + "'";
            
            // apply locus filter
			if (!String.IsNullOrEmpty(this.ddlLocus.SelectedValue))
			{
				if (!String.IsNullOrEmpty(strRowFilter))
					strRowFilter += " and ";
				strRowFilter += "LocusID = '" + this.ddlLocus.SelectedValue + "'";
			}

            // apply allele filter
			if (!String.IsNullOrEmpty(this.ddlAlleleNo.SelectedValue))
			{
				if (!String.IsNullOrEmpty(strRowFilter))
					strRowFilter += " and ";
				strRowFilter += "AlleleID = '" + this.ddlAlleleNo.SelectedValue + "'";
			}

            // apply all filtes to data view
			if (!String.IsNullOrEmpty(strRowFilter))
				dv.RowFilter = strRowFilter;

            // set data view source for gridview and databind
			this.gvFreqView.DataSource = dv;
			this.gvFreqView.DataBind();
		}
    }

    /// <summary>
    /// Handles the locus changing so we display the correct alleles for it
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlLocus_SelectedIndexChanged(object sender, EventArgs e)
    {
        FST.Common.Business_Interface bi = new FST.Common.Business_Interface();
        // if the locus is blank, don't show an alleles filter
		if (String.IsNullOrEmpty(this.ddlLocus.SelectedValue))
        {
            this.ddlAlleleNo.Items.Clear();
            this.ddlAlleleNo.Items.Add("");
            this.ddlAlleleNo.SelectedIndex = 0;
        }
        else
        {   // otherwise, if a locus is selected
            // get the alleles for that locus
            DataTable dt = bi.GetAlleles(this.ddlLocus.SelectedValue);
            this.ddlAlleleNo.DataSource = dt;
            // add a blank row
            DataRow row = dt.NewRow();
            row["FieldName"] = DBNull.Value;
            row["FieldValue"] = DBNull.Value;
            dt.Rows.Add(row);
            // and bind it to the allele drop down
            this.ddlAlleleNo.DataTextField = "FieldName";
            this.ddlAlleleNo.DataValueField = "FieldValue";
            this.ddlAlleleNo.DataBind();
            this.ddlAlleleNo.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// Handles the search button click event, populates the grid based on the selected filters
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        PopulateGrid();
    }

    /// <summary>
    /// Handles pagination
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvFreqView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvFreqView.PageIndex = e.NewPageIndex;
        PopulateGrid();
    }

    /// <summary>
    /// If the save button is clicked, save the data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        SaveData();
    }

    /// <summary>
    /// Saves the updated frequency to the database
    /// </summary>
    public void SaveData()
    {
        // go through each of the rows
        for (int i = 0; i < gvFreqView.Rows.Count; i++)
        {
            // get the ID, current value, and old value
            string strFreq_Id = ((Label)gvFreqView.Rows[i].FindControl("lblFrequency_ID")).Text;
            string strFreqNew = ((TextBox)gvFreqView.Rows[i].FindControl("txtFrequency")).Text;
            string strFreqOld = ((Label)gvFreqView.Rows[i].FindControl("lblOldFrequency")).Text;

            // if we have a difference between the old value and the current value
            if (strFreqNew != strFreqOld)
            {
                // update the frequency in the DB
                FST.Common.Business_Interface bi = new FST.Common.Business_Interface();
                string message = bi.UpdateFrequency(strFreq_Id, strFreqNew);
                // log the change
                Log.Info(Context.User.Identity.Name, Request.FilePath, Session, "Edited Frequency Rate", "ID:" + strFreq_Id + ",OldValue:" + strFreqOld + ",NewValue:" + strFreqNew);
            }                    

        }
    }
}
