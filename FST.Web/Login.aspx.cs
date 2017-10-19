using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using FST.Common;
using System.Web.Security;
using System.DirectoryServices;

namespace FST.Web
{
    public partial class Login : System.Web.UI.Page
    {
        // used to set the base path for images on the page (so pages and controls in sub-folders can use the appropriate path)
        public string ImagePath { get; set; }
        // this is used to show the FST version at the top of the page
        public string FST_VERSION { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            // if we haven't picked an FST version, we get the one from the config file, match it with the one in the database, set it on match, error on mismatch
            if (Session["FST_VERSION"] == null)
            {
                try
                {
                    string appConfigVersion = ConfigurationManager.AppSettings.Get("FST_VERSION");
                    string databaseVersion = string.Empty;
                    try
                    {
                        FST.Common.Database db = new FST.Common.Database();
                        databaseVersion = db.getVersion();
                    }
                    catch
                    {
                        Response.Write("Database conenction failed or internal application permissions were insufficient. Please notify the person responsible for this application.");
                        Response.End();
                        return;
                    }

                    // we have a version mismatch, so print an error
                    if (appConfigVersion != databaseVersion)
                    {
                        Response.Write("Current database version and application version do not match. This may be a configuration or deployment issue. Please notify the person responsible for this application.");
                        Response.End();
                        return;
                    }

                    Session["FST_VERSION"] = FST_VERSION = appConfigVersion;
                }
                catch
                {

                }
            }
            else
            {
                FST_VERSION = Convert.ToString(Session["FST_VERSION"]);
            }

            try
            {
                // generate the proper path to the root URL. this actually comes out different on PWS vs IIS, so be careful about changing this.
                ImagePath = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath;
                ImagePath += (ImagePath.LastIndexOf('/') == ImagePath.Length - 1 ? string.Empty : "/");
            }
            catch
            {

            }

            if (!IsPostBack)
            {
                lblConfirmation.Visible = false;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
           // string domainName = ConfigurationManager.AppSettings["DomainName"];
            // string UserName = domainName+"\\"+txbUserName.Text;
            string UserName = txbUserName.Text;
           // if (IsAuthenticated(UserName, txbPassWord.Text))
           // {
           //     FormsAuthentication.SetAuthCookie(UserName, true);
            //    Response.Redirect("~/frmDefault.aspx");

                //No need to do database validation because they want to validate user in LDAP (Windows Server).
                if (Membership.ValidateUser(UserName, txbPassWord.Text) == true)
                {
                    FormsAuthentication.SetAuthCookie(UserName, true);
                    Response.Redirect("~/frmDefault.aspx");
                }
           // }
            else
            {
                lblConfirmation.Text = "Login failed. UserName or PassWord is not correct";
                lblConfirmation.Visible = true;
            }
        }

        //srvr = ldap server, e.g. LDAP://domain.com
        //usr = user name
        //pwd = user password
        public bool IsAuthenticated(string usr, string pwd)
        {
            bool authenticated = false;

            try
            {
                string srvr = ConfigurationManager.ConnectionStrings["ADConnectionString"].ConnectionString;
                DirectoryEntry entry = new DirectoryEntry(srvr, usr, pwd);
                object nativeObject = entry.NativeObject;
                authenticated = true;
            }
            catch (Exception ex)
            {
                authenticated = false;
            }

            return authenticated;
        }
    }
}