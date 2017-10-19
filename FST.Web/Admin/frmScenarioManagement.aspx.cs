using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using FST.Common;

public partial class Admin_frmScenarioManagement : System.Web.UI.Page
{
    Business_Interface bi = new Business_Interface();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            DataTable dt = bi.GetCaseTypesTable();
            gvCases.DataSource = dt;
            gvCases.DataBind();
        }
    }

    protected void btnOn_Click(object sender, EventArgs e)
    {
        ActivateDeactivateScenario(true);
    }

    protected void btnOff_Click(object sender, EventArgs e)
    {
        ActivateDeactivateScenario(false);
    }

    private void ActivateDeactivateScenario(bool enabled)
    {
        List<string> toActivate = new List<string>();
        bool isChecked = false;
        string curID = string.Empty;

        foreach (GridViewRow gvr in gvCases.Rows)
            foreach (Control c in gvr.Controls)
            {
                foreach (Control child in c.Controls)
                {
                    if (child.ID == "chkActivateDeactivate" && (child as CheckBox).Checked)
                    {
                        isChecked = true;
                    }
                    if (child.ID == "lblID" && isChecked)
                    {
                        curID = (child as Label).Text;
                    }
                    if (curID != string.Empty && isChecked && !toActivate.Contains(curID))
                    {
                        toActivate.Add(curID);
                        curID = string.Empty;
                        isChecked = false;
                    }
                }
            }
        foreach (string keyID in toActivate)
            bi.UpdateCaseTypeStatusByID(keyID, enabled);

        DataTable dt = bi.GetCaseTypesTable();
        gvCases.DataSource = dt;
        gvCases.DataBind();
    }
}