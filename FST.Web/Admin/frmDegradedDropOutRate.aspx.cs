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

public partial class frmDegradedDropOutRate : System.Web.UI.Page
{
    FST.Common.Business_Interface bi = new FST.Common.Business_Interface();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
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
            this.ddlDegradationType.Text = "";


            dt.Clear();
            dt = bi.GetDegradedDropOutData();
            this.gvDropOut.DataSource = dt;
            this.gvDropOut.DataBind();
        }
    }

    public void PopulateGrid()
    {
        DataTable dt = bi.GetDegradedDropOutData();
		  using (DataView dv = new DataView(dt))
		  {
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
			  if (!String.IsNullOrEmpty(this.ddlDegradationType.SelectedValue))
			  {
				  if (!String.IsNullOrEmpty(strRowFilter))
					  strRowFilter += " and ";
				  strRowFilter += "Degradation_Type = '" + this.ddlDegradationType.SelectedValue + "'";
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

			  if (!String.IsNullOrEmpty(strRowFilter))
				  dv.RowFilter = strRowFilter;

			  this.gvDropOut.DataSource = dv;
			  this.gvDropOut.DataBind();
		  }
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        PopulateGrid();
    }
    protected void gvDropOut_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvDropOut.PageIndex = e.NewPageIndex;
        PopulateGrid();
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        SaveData();
    }

    public void SaveData()
    {
        for (int i = 0; i < gvDropOut.Rows.Count; i++)
        {
            string strDropoutRateID = ((Label)gvDropOut.Rows[i].FindControl("lblDropoutRateID")).Text;
            string strDropOutRateNew = ((TextBox)gvDropOut.Rows[i].FindControl("txtDropOutRate")).Text;
            string strDropOutRateOld = ((Label)gvDropOut.Rows[i].FindControl("lblOldDropOutRate")).Text;
            if (strDropOutRateNew != strDropOutRateOld)
            {
                string message = bi.UpdateDegradedDropOutRates(strDropoutRateID, strDropOutRateNew);
            }

        }
    }
}
