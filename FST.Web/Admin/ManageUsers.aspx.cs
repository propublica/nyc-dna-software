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
using System.Globalization;

namespace Forensic.Admin
{
	public partial class Admin_ManagingUsers : System.Web.UI.Page
	{
		//private MembershipUserCollection allUsers = Membership.GetAllUsers();
		private MembershipProvider localProvider = Membership.Providers["ss_MembershipProvider"];
		private MembershipUserCollection allUsers = Membership.GetAllUsers();
		//static int iCount;
		//private MembershipUserCollection allUsers = Membership.Providers["ss_MembershipProvider"].GetAllUsers(0,100,out iCount);

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.IsPostBack)
			{
                BindUsers(true);
				lblTotUsers.Text = allUsers.Count.ToString(CultureInfo.CurrentCulture);
				lblOnlineUsers.Text = Membership.GetNumberOfUsersOnline().ToString(CultureInfo.CurrentCulture);
				//lblOnlineUsers.Text = Membership.Providers["ss_MembershipProvider"].GetNumberOfUsersOnline().ToString();

				string[] alphabet = "A;B;C;D;E;F;G;H;I;J;K;L;M;N;O;P;Q;R;S;T;U;V;W;X;Y;Z;All".Split(';');
				rptAlphabet.DataSource = alphabet;
				rptAlphabet.DataBind();
			}
		}

		private void BindUsers(bool reloadAllUsers)
		{
			if (reloadAllUsers)
				allUsers = Membership.GetAllUsers();

			MembershipUserCollection users = null;

			string searchText = "";
			if (!String.IsNullOrEmpty(gvwUsers.Attributes["SearchText"]))
				searchText = gvwUsers.Attributes["SearchText"];

			bool searchByEmail = false;
			if (!String.IsNullOrEmpty(gvwUsers.Attributes["SearchByEmail"]))
				searchByEmail = bool.Parse(gvwUsers.Attributes["SearchByEmail"]);

			if (searchText.Length > 0)
			{
				if (searchByEmail)
					users = Membership.FindUsersByEmail(searchText);
				else
					users = Membership.FindUsersByName(searchText);
			}
			else
			{
				users = allUsers;
			}

			gvwUsers.DataSource = users;
			gvwUsers.DataBind();
		}

		protected void rptAlphabet_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			gvwUsers.Attributes.Add("SearchByEmail", true.ToString());
			if (e.CommandArgument.ToString().Length == 1)
			{
				gvwUsers.Attributes.Add("SearchText", e.CommandArgument.ToString() + "%");
				BindUsers(false);
			}
			else
			{
				gvwUsers.Attributes.Add("SearchText", "");
				BindUsers(false);
			}

		}

		private string ConvertSortDirectionToSql(SortDirection sortDireciton)
		{
			string newSortDirection = String.Empty;

			switch (sortDireciton)
			{
				case SortDirection.Ascending:
					newSortDirection = "ASC";
					break;

				case SortDirection.Descending:
					newSortDirection = "DESC";
					break;
			}

			return newSortDirection;
		}

		protected void gvwUsers_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			gvwUsers.PageIndex = e.NewPageIndex;
			gvwUsers.Attributes.Add("SearchText", "");
			BindUsers(false);
		}

		protected void gvwUsers_Sorting(object sender, GridViewSortEventArgs e)
		{
			DataTable dataTable = gvwUsers.DataSource as DataTable;

			if (dataTable != null)
			{
				using (DataView dataView = new DataView(dataTable))
				{
					dataView.Sort = e.SortExpression + " " + ConvertSortDirectionToSql(e.SortDirection);

					gvwUsers.DataSource = dataView;
					gvwUsers.DataBind();
				}
			}
		}

		protected void gvwUsers_RowCreated(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				ImageButton btn = e.Row.Cells[6].Controls[0] as ImageButton;
				btn.OnClientClick = "return confirm('Are you sure you want to delete this user account?');";
			}
		}

		protected void gvwUsers_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			string userName = gvwUsers.DataKeys[e.RowIndex].Value.ToString();
			ProfileManager.DeleteProfile(userName);
			Membership.DeleteUser(userName);
			BindUsers(true);
			lblTotUsers.Text = allUsers.Count.ToString(CultureInfo.CurrentCulture);
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			bool searchByEmail = (ddlSearchTypes.SelectedValue == "E-mail");
			gvwUsers.Attributes.Add("SearchText", "%" + txtSearchText.Text + "%");
			gvwUsers.Attributes.Add("SearchByEmail", searchByEmail.ToString(CultureInfo.CurrentCulture));
			BindUsers(false);
		}

	}
}
