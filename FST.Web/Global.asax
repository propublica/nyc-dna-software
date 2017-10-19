<%@ Application Language="C#" %>

<script runat="server">
    /// <summary>
    /// Global exception handling code
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs
        Exception ex = Server.GetLastError();
        // The original error may have been wrapped in a HttpUnhandledException, 
        // so we need to log the details of the InnerException. 
        ex = ex.InnerException ?? ex;
        try
        {
            FST.Common.Log.Crash(Context.User.Identity.Name, Request.FilePath, Session, null, ex);
            Server.ClearError();
        }
        catch
        {
        }
        Response.Redirect("~/ErrorPage.aspx", false); 
    }
</script>
