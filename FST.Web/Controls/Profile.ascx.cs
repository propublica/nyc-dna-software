using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;

namespace FST.Web.Controls
{
    public partial class Profile : System.Web.UI.UserControl
    {
        public int LociPerRow { get; set; }

        public Profile()
        {
            alleles = new ProfileAlleles(this);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CreateLocusControls();
            BindDropDowns();
            AddDropDownJavaScripts();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        private void CreateLocusControls()
        {
            int lociPerRow = LociPerRow;

            DataTable dtLocus = bi.GetLocus(Guid.Parse(Session["LabKitID"].ToString()));
            int rows = dtLocus.Rows.Count / lociPerRow;
            if (dtLocus.Rows.Count % lociPerRow > 0) rows++;

            for (int i = 0; i < rows; i++)
            {
                TableRow tr = new TableRow();
                tr.Height = 46;
                for (int j = 0; j < lociPerRow & i * lociPerRow + j < dtLocus.Rows.Count; j++)
                {
                    TableCell td = new TableCell();
                    td.Style.Add("width", "172px");
                    td.Style.Add("height", "47px");
                    td.Style.Add("background-image", "images/combo_bg.gif");

                    ProfileLocus locus = (ProfileLocus) LoadControl("~/Controls/ProfileLocus.ascx");
                    locus.Attributes.Add("style", "width: 170; height:48;");
                    string locusName = dtLocus.Rows[i * lociPerRow + j]["FieldName"].ToString();
                    locus.ID = locusName;
                    locus.LocusName.Text = locusName;
                    td.Controls.Add(locus);
                    tr.Controls.Add(td);

                    if (j != lociPerRow - 1)
                    {
                        TableCell tdSpacer = new TableCell();
                        tdSpacer.Width = 18;
                        tdSpacer.Controls.Add(new LiteralControl("&nbsp;"));
                        tr.Controls.Add(tdSpacer);
                    }
                }

                tblLoci.Rows.Add(tr);

                if (i != rows - 1)
                {
                    TableRow trSpacer = new TableRow();
                    TableCell td = new TableCell();
                    td.ColumnSpan = 9;
                    td.Height = 8;
                    td.Controls.Add(new LiteralControl("<img src=\"images/spacer.gif\" width=\"1\" height=\"4\" />"));
                    trSpacer.Controls.Add(td);
                    tblLoci.Controls.Add(trSpacer);
                }
            }
            
        }

        FST.Common.Business_Interface bi = new FST.Common.Business_Interface();

        private void AddDropDownJavaScripts()
        {
            List<DropDownList> ddlControls = new List<DropDownList>();
            GetDropDownsList(ddlControls);

            foreach (DropDownList ddlControl in ddlControls)
            {
                StringBuilder sbScript = new StringBuilder();
                ProfileLocus parent = ((ProfileLocus)ddlControl.Parent);
                sbScript.Append("javascript:");
                string ddID = ddlControl.ClientID;
                string ddIDOther = ddlControl == parent.DdAllele1 ? parent.DdAllele2.ClientID : parent.DdAllele1.ClientID;
                string txtID = parent.Alleles.ClientID;
                sbScript.Append("var txt = document.getElementById('" + txtID + "');");
                sbScript.Append("var dd = document.getElementById('" + ddID + "');");
                sbScript.Append("var ddOther = document.getElementById('" + ddIDOther + "');");
                //sbScript.Append("debug;");
                sbScript.Append("if(dd.value == 'INC' && ddOther.value == 'INC') txt.value = '';");
                sbScript.Append("if(dd.value == 'INC' && ddOther.value != 'INC') txt.value = ddOther.value;");
                sbScript.Append("if(dd.value != 'INC' && ddOther.value == 'INC') txt.value = dd.value;");
                sbScript.Append("if(dd.value != 'INC' && ddOther.value != 'INC') txt.value = parseInt(dd.value,10) < parseInt(ddOther.value) ? dd.value + ',' + ddOther.value : ddOther.value + ',' + dd.value;");
                
                ddlControl.Attributes.Add("onchange", sbScript.ToString());
            }
        }

        private void GetDropDownsList(List<DropDownList> ddlControls)
        {
            DataTable dtLoci = bi.GetLocus(Guid.Parse(Session["LabKitID"].ToString()));

            foreach (DataRow dr in dtLoci.Rows)
            {
                string locus = dr["FieldName"].ToString();

                ProfileLocus pl = (ProfileLocus) tblLoci.FindControl(locus);
                ddlControls.Add(pl.DdAllele1);
                ddlControls.Add(pl.DdAllele2);
            }
        }

        private void GetTextBoxesList(List<TextBox> txtControls)
        {
            DataTable dtLoci = bi.GetLocus(Guid.Parse(Session["LabKitID"].ToString()));

            foreach (DataRow dr in dtLoci.Rows)
            {
                string locus = dr["FieldName"].ToString();

                ProfileLocus pl = (ProfileLocus)tblLoci.FindControl(locus);
                txtControls.Add(pl.Alleles);
            }
        }

        private void BindDropDowns()
        {
            DataTable dtLoci = bi.GetLocus(Guid.Parse(Session["LabKitID"].ToString()));

            foreach (DataRow dr in dtLoci.Rows)
            {
                string locus = dr["FieldName"].ToString();
                DataTable dtValues = bi.getLocusAlleles(locus);

                // remove the zero allele which only exists to provide a default frequency value for an infrequent allele. 
                //the user can type this in manually as any allele that's not in the dropdown
                dtValues.Rows.Remove(dtValues.Select("FiledValue='0'")[0]);

                dtValues.Select("FiledName='INC'")[0]["FiledValue"] = "INC";
                // no NEG here

                List<DropDownList> ddlControls = new List<DropDownList>();

                ProfileLocus pl = (ProfileLocus) tblLoci.FindControl(locus);
                ddlControls.Add(pl.DdAllele1);
                ddlControls.Add(pl.DdAllele2);
                
                foreach (DropDownList ddlControl in ddlControls)
                {
                    ddlControl.DataSource = dtValues;
                    ddlControl.DataTextField = "FiledName";
                    ddlControl.DataValueField = "FiledValue";
                    ddlControl.DataBind();
                }
            }
        }

        public Dictionary<string, string> GetProfileDictionary()
        {
            Dictionary<string, string> val = new Dictionary<string, string>();

            DataTable dtLoci = bi.GetLocus(Guid.Parse(Session["LabKitID"].ToString()));

            foreach (DataRow dr in dtLoci.Rows)
            {
                string locus = dr["FieldName"].ToString().ToUpper();

                val.Add(locus, this.Alleles[locus]);
            }

            return val;
        }

        public void SetProfileDictionary(Dictionary<string, string> profile)
        {
            if (profile == null) return;
            DataTable dtLoci = bi.GetLocus(Guid.Parse(Session["LabKitID"].ToString()));

            foreach (DataRow dr in dtLoci.Rows)
            {
                string locus = dr["FieldName"].ToString().ToUpper();

                this.Alleles[locus] = profile[locus];
            }
        }

        public void Reset()
        {
            List<DropDownList> ddlControls = new List<DropDownList>();
            GetDropDownsList(ddlControls);

            foreach (DropDownList ddlControl in ddlControls)
                ddlControl.SelectedIndex = 0;

            List<TextBox> txtControls = new List<TextBox>();
            GetTextBoxesList(txtControls);

            foreach (TextBox txtControl in txtControls)
                txtControl.Text = string.Empty;

        }


        private ProfileAlleles alleles;
        public ProfileAlleles Alleles
        {
            get { return alleles; }
        }

        public class ProfileAlleles
        {
            UserControl Parent;
            public ProfileAlleles(UserControl parent)
            {
                Parent = parent;
            }
            public string this[string locus]
            {
                get
                {
                    string allele = ((ProfileLocus)Parent.FindControl(locus)).Alleles.Text;
                    if (!allele.Contains(",") && allele.Trim() != string.Empty) allele = allele + "," + allele;
                    return allele;
                }
                set
                {
                    ((ProfileLocus)Parent.FindControl(locus)).Alleles.Text = value;
                }
            }
        }
    }
}