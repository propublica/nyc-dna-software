using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FST.Web.Controls
{
    public partial class EvidenceLocus : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        public TextBox Alleles { get { return this.txtAlleles ?? (txtAlleles = new TextBox()); } }
        public Label LocusName { get { return this.lblLocusName ?? (lblLocusName = new Label()); } }
        public DropDownList DdAllele { get { return this.ddAlleles ?? (ddAlleles = new DropDownList()); } }

    }
}