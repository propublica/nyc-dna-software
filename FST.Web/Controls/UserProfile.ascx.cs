using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Profile;

namespace Forensic.Controls
{
    public partial class UserProfile : System.Web.UI.UserControl
    {
        ProfileBase Profile = new ProfileBase();
        private string _userName = "";
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            this.Page.RegisterRequiresControlState(this);
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                // if the UserName property contains an emtpy string, retrieve the profile
                // for the current user, otherwise for the specified user
                ProfileBase profile = this.Profile;
                if (this.UserName.Length > 0)
                    profile = ProfileBase.Create(this.UserName);

                try { txtFirstName.Text = profile.GetPropertyValue("FirstName").ToString(); } catch { }
                try { txtLastName.Text = profile.GetPropertyValue("LastName").ToString(); } catch { }
                try { txtDisplayName.Text = profile.GetPropertyValue("DisplayName").ToString(); } catch { }
            }
            else
            {
                if (String.IsNullOrEmpty(this.UserName))
                {
                    txtFirstName.Text = "";
                    txtLastName.Text = "";
                    txtDisplayName.Text = "";
                }
            }
        }

        public void SaveProfile()
        {
            // if the UserName property contains an emtpy string, save the current user's profile,
            // othwerwise save the profile for the specified user
            //ProfileCommon profile = this.Profile;
            //if (this.UserName.Length > 0)
            //    profile = this.Profile.GetProfile(this.UserName);

            //profile.FirstName = txtFirstName.Text;
            //profile.LastName = txtLastName.Text;
            //profile.DisplayName = txtDisplayName.Text;
            ////profile.Discipline = ddlDiscipline.Text;
            //profile.Save();
            //if (this.chkIsAdmin.Checked)
            //    Roles.AddUserToRole(this.UserName, "Admin");
        }
    }
}