using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using FST.Common;

namespace FST.Web
{
    public partial class frmDefault : System.Web.UI.Page
    {
        /// <summary>
        /// This is the page Load event of the Home page with the default values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //gets all the Case Types
                FST.Common.Business_Interface bi = new FST.Common.Business_Interface();
                DataTable dt = bi.GetCaseTypes();
                this.ddlCase_Type.DataSource = dt;
                this.ddlCase_Type.DataTextField = "FieldName";
                this.ddlCase_Type.DataValueField = "FieldValue";
                this.ddlCase_Type.DataBind();
                this.ddlCase_Type.SelectedIndex = 0;
                this.ddlCase_Type_SelectedIndexChanged(ddlCase_Type, new EventArgs());

                //populates the Theta
                dt.Clear();
                dt = bi.GetTheta();
                this.ddlTheta.DataSource = dt;
                this.ddlTheta.DataTextField = "FieldName";
                this.ddlTheta.DataValueField = "FieldValue";
                this.ddlTheta.DataBind();
                this.ddlTheta.SelectedIndex = 2;

                // get the lab kits
                dt.Clear();
                dt = bi.GetLabKits();
                this.ddlLabKit.DataSource = dt;
                this.ddlLabKit.DataTextField = "FieldName";
                this.ddlLabKit.DataValueField = "FieldValue";
                this.ddlLabKit.DataBind();
                int cnt = 0;
                if (Session["LabKitID"] != null)
                    this.ddlLabKit.SelectedValue = Session["LabKitID"].ToString();
                else
                    this.ddlLabKit.SelectedIndex = 0;
                Session["LabKitID"] = this.ddlLabKit.SelectedValue;
                Session["LabKitName"] = this.ddlLabKit.SelectedItem.Text;

                this.ddlDegradedType.SelectedIndex = 0;

                // show the appropriate panels for the first comparison in the list
                this.PnlCaseTypes.Visible = true;
                this.pnlSuspPrfl1.Visible = true;
                this.pnlSuspPrfl2.Visible = false;
                this.pnlKnownPrfl1.Visible = false;
                this.pnlKnownPrfl2.Visible = false;
                this.pnlKnownPrfl3.Visible = false;
                this.pnlKnownPrfl4.Visible = false;
                this.btnGo.Visible = true;
                this.btnBulk.Visible = true;
                this.lblDegradedType.Visible = true;
                this.ddlDegradedType.Visible = true;
                this.pnlUserWarning.Visible = false;
            }
        }

        public void ddlKabKit_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Session["LabKitID"] = ddlLabKit.SelectedValue;
        }

        ComparisonData comparisonData = null;

        /// <summary>
        /// This function traps the SelectedIndexChanged event of the combobox Case Type
        /// and changes the screen's layout accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ddlCase_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            // we always show this
            btnGo.Visible = true;

            // this can be done better, use comparisondata num/den configs to do this..
            int compareMethodID = int.Parse(this.ddlCase_Type.SelectedValue.ToString());
            comparisonData = new ComparisonData(compareMethodID);

            // comparisons are only in numerator, so we check the numerator comparison profile count
            this.pnlSuspPrfl1.Visible = comparisonData.NumeratorProfiles.ComparisonCount >= 1;
            this.pnlSuspPrfl2.Visible = comparisonData.NumeratorProfiles.ComparisonCount >= 2;
            this.pnlSuspPrfl3.Visible = comparisonData.NumeratorProfiles.ComparisonCount >= 3;
            this.pnlSuspPrfl4.Visible = comparisonData.NumeratorProfiles.ComparisonCount >= 4;

            // the knowns are in the numerator and in the denominator, although almost all of the comparisons
            // at the time of writing have either the same number or more knowns in the numerator than in the
            // denominator. doesn't hurt to future-proof though.
            this.pnlKnownPrfl1.Visible = comparisonData.NumeratorProfiles.KnownCount >= 1 || comparisonData.DenominatorProfiles.KnownCount >= 1;
            this.pnlKnownPrfl2.Visible = comparisonData.NumeratorProfiles.KnownCount >= 2 || comparisonData.DenominatorProfiles.KnownCount >= 2;
            this.pnlKnownPrfl3.Visible = comparisonData.NumeratorProfiles.KnownCount >= 3 || comparisonData.DenominatorProfiles.KnownCount >= 3;
            this.pnlKnownPrfl4.Visible = comparisonData.NumeratorProfiles.KnownCount >= 4 || comparisonData.DenominatorProfiles.KnownCount >= 4;

            // we have no bulk comparison scenarios where there are more than two comparison profiles
            this.btnBulk.Visible = comparisonData.NumeratorProfiles.ComparisonCount == 1;

            Session["ComparisonData"] = comparisonData;
        }


        /// <summary>
        /// This function represents a button click event which directs to the respective Compare screens based on certain values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnGo_Click(object sender, EventArgs e)
        {
            bool isBulk = false;
            GoCompare(isBulk);
        }

        /// <summary>
        /// This function represents a button click event which directs to the respective Bulk Compare screens based on certain values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBulk_Click(object sender, EventArgs e)
        {
            bool isBulk = true;
            GoCompare(isBulk);
        }

        private void GoCompare(bool isBulk)
        {
            comparisonData = (ComparisonData) Session["ComparisonData"];

            // get our username from session, version from config file
            comparisonData.UserName = Context.User.Identity.Name;
            comparisonData.Version = Session["FST_VERSION"].ToString();

            comparisonData.Theta = float.Parse(this.ddlTheta.SelectedItem.ToString());

            switch (this.ddlDegradedType.SelectedValue.ToString())
            {
                default:
                case "ND":
                    comparisonData.Degradation = ComparisonData.enDegradation.None;
                    break;
                case "MD":
                    comparisonData.Degradation = ComparisonData.enDegradation.Mild;
                    break;
                case "SD":
                    comparisonData.Degradation = ComparisonData.enDegradation.Mild;
                    break;
            }

            // get our names from the UI
            comparisonData.Comparison1Name = this.txtSuspPrfl1_Nm.Text.Trim();
            comparisonData.Comparison2Name = this.txtSuspPrfl2_Nm.Text.Trim();
            comparisonData.Comparison3Name = this.txtSuspPrfl3_Nm.Text.Trim();
            comparisonData.Comparison4Name = this.txtSuspPrfl4_Nm.Text.Trim();
            comparisonData.Known1Name = this.txtKnownPrfl1_Nm.Text.Trim();
            comparisonData.Known2Name = this.txtKnownPrfl2_Nm.Text.Trim();
            comparisonData.Known3Name = this.txtKnownPrfl3_Nm.Text.Trim();
            comparisonData.Known4Name = this.txtKnownPrfl4_Nm.Text.Trim();

            comparisonData.Bulk = isBulk;
            comparisonData.LabKitID = Guid.Parse(this.ddlLabKit.SelectedValue);
            comparisonData.LabKitName = this.ddlLabKit.SelectedItem.Text;   // this gets printed on the reports

            Session["LabKitID"] = comparisonData.LabKitID;
            Session["ComparisonData"] = comparisonData;
            Session["Processed"] = "Y"; // wtf is this?

            Response.Redirect("frmUpload.aspx");
        }
    }
}