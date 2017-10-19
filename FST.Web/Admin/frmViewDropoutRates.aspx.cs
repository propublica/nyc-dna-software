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

public partial class frmViewDropoutRates : System.Web.UI.Page
{
    FST.Common.Business_Interface bi = new FST.Common.Business_Interface();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // load number of persons involved from DB and bind to gridview
            DataTable dt = bi.GetNoOfPersonsInvolved();
            this.ddlNoOfPersons.DataSource = dt;
            this.ddlNoOfPersons.DataTextField = "FieldName";
            this.ddlNoOfPersons.DataValueField = "FieldValue";
            this.ddlNoOfPersons.DataBind();
            this.ddlNoOfPersons.SelectedIndex = 0;
            dt.Clear();

            // load lab kits from db and bind to gridview
            dt = bi.GetLabKits();
            this.ddLabKit.DataSource = dt;
            this.ddLabKit.DataBind();
            string firstLabKitID = dt.Rows[0]["FieldValue"].ToString();
            dt.Clear();

            // load loci for the lab kit from DB and bind to gridview
            dt = bi.GetLocus(Guid.Parse(firstLabKitID));
            this.ddlLocus.DataSource = dt;
            DataRow row = dt.NewRow();
            row["FieldName"] = DBNull.Value;
            row["FieldValue"] = DBNull.Value;
            dt.Rows.Add(row);
            this.ddlLocus.DataTextField = "FieldName";
            this.ddlLocus.DataValueField = "FieldValue";
            this.ddlLocus.DataBind();
            this.ddlLocus.Text = "";
            dt.Clear();

            // set some defaults
            this.ddlDropOutType.Text = "";
            this.ddlDeducible.Text = Convert.ToString("Yes", CultureInfo.CurrentCulture);
				this.txtDropOutOption.Text = Convert.ToString("6.25 pg", CultureInfo.CurrentCulture);

