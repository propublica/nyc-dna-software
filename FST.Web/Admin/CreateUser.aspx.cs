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
using System.DirectoryServices;
using System.Security.Principal;
using System.Security.Permissions;


namespace Forensic.Admin
{
	public partial class Admin_CreateUser : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			//if (!this.IsPostBack)
			//    this.CreateUserWizard1.DataBind();
		}

		protected void CreateUserWizard1_FinishButtonClick(object sender, WizardNavigationEventArgs e)
		{
			UserProfile1.UserName = CreateUserWizard1.UserName;
			UserProfile1.SaveProfile();
			//CreateUserWizard1.FinishDestinationPageUrl = "~/Admin/EditUser.aspx?UserName=" + CreateUserWizard1.UserName;
		}

		protected void CreateUserWizard1_CreatedUser(object sender, EventArgs e)
		{
			// add the current user to the Typist role
			UserProfile1.UserName = CreateUserWizard1.UserName;
			Roles.AddUserToRole(CreateUserWizard1.UserName, "guest");
		}

		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
		protected void btnCheckUser_Click(object sender, ImageClickEventArgs e)
		{
			using (DirectoryEntry entry = new DirectoryEntry())
			{
				string curName = this.CreateUserWizard1.UserName;
				if (curName.IndexOf('\\') > 0)
					curName = curName.Substring(curName.IndexOf('\\') + 1);
				entry.Username = curName;
				DirectorySearcher newUserSearch = new DirectorySearcher(entry) { Filter = ("(SAMAccountName=" + curName + ")") }; ;
				//newUserSearch.Filter="
				SearchResult result;
				try
				{
					result = newUserSearch.FindOne();
					if (null == result)
					{
						this.CreateUserWizard1.UserName = this.CreateUserWizard1.UserName + "  --  " + "Invalid User.";
					}
					else
					{
						this.CreateUserWizard1.UserName = WindowsIdentity.GetCurrent().Name;
						this.CreateUserWizard1.Email = result.Properties["mail"][0].ToString();
					}
					//result.Properties.
				}
				catch (Exception ex)
				{
					string strError = ex.Message;
				}
			}
		}
	}
}
