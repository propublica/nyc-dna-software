<%@ Control Language="C#" AutoEventWireup="true" Inherits="Forensic.Controls.UserProfile" Codebehind="UserProfile.ascx.cs" %>
<div class="bluetextBold">Personal details</div>
<p></p>
<table cellpadding="2">
   <tr>
      <td style="width: 110px;" class="gridnormal"><asp:Label runat="server" ID="lblFirstName" AssociatedControlID="txtFirstName" Text="First Name:" /></td>
      <td style="width: 400px;"><asp:TextBox ID="txtFirstName" runat="server" CssClass="textboxBg" Width="99%"></asp:TextBox></td>
   </tr>
   <tr>
      <td class="gridnormal"><asp:Label runat="server" ID="lblLastName" AssociatedControlID="txtLastName" Text="Last Name:" /></td>
      <td><asp:TextBox ID="txtLastName" runat="server" CssClass="textboxBg" Width="99%"></asp:TextBox></td>
   </tr>
   <tr>
      <td class="gridnormal"><asp:Label runat="server" ID="lblDisplayName" AssociatedControlID="txtDisplayName" Text="Display Name:" /></td>
      <td><asp:TextBox ID="txtDisplayName" runat="server" CssClass="textboxBg" Width="99%"></asp:TextBox></td>
   </tr>
   <tr>
      <td class="gridnormal"><asp:Label runat="server" ID="lblIsAdmin" Text="Is Admin:" /></td>
      <td align="left"><asp:CheckBox runat="server" ID="chkIsAdmin" /></td>                                                                
   </tr>
 
</table>
