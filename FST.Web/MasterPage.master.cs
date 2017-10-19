using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Globalization;

public partial class MasterPage : System.Web.UI.MasterPage
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
                catch (Exception ex)
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
        else FST_VERSION = Convert.ToString(Session["FST_VERSION"]);

        try
        {
            // generate the proper path to the root URL. this actually comes out different on PWS vs IIS, so be careful about changing this.
            ImagePath = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath;
            ImagePath += (ImagePath.LastIndexOf('/') == ImagePath.Length - 1 ? string.Empty : "/");

            if (!Page.IsPostBack)
            {
                // Show User Name
                if (Page.User.Identity.Name.ToString(CultureInfo.CurrentCulture) != null)
                {
                    lblWellcome.Visible = lblUserName.Visible = true;
                    lblUserName.Text = Page.User.Identity.Name.ToString(CultureInfo.CurrentCulture);
                }
                else
                {
                    lblWellcome.Visible = lblUserName.Visible = false;
                }

                #region Set Menu roles from session variables

                bool blCheckUserInAdminRole = Roles.IsUserInRole("Admin");

                if (blCheckUserInAdminRole)
                {
                    Menu1.UserRoles.Add("ADMIN");
                    Menu1.UserRoles.Add("STANDARDS");
                    Menu1.UserRoles.Add("EDIT LAB KITS");
                    Menu1.UserRoles.Add("EDIT DEGRADED DROPOUT");
                    Menu1.UserRoles.Add("EDIT DROPIN");
                    Menu1.UserRoles.Add("EDIT FREQUENCY");
                    Menu1.UserRoles.Add("EDIT LAB TYPES");
                    Menu1.UserRoles.Add("EDIT POPULATION");
                    Menu1.UserRoles.Add("CREATE USER");
                    Menu1.UserRoles.Add("MANAGE USERS");
                    Menu1.UserRoles.Add("VIEW DROPOUT RATES");
                    Menu1.UserRoles.Add("VIEW SUBMITTED JOBS STATUS");
                    Menu1.UserRoles.Add("MANAGE SCENARIOS");
                }

                #endregion

                #region Bind XML Data in Menu Control
                if (Menu1.DataSource == null)
                {
                    // if we're in the root directory, use the root map. if we're up one, map accordingly. 
                    if(Server.MapPath("~").ToString() == Server.MapPath(".").ToString())
                        Menu1.DataSource = Server.MapPath("~/XmlMenu.xml");
                    else
                        Menu1.DataSource = Server.MapPath("~/XmlMenuOneUp.xml");

                    Menu1.DataBind();
                }
                #endregion
            }
        }
        catch(Exception ex)
        {
            #region redirect to Login Page
            Response.Redirect("login.aspx");
            #endregion
        }

        #region CLEAR CACHE
        Response.Cache.SetNoStore();
        Response.AddHeader("Cache-control", "no-store, must-revalidate, private,no-cache");
        Response.AddHeader("Pragma", "no-cache");
        Response.AddHeader("Expires", "0");
        #endregion
          
    }

    public void Login_Authenticate(object sender, System.Web.UI.WebControls.AuthenticateEventArgs e)
    {
        Login login = (Login)sender;
        string loginUsername = login.UserName;
        string loginPassword = login.Password;

        bool isADUser = Membership.Providers["ADMembershipProvider"].ValidateUser(loginUsername, loginPassword);
        //bool isADUser = true;
        //loginUsername = "jmernin";
        if (isADUser)
        {
            string LocalPass = "submittals123";
            bool isSSUser = Membership.Providers["FSP_MembershipProvider"].ValidateUser(loginUsername, LocalPass);
            if (isSSUser)
                e.Authenticated = true;
            else
                e.Authenticated = false;
        }
        else
            e.Authenticated = false;
    }
    
}
