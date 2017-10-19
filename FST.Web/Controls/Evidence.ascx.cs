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
    public partial class Evidence : System.Web.UI.UserControl
    {
        FST.Common.Business_Interface bi = new FST.Common.Business_Interface();

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CreateLocusControls();
            BindDropDowns();
            AddDropDownJavaScripts();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Alleles = new EvidenceReplicates(this);
            }
        }

        private void CreateLocusControls()
        {
            DataTable dtLocus = bi.GetLocus(Guid.Parse(Session["LabKitID"].ToString()));
            int rows = dtLocus.Rows.Count / 5;
            if (dtLocus.Rows.Count % 5 > 0) rows++;

            List<Table> tables = GetTables();

            int replicate = 1;
            foreach (Table table in tables)
            {
                for (int i = 0; i < rows; i++)
                {
                    TableRow tr = new TableRow();
                    tr.Height = 46;
                    for (int j = 0; j < 5 && i * 5 + j < dtLocus.Rows.Count; j++)
                    {
                        TableCell td = new TableCell();
                        td.Style.Add("width", "170");
                        td.Style.Add("height", "48");
                        td.Style.Add("background-image", "images/combo_bg.gif");

                        EvidenceLocus locus = (EvidenceLocus)LoadControl("~/Controls/EvidenceLocus.ascx");
                        locus.Attributes.Add("style", "width: 170; height:48;");
                        string locusName = dtLocus.Rows[i * 5 + j]["FieldName"].ToString();
                        locus.ID = "Rep" + replicate + "_" + locusName;
                        locus.LocusName.Text = locusName;
                        td.Controls.Add(locus);
                        tr.Controls.Add(td);

                        if (j != 4)
                        {
                            TableCell tdSpacer = new TableCell();
                            tdSpacer.Width = 18;
                            tdSpacer.Controls.Add(new LiteralControl("&nbsp;"));
                            tr.Controls.Add(tdSpacer);
                        }
                    }

                    table.Rows.Add(tr);

                    if (i != rows - 1)
                    {
                        TableRow trSpacer = new TableRow();
                        TableCell td = new TableCell();
                        td.ColumnSpan = 9;
                        td.Height = 8;
                        td.Controls.Add(new LiteralControl("<img src=\"images/spacer.gif\" width=\"1\" height=\"4\" />"));
                        trSpacer.Controls.Add(td);
                        table.Controls.Add(trSpacer);
                    }
                }
                replicate++;
            }
        }

        private List<Table> GetTables()
        {
            List<Table> val = new List<Table>();
            val.Add(tblRep1);
            val.Add(tblRep2);
            val.Add(tblRep3);
            return val;
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

        public Dictionary<string, Dictionary<int, string>> GetEvidenceDictionary()
        {
            alleles = alleles ?? new EvidenceReplicates(this);
            Dictionary<string, Dictionary<int, string>> val = new Dictionary<string, Dictionary<int, string>>();

            DataTable dtLoci = bi.GetLocus(Guid.Parse(Session["LabKitID"].ToString()));

            foreach (DataRow dr in dtLoci.Rows)
            {
                string locus = dr["FieldName"].ToString().ToUpper();

                val.Add(locus, new Dictionary<int, string>());

                // 3 replicates
                for (int i = 1; i <= 3; i++)
                {
                     val[locus].Add(i, Alleles[i][locus]);
                }
            }

            return val;
        }

        internal void SetEvidenceDictionary(Dictionary<string, Dictionary<int, string>> dictionary)
        {
            if (dictionary == null) return;

            alleles = alleles ?? new EvidenceReplicates(this);

            DataTable dtLoci = bi.GetLocus(Guid.Parse(Session["LabKitID"].ToString()));

            foreach (DataRow dr in dtLoci.Rows)
            {
                string locus = dr["FieldName"].ToString().ToUpper();
                // 3 replicates
                for (int i = 1; i <= 3; i++)
                {
                    Alleles[i][locus] = dictionary[locus][i];
                }
            }
        }

        private void AddDropDownJavaScripts()
        {
            List<DropDownList> ddlControls = new List<DropDownList>();
            GetDropDownsList(ddlControls);

            foreach (DropDownList ddlControl in ddlControls)
            {
                StringBuilder sbScript = new StringBuilder();

                sbScript.Append("javascript:");
                EvidenceLocus parent = ((EvidenceLocus)ddlControl.Parent);
                string ddID = ddlControl.ClientID;
                // get text ID (when we do this from DB, we will generate them together, so we will set it in the same place... much better)
                string txtID = parent.Alleles.ClientID;
                sbScript.Append("var txt = document.getElementById('" + txtID + "');");
                sbScript.Append("var dd = document.getElementById('" + ddID + "');");
                sbScript.Append("if(dd.value == 'INC') txt.value = '';");
                sbScript.Append("else if(dd.value == 'NEG') txt.value = 'NEG';");
                sbScript.Append("else if(txt.value != '' && txt.value != 'NEG') txt.value += ',' + dd.value;");
                sbScript.Append("else txt.value = dd.value;");

                ddlControl.Attributes.Add("onchange", sbScript.ToString());
            }
        }

        private void GetTextBoxesList(List<TextBox> txtControls)
        {
            DataTable dtLoci = bi.GetLocus(Guid.Parse(Session["LabKitID"].ToString()));

            foreach (DataRow dr in dtLoci.Rows)
            {
                string locus = dr["FieldName"].ToString();

                int replicate = 1;
                foreach (Table table in GetTables())
                {
                    string controlName = "Rep" + replicate.ToString() + "_" + locus;
                    EvidenceLocus el = (EvidenceLocus)table.FindControl(controlName);
                    txtControls.Add(el.Alleles);
                    replicate++;
                }
            }
        }

        private void GetDropDownsList(List<DropDownList> ddlControls)
        {
            DataTable dtLoci = bi.GetLocus(Guid.Parse(Session["LabKitID"].ToString()));

            foreach (DataRow dr in dtLoci.Rows)
            {
                string locus = dr["FieldName"].ToString();

                int replicate = 1;
                foreach (Table table in GetTables())
                {
                    string controlName = "Rep" + replicate.ToString() + "_" + locus;
                    EvidenceLocus el = (EvidenceLocus)table.FindControl(controlName);
                    ddlControls.Add(el.DdAllele);
                    replicate++;
                }
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
                DataRow drNeg = dtValues.NewRow();
                drNeg["FiledName"] = "NEG";
                drNeg["FiledValue"] = "NEG";
                dtValues.Rows.Add(drNeg);

                List<DropDownList> ddlControls = new List<DropDownList>();

                int replicate = 1;
                foreach (Table table in GetTables())
                {
                    string controlName = "Rep" + replicate.ToString() + "_" + locus;
                    EvidenceLocus el = (EvidenceLocus)table.FindControl(controlName);
                    ddlControls.Add(el.DdAllele);
                    replicate++;
                }

                foreach (DropDownList ddlControl in ddlControls)
                {
                    ddlControl.DataSource = dtValues;
                    ddlControl.DataTextField = "FiledName";
                    ddlControl.DataValueField = "FiledValue";
                    ddlControl.DataBind();
                }
            }
        }

        protected void Menu1_MenuItemClick(object sender, MenuEventArgs e)
        {
            if (IsPostBack)
            {
                int rep = Int32.Parse(e.Item.Value);
                viewReplicates.ActiveViewIndex = rep;

                for (int i = 0; i < this.Menu1.Items.Count; i++)
                {
                    if (i == Int32.Parse(e.Item.Value))
                        Menu1.Items[i].ImageUrl = "~/Images/rep" + ((int)i + 1).ToString() + "_on.gif";
                    else
                        Menu1.Items[i].ImageUrl = "~/Images/rep" + ((int)i + 1).ToString() + "_off.gif";
                }
            }
        }

        private EvidenceReplicates alleles;
        public EvidenceReplicates Alleles
        {
            get { return alleles; }
            set { alleles = value; }
        }

        public class EvidenceReplicates
        {
            public EvidenceReplicates(UserControl parent)
            {
                replicate1 = new ReplicateAlleles(parent, 1);
                replicate2 = new ReplicateAlleles(parent, 2);
                replicate3 = new ReplicateAlleles(parent, 3);
            }

            ReplicateAlleles replicate1, replicate2, replicate3;

            public ReplicateAlleles this[int replicate]
            {
                get
                {
                    if (replicate == 1) return replicate1;
                    if (replicate == 2) return replicate2;
                    if (replicate == 3) return replicate3;
                    return null;
                }
            }

            public class ReplicateAlleles
            {
                UserControl Parent;
                int ReplicateID;
                public ReplicateAlleles(UserControl parent, int replicateID)
                {
                    Parent = parent;
                    ReplicateID = replicateID;
                }

                public string this[string locus]
                {
                    get
                    {
                        string controlName = "Rep" + ReplicateID + "_" + locus;
                        return ((EvidenceLocus)Parent.FindControl(controlName)).Alleles.Text;
                    }
                    set
                    {
                        string controlName = "Rep" + ReplicateID + "_" + locus;
                        ((EvidenceLocus)Parent.FindControl(controlName)).Alleles.Text = value;
                    }
                }
            }
        }
    }
}