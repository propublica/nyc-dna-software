<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" Inherits="Forensic.Admin.Admin_EditUser" Title="Admin - Edit User" Codebehind="EditUser.aspx.cs" %>
<%@ Register Src="../Controls/UserProfile.ascx" TagName="UserProfile" TagPrefix="mb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentHolder" Runat="Server">
<br />
 <table width="100%" height="600" border="0" cellspacing="0" cellpadding="0">
    <tr>
    <td width="10"></td>
      <td bgcolor="#FFFFFF" align="center"><table width="99%" height="600" border="0" cellspacing="1" cellpadding="0" bgcolor="#CCCCCC">
          <tr>
            <td><table width="100%" height="600" border="0" cellspacing="0" cellpadding="0">
                <tr>
                  <td bgcolor="#FFFFFF" valign="top" >
         <!-------------------------------Content Here------------------->	
    <div class="maincontent" align="center">
        <div class="bluetextBold">General User Information</div>
       <p></p>
       <table cellpadding="2">
          <tr>
             <td style="width: 110px;" class="gridnormal">UserName:</td>
             <td style="width: 300px;" class="gridnormal"><asp:Literal runat="server" ID="lblUserName"/></td>
          </tr>
          <tr>
             <td class="gridnormal">E-mail:</td>
             <td class="gridnormal"><asp:HyperLink runat="server" ID="lnkEmail" /></td>
          </tr>
          <tr>
             <td class="gridnormal">Created:</td>
             <td class="gridnormal"><asp:Literal runat="server" ID="lblRegistered" /></td>
          </tr>
          <tr>
             <td class="gridnormal">Last Login:</td>
             <td class="gridnormal"><asp:Literal runat="server" ID="lblLastLogin" /></td>
          </tr>
          <tr>
             <td class="gridnormal">Last Activity:</td>
             <td class="gridnormal"><asp:Literal runat="server" ID="lblLastActivity" /></td>
          </tr>
          <tr>
             <td class="gridnormal"><asp:Label runat="server" ID="lblOnlineNow" AssociatedControlID="chkOnlineNow" Text="Online Now:" /></td>
             <td class="gridnormal"><asp:CheckBox runat="server" ID="chkOnlineNow" Enabled="false" /></td>
          </tr>
          <tr>
             <td class="gridnormal"><asp:Label runat="server" ID="lblApproved" AssociatedControlID="chkApproved" Text="Approved:" /></td>
             <td class="gridnormal"><asp:CheckBox runat="server" ID="chkApproved" AutoPostBack="true" OnCheckedChanged="chkApproved_CheckedChanged" /></td>
          </tr>
          <tr>
             <td class="gridnormal"><asp:Label runat="server" ID="lblLockedOut" AssociatedControlID="chkLockedOut" Text="Locked Out:" /></td>
             <td class="gridnormal"><asp:CheckBox runat="server" ID="chkLockedOut" AutoPostBack="true" OnCheckedChanged="chkLockedOut_CheckedChanged" /></td>
          </tr>
       </table>
       <p></p>
       <div class="bluetextBold">Edit User's Roles</div>
       <p></p>
       <asp:CheckBoxList runat="server" ID="chklRoles" CssClass="gridnormal" RepeatColumns="5" CellSpacing="4" />
       <table cellpadding="2" style="width: 450px;">
          <tr><td style="text-align: right;">
             <asp:Label runat="server" ID="lblRolesFeedbackOK" SkinID="FeedbackOK" Text="Roles updated successfully" Visible="false" />
             <asp:Button runat="server" ID="btnUpdateRoles" Text="Update" OnClick="btnUpdateRoles_Click" />
          </td></tr>
       </table>
       <p></p>
       <div class="bluetextBold">Edit User's Profile</div>
       <p></p>
       <mb:UserProfile ID="UserProfile1" runat="server" />
          <table cellpadding="2" style="width: 450px;">
          <tr><td style="text-align: right;">
             <asp:Label runat="server" ID="lblProfileFeedbackOK" SkinID="FeedbackOK" Text="Profile updated successfully" Visible="false" />
             <asp:Button runat="server" ID="btnUpdateProfile" ValidationGroup="EditProfile" Text="Update" OnClick="btnUpdateProfile_Click" />
          </td></tr>
       </table>
    </div>
    <!-------------------------------Content End------------------->                 
		 </td>
                </tr>
                <tr>
                  <td bgcolor="#FFFFFF" >&nbsp;</td>
                </tr>
              </table></td>
          </tr>
        </table></td>
        <td width="10"></td>
    </tr>         
  </table>
</asp:Content>

