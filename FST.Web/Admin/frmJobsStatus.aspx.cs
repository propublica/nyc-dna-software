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

public partial class Admin_frmJobsStatus : System.Web.UI.Page
{
    FST.Common.Business_Interface bi = new FST.Common.Business_Interface();

	private int caseCount = 0;
	private string caseStatus = "";

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
            // loading counters for the different case statuses
			caseStatus = "N";
			caseCount = bi.GetCaseCounterByStatus(caseStatus);

            // get the cases by status (we default to the new jobs page)
			SetGVJobStatus(caseStatus);
		}
	}

    /// <summary>
    /// Gets the jobs (cases) from DB by status and binds them to the main gridview
    /// </summary>
    /// <param name="caseStatus"></param>
	public void SetGVJobStatus(string caseStatus)
	{
		DataTable dt = bi.GetSubmittedJobsInfo(caseStatus);
		this.gvJobStatus.ShowHeader = true;
		this.gvJobStatus.DataSource = null;
		this.gvJobStatus.DataSource = dt;
		this.gvJobStatus.DataBind();

	}
    
    /// <summary>
    /// Fires when a new status radio button gets selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
	protected void rbtnlStatus_SelectedIndexChanged(object sender, EventArgs e)
	{
        // we retrieve and display the cases based on which button was selected
		DisplaySubmittedCaseInfoPage();
	}

    /// <summary>
    /// Fires when the refresh button is clicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
	protected void imgbtnRefresh_Click(object sender, EventArgs e)
	{
        // we retireve and display the cases
		DisplaySubmittedCaseInfoPage();
	}

    /// <summary>
    /// Retrieves and displays cases based on which radio button is selected in the UI
    /// </summary>
	private void DisplaySubmittedCaseInfoPage()
	{
		caseCount = 0;

        // find which radio button is selected in the UI and change the status accordingly
		switch (rbtnlStatus.SelectedValue)
		{
			case "To Be Processed": caseStatus = "N"; break;
			case "Processing": caseStatus = "P"; break;
			case "Processed": caseStatus = "Y"; break;
			case "Removed": caseStatus = "D"; break;

		}

        // get the counters from DB
		caseCount = bi.GetCaseCounterByStatus(caseStatus);
        // get and display the jobs by status
		SetGVJobStatus(caseStatus);
	}

    /// <summary>
    /// Removes a case from the list
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
	protected void btnRemove_Click(object sender, EventArgs e)
	{
        // find out which row we are deleting
		ImageButton btnRemove = (ImageButton)sender;
		GridViewRow gvRow = (GridViewRow)btnRemove.Parent.Parent;

        // find the job ID
		int lastCol = gvRow.Cells.Count - 1;
		string recordID = gvRow.Cells[lastCol].Text.Replace("<nobr>", "");
		recordID = recordID.Replace("</nobr>", "");

        // get the case status from the session
		caseStatus = Session["caseStatus"] != null ? Session["caseStatus"].ToString() : "N";

        // set the status to deleted
		bi.UpdateCaseByRecordID(recordID, "D");
        
        // retrieve the cases and display them on the page
		DataTable dt = bi.GetSubmittedJobsInfo(caseStatus);
		this.gvJobStatus.ShowHeader = true;
		this.gvJobStatus.DataSource = null;
		this.gvJobStatus.DataSource = dt;
		this.gvJobStatus.DataBind();
	}

    /// <summary>
    /// Restarts the job on the row which contains the button that has been clicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnRestartJob_Click(object sender, EventArgs e)
	{
        // find out which row the button was on
        ImageButton btnRestartJob = (ImageButton)sender;
        GridViewRow gvRow = (GridViewRow)btnRestartJob.Parent.Parent;

        // find the job ID
		int lastCol = gvRow.Cells.Count - 1;
		string recordID = gvRow.Cells[lastCol].Text.Replace("<nobr>", "");
		recordID = recordID.Replace("</nobr>", "");

        // get the current status the page is displaying 
		caseStatus = Session["caseStatus"].ToString();

        // set the case to new again
		bi.UpdateCaseByRecordID(recordID, "N");
        // get the jobs from the database, and upload them to the database
		DataTable dt = bi.GetSubmittedJobsInfo(caseStatus);
		this.gvJobStatus.ShowHeader = true;
		this.gvJobStatus.DataSource = null;
		this.gvJobStatus.DataSource = dt;
		this.gvJobStatus.DataBind();
	}

    /// <summary>
    /// Handle paging...
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
	protected void gvJobStatus_PageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		this.gvJobStatus.PageIndex = e.NewPageIndex;
		caseStatus = Session["caseStatus"].ToString();
		SetGVJobStatus(caseStatus);
	}

    /// <summary>
    /// Makes sure that the text-wrapping is disabled in each cell.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
	protected void gvJobStatus_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		//no wrapped..
		foreach (TableCell cell in e.Row.Cells)
		{
			if (cell.Text.Length > 0)
				cell.Text = "<nobr>" + cell.Text + "</nobr>";
		}
	}


}
