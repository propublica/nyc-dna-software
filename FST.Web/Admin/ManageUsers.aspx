<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" Inherits="Forensic.Admin.Admin_ManagingUsers" Title="Admin - Manage Users" Codebehind="ManageUsers.aspx.cs" %>
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
        <div class="bluetextBold">User Management</div>
       <p></p>
       <div style="text-align:left" class="bluetextBold">
           <a href="CreateUser.aspx">Create New User</a>: creates new user, add his/her profile and roles.
        </div>
        <p></p>
       <b class="bluetextBold">- Total created users: <asp:Literal runat="server" ID="lblTotUsers" /><br />
       - Users online now: <asp:Literal runat="server" ID="lblOnlineUsers" /></b>
       <asp:GridView ID="gvwUsers" runat="server" AutoGenerateColumns="false" DataKeyNames="UserName"
            OnRowDeleting="gvwUsers_RowDeleting"  OnRowCreated="gvwUsers_RowCreated"  
            AllowPaging="True" AllowSorting="True" 
            OnPageIndexChanging="gvwUsers_PageIndexChanging" OnSorting="gvwUsers_Sorting" 
            EnableViewState="False" BorderWidth="1px" BorderColor="#B7BABC"  BorderStyle="Solid" >
            <RowStyle CssClass="datagridRow1" />
            <SelectedRowStyle CssClass="comboBg" />
            <HeaderStyle Height="20" CssClass="datagridRowHead" HorizontalAlign="Center" />
            <EditRowStyle CssClass="comboBg" />
            <AlternatingRowStyle CssClass="datagridRow2" />
          <Columns>
             <asp:BoundField HeaderText="UserName" DataField="UserName" />
             <asp:HyperLinkField HeaderText="E-mail" DataTextField="Email" DataNavigateUrlFormatString="mailto:{0}" DataNavigateUrlFields="Email" />
             <asp:BoundField HeaderText="Created" DataField="CreationDate" DataFormatString="{0:MM/dd/yy h:mm tt}" />
             <asp:BoundField HeaderText="Last Activity" DataField="LastActivityDate" DataFormatString="{0:MM/dd/yy h:mm tt}" />
             <asp:CheckBoxField HeaderText="Approved" DataField="IsApproved" >
                 <ItemStyle HorizontalAlign="Center" />
                 <HeaderStyle HorizontalAlign="Center" />
             </asp:CheckBoxField>
             <asp:HyperLinkField Text="&lt;img src='../images/edit.gif' border='0' /&gt;" DataNavigateUrlFormatString="EditUser.aspx?UserName={0}" DataNavigateUrlFields="UserName" />
             <asp:ButtonField  CommandName="Delete" ButtonType="Image" ImageUrl="~/images/delete.gif" />
          </Columns>
          <EmptyDataTemplate><b>No users found for the specified criteria</b></EmptyDataTemplate>
           <PagerSettings FirstPageImageUrl="~/Admin/Images/First.gif" LastPageImageUrl="~/Admin/Images/Last.gif"
               Mode="NextPreviousFirstLast" NextPageImageUrl="~/Admin/Images/Next.gif" PreviousPageImageUrl="~/Admin/Images/Prev.gif" />
           <PagerStyle HorizontalAlign="Center" />
       </asp:GridView>
       <asp:Label ID="lblInfo1" runat="server" CssClass="bluetextBold" Text="Click one of the following link to display all users whose name begins with that letter:" />
       <p></p>
       <asp:Repeater runat="server" ID="rptAlphabet" OnItemCommand="rptAlphabet_ItemCommand">
          <ItemTemplate><asp:LinkButton ID="LinkButton3" runat="server" CssClass="bluetextBold" Text='<%# Container.DataItem %>'
             CommandArgument='<%# Container.DataItem %>' />&nbsp;&nbsp;
          </ItemTemplate>
       </asp:Repeater>
       <p></p>
       <asp:Label ID="lblInfo2" runat="server" CssClass="bluetextBold" Text="Otherwise use the controls below to search users by partial username or e-mail:" />
       <p></p>
       <div style="margin-top:3px; margin-left:300px; text-align:center; float:left; ">
           <asp:DropDownList runat="server" ID="ddlSearchTypes" CssClass="comboBg">
              <asp:ListItem Text="UserName" Selected="true" />
              <asp:ListItem Text="E-mail" />
           </asp:DropDownList> 
           <asp:Label ID="lblInfo3" runat="server" CssClass="bluetextBold" Text="contains" /> 
           <asp:TextBox runat="server" ID="txtSearchText" CssClass="textboxBg" /> 
       </div>
       <asp:ImageButton runat="server" ID="btnSearch" Text="Search" OnClick="btnSearch_Click" ImageUrl="~/Images/Buttons/Search1.png" OnClientClick="this.src='../Images/Buttons/Search2.png';" style="margin-left:-300px;" />
       <p></p>
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
<script type="text/javascript">
    var images = new Array();
    function preload() {
        for (i = 0; i < preload.arguments.length; i++) {
            images[i] = new Image();
            images[i].src = preload.arguments[i];
        }
    }
    preload(
	    "../Images/Buttons/Search2.png"
    );
</script>
</asp:Content>