            // get dropout rates for page defaults from DB and databind to gridview
            dt = ReadDropoutRate(this.txtDropOutOption.Text, Convert.ToInt16(this.ddlNoOfPersons.Text, CultureInfo.CurrentCulture), this.ddlDeducible.Text, firstLabKitID);
            this.gvDropOut.DataSource = dt;
            this.gvDropOut.DataBind();
        }
    }

    /// <summary>
    /// This method retrieves dropout rates from the database, and computes a rate based on the DNA evidence amount. If the DNA evidence amount happens to match one of the
    /// data points for which we have a measured dropout rate, then we use that rate exactly. If the rate is not matched, then we compute a rate based on a linear regression
    /// between the two nearest rates. There is also a special case where the DNA evidence amount may be a value between 100 and 101 picograms of DNA evidence. In this case,
    /// we use the 101 picogram dropout rate explicitly.
    /// </summary>
    /// <param name="dnaEvidenceAmount">DNA evidence amount entered in the UI in picograms.</param>
    /// <param name="NoOfPersonsInvolvd">The number of persons assumed to be involved based on this comparison scenario.</param>
    /// <param name="strDeducible">A string containing 'Yes' or 'No' based on whether the number of contributors to this evidence is deducible. Different dropout rates are selected.</param>
    /// <param name="labKitID">A GUID representing which Lab Kit is being used for this comparison. Different Lab Kits contain different dropout rates.</param>
    /// <returns>Data table with the dropout rates.</returns>
    protected DataTable ReadDropoutRate(string dropoutOption, int NoOfPersonsInvolvd, string strDeducible, string labKitID)
    {
        decimal tempDropout = 0.0m;
        string strDropOutOptionBegin = "";
        string strDropOutOptionEnd = "";
        // sometimes the users may copy a DNA evidence amount that includes the units. remove it, trim
        dropoutOption = dropoutOption.Replace("pg", "").Trim();
        tempDropout = Convert.ToDecimal(dropoutOption, CultureInfo.CurrentCulture);
        // cap the amount at 500 picograms
        if (tempDropout > 500m)
            tempDropout = 500m;
        // if the amount is between 100 and 101 picograms, use the 101 picogram rate.
		  if (100m < tempDropout && tempDropout < 101m)
			  tempDropout = 101m;

          // format the string to match the values in the DropoutOptions table in the database
        dropoutOption = tempDropout + " pg";

        // if our value matches any of the dropout rate points, then we use that exact rate. 
		if (tempDropout == 6.25m || tempDropout == 12.5m || tempDropout == 25m || tempDropout == 50m || tempDropout == 100m || tempDropout == 101m
            || tempDropout == 150m || tempDropout == 250m || tempDropout == 500m)
        {
            strDropOutOptionBegin = dropoutOption;
            strDropOutOptionEnd = dropoutOption;
        } 
        // otherwise, we pick a lower rate and an upper rate (strDropOutOptionBegin, strDropOutOptionEnd)
        else if (6.25m < tempDropout && tempDropout < 12.5m)
        {
            strDropOutOptionBegin = "6.25 pg";
            strDropOutOptionEnd = "12.5 pg";
            tempDropout = tempDropout - 6.25m;
        }
        else if (12.5m < tempDropout && tempDropout < 25m)
        {
            strDropOutOptionBegin = "12.5 pg";
            strDropOutOptionEnd = "25 pg";
            tempDropout = tempDropout - 12.5m;
        }
        else if (25m < tempDropout && tempDropout < 50m)
        {
            strDropOutOptionBegin = "25 pg";
            strDropOutOptionEnd = "50 pg";
            tempDropout = tempDropout - 25m;
        }
        else if (50m < tempDropout && tempDropout < 100m)
        {
            strDropOutOptionBegin = "50 pg";
            strDropOutOptionEnd = "100 pg";
            tempDropout = tempDropout - 50m;
        }
        else if (101m < tempDropout && tempDropout < 150m)
        {
            strDropOutOptionBegin = "101 pg";
            strDropOutOptionEnd = "150 pg";
            tempDropout = tempDropout - 101m;
        }
        else if (150m < tempDropout && tempDropout < 250m)
        {
            strDropOutOptionBegin = "150 pg";
            strDropOutOptionEnd = "250 pg";
            tempDropout = tempDropout - 150m;
        }
        else if (250m < tempDropout && tempDropout < 500m)
        {
            strDropOutOptionBegin = "250 pg";
            strDropOutOptionEnd = "500 pg";
            tempDropout = tempDropout - 250m;
        }

        // get the upper and lower dropout rate points from the database based on the number of people involved and whether the number of people is deducible from the evidence
        // NOTE: degradation is calculated in the database
        DataTable dt = bi.getDropOutRate(strDropOutOptionBegin, strDropOutOptionEnd, NoOfPersonsInvolvd, strDeducible, labKitID);
        // if our DNA evidence amount is exactly one of the dropout rate points then return the rates
        if (strDropOutOptionBegin == strDropOutOptionEnd)
        {
            dt.Columns.Remove("DropoutOptionID");
            dt.Columns.Remove("DropoutOption");
            return dt;
        }
        else
        {   // otherwise, calculate the rates by doing a linear regression around the two points for every locus. we iterate every two rows, and calculate the value for one row
            for (int i = 0; i < dt.Rows.Count; i = i + 2)
            {
                dt.Rows[i]["DropOutRate"] = Convert.ToDecimal(dt.Rows[i]["DropOutRate"].ToString(), CultureInfo.CurrentCulture) +
                    ((Convert.ToDecimal(dt.Rows[i + 1]["DropOutRate"].ToString(), CultureInfo.CurrentCulture) - Convert.ToDecimal(dt.Rows[i]["DropOutRate"].ToString(), CultureInfo.CurrentCulture)) /
                    (Convert.ToDecimal(dt.Rows[i + 1]["DropoutOption"].ToString(), CultureInfo.CurrentCulture) - Convert.ToDecimal(dt.Rows[i]["DropoutOption"].ToString(), CultureInfo.CurrentCulture))) * tempDropout;
            }
            // then we go delete every other row because that rate is no longer used. it was used only for the linear regression
            for (int i = dt.Rows.Count - 1; i >= 0; i = i - 2)
                dt.Rows.RemoveAt(i);

            dt.Columns.Remove("DropoutOptionID");
            dt.Columns.Remove("DropoutOption");
            return dt;

        }
    }

    /// <summary>
    /// Populates the main gridview with the sorted and filtered drop-out data
    /// </summary>
    public void PopulateGrid()
    {
        // get the drop-out data from the database
        DataTable dt = ReadDropoutRate(this.txtDropOutOption.Text, Convert.ToInt16(this.ddlNoOfPersons.Text, CultureInfo.CurrentCulture), this.ddlDeducible.Text, this.ddLabKit.SelectedValue);
        // sort through it
		using (DataView dv = new DataView(dt))
		{
            // apply filters
			string strRowFilter = "";
			if (!String.IsNullOrEmpty(this.ddlDropOutType.SelectedValue))
				strRowFilter = "Type = '" + this.ddlDropOutType.SelectedItem.Text + "'";

			if (!String.IsNullOrEmpty(this.ddlLocus.SelectedValue))
			{
				if (!String.IsNullOrEmpty(strRowFilter))
					strRowFilter += " and ";
				strRowFilter += "LocusName = '" + this.ddlLocus.SelectedItem.Text + "'";
			}

			if (!String.IsNullOrEmpty(strRowFilter))
				dv.RowFilter = strRowFilter;

            // databind to gridview
			this.gvDropOut.DataSource = dv;
			this.gvDropOut.DataBind();
		}
    }

    /// <summary>
    /// Populates teh gird
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        PopulateGrid();
    }

    /// <summary>
    /// Changes the page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvDropOut_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvDropOut.PageIndex = e.NewPageIndex;
        PopulateGrid();
    }
        
}
