<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" Inherits="Forensic.Admin.Admin_CreateUser" Title="Create New User" Codebehind="CreateUser.aspx.cs" %>
<%@ Register Src="~/Controls/UserProfile.ascx" TagName="UserProfile" TagPrefix="FSTOCME" %>

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
    <div class="maincontent" align="center" width="100%">
        <asp:CreateUserWizard runat="server" ID="CreateUserWizard1" AutoGeneratePassword="False"  
                ContinueDestinationPageUrl="~/Admin/CreateUser.aspx" OnFinishButtonClick="CreateUserWizard1_FinishButtonClick" OnCreatedUser="CreateUserWizard1_CreatedUser" LoginCreatedUser="false" ContinueButtonText="Create Another User"
                 CreateUserButtonType="Image" CreateUserButtonImageUrl="~/Images/Buttons/Create1.png">
            <WizardSteps>
                 <asp:CreateUserWizardStep runat="server">
                    <ContentTemplate>
                   <br />
                    <div class="bluetextBold" align="center">Create new user account</div>
                    <p></p>
                    
                    <table cellpadding="2" width="100%" border="0">
                       <tr>
                          <td style="width: 84px;" class="gridnormal"><asp:Label runat="server" ID="lblUserName" AssociatedControlID="UserName" Text="Username:" /></td>
                          <td style="width: 300px;" align="left"><asp:TextBox runat="server" ID="UserName" CssClass="textboxBg" Width="90%" />&nbsp;
                              <asp:ImageButton ID="btnCheckUser" runat="server" ImageUrl="~/Images/CHECK.GIF" 
                                  onclick="btnCheckUser_Click" />
                           </td>
                          <td>
                             <asp:RequiredFieldValidator ID="valRequireUserName" runat="server" ControlToValidate="UserName" SetFocusOnError="true" Display="Dynamic"
                                ErrorMessage="Username is required." ToolTip="Username is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                          </td>            
                       </tr>               
                      
                       <tr>
                          <td style="width: 84px;" class="gridnormal"><asp:Label runat="server" ID="lblPassword" AssociatedControlID="Password" Text="Password:" /></td>
                          <td style="width: 300px;" align="left"><asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="textboxBg" Width="90%" /></td>
                          <td>
                             <asp:RequiredFieldValidator ID="valRequirePassword" runat="server" ControlToValidate="Password" SetFocusOnError="true" Display="Dynamic"
                                ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                             <asp:RegularExpressionValidator ID="valPasswordLength" runat="server" ControlToValidate="Password" SetFocusOnError="true" Display="Dynamic"
                                ValidationExpression="\w{5,}" ErrorMessage="Password must be at least 5 characters long." ToolTip="Password must be at least 5 characters long."
                                ValidationGroup="CreateUserWizard1">*</asp:RegularExpressionValidator>
                          </td>            
                       </tr>
                       <tr>
                          <td style="width: 84px;" class="gridnormal"><asp:Label runat="server" ID="lblConfirmPassword" AssociatedControlID="ConfirmPassword" Text="Confirm password:" /></td>
                          <td style="width: 300px;" align="left"><asp:TextBox runat="server" ID="ConfirmPassword" TextMode="Password" CssClass="textboxBg" Width="90%" /></td>
                          <td>
                             <asp:RequiredFieldValidator ID="valRequireConfirmPassword" runat="server" ControlToValidate="ConfirmPassword" SetFocusOnError="true" Display="Dynamic"
                                ErrorMessage="Confirm Password is required." ToolTip="Confirm Password is required."
                                ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                             <asp:CompareValidator ID="valComparePasswords" runat="server" ControlToCompare="Password" SetFocusOnError="true"
                                ControlToValidate="ConfirmPassword" Display="Dynamic" ErrorMessage="The Password and Confirmation Password must match."
                                ValidationGroup="CreateUserWizard1">*</asp:CompareValidator>
                          </td>            
                       </tr>
                       
                       <tr>
                          <td style="width: 84px;" class="gridnormal"><asp:Label runat="server" ID="lblEmail" AssociatedControlID="Email" Text="E-mail:" /></td>
                          <td style="width: 300px;" align="left"><asp:TextBox runat="server" ID="Email" CssClass="textboxBg" Width="90%" /></td>
                          <td>
                             <asp:RequiredFieldValidator ID="valRequireEmail" runat="server" ControlToValidate="Email" SetFocusOnError="true" Display="Dynamic"
                                ErrorMessage="E-mail is required." ToolTip="E-mail is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                             <asp:RegularExpressionValidator runat="server" ID="valEmailPattern"  Display="Dynamic" SetFocusOnError="true" ValidationGroup="CreateUserWizard1"
                                ControlToValidate="Email" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ErrorMessage="The e-mail address you specified is not well-formed.">*</asp:RegularExpressionValidator>
                          </td>            
                       </tr>
                       <tr>
                          <td colspan="3" style="text-align: right; height: 23px;">
                             <asp:Label ID="ErrorMessage" SkinID="FeedbackKO" runat="server" EnableViewState="False"></asp:Label>
                          </td>
                       </tr>
                    </table>
                    <asp:ValidationSummary ValidationGroup="CreateUserWizard1" ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" />
                    </ContentTemplate>
                 </asp:CreateUserWizardStep>
                 <asp:WizardStep runat="server" Title="Set preferences">
                    <div class="sectiontitle">Set-up your profile</div>
                    <p></p>
                    Please enter the following information
                    <p></p>
                    <FSTOCME:UserProfile ID="UserProfile1" runat="server" />
                 </asp:WizardStep>
                 <asp:CompleteWizardStep runat="server"></asp:CompleteWizardStep>
              </WizardSteps>

              <StepNavigationTemplate>
                <asp:ImageButton ID="ImageButton1" CommandName="MoveNext" runat="server" ImageUrl="../Images/Buttons/Create1.png" OnClientClick="this.src='../Images/Buttons/Create2.png';" />
              </StepNavigationTemplate>
              <FinishNavigationTemplate>
                <asp:ImageButton CommandName="MoveComplete" runat="server" ImageUrl="../Images/Buttons/Finish1.png" OnClientClick="this.src='../Images/Buttons/Finish2.png';" />
              </FinishNavigationTemplate>
        </asp:CreateUserWizard>
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
	    "Images/Buttons/Finish2.png",
        "Images/Buttons/Create2.png"
    );
</script>
</asp:Content>

