<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="frmTestCenter.aspx.cs" Inherits="FST.Web.Admin.frmTestCenter" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentHolder" runat="server">
    <table style="width: 100%; font-family:Verdana; font-size:10px;">
        <tr>
            <td style="width: 30%; text-align: right;">
                Test Name:
            </td>
            <td style="width: 68%; text-align: left;">
                <asp:TextBox ID="txtTestName" runat="server" Width="200"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="width: 30%; text-align: right;">
                Type of Test:
            </td>
            <td style="width: 68%; text-align: left;">
                <asp:CheckBox ID="chkIndividual" runat="server" Text="Individual" />
                <asp:CheckBox ID="chkBulk" runat="server" Text="Bulk" />
            </td>
        </tr>
        <tr>
            <td style="width: 30%; text-align: right;">
                Comparison Types:
            </td>
            <td style="width: 68%; text-align: left;">
                <asp:CheckBoxList ID="chkComparisonTypes" runat="server" DataValueField="Case_Id" DataTextField="Case_Desc"></asp:CheckBoxList>
            </td>
        </tr>
        <tr>
            <td style="width: 30%; text-align: right;">
                File Upload or Manual Entry:
            </td>
            <td style="width: 68%; text-align: left;">
                <asp:RadioButton ID="rbDeducible" runat="server" GroupName="rbDeducible" Checked="true" Text="Deducible" />
                <asp:RadioButton ID="rbNonDeducible" runat="server" GroupName="rbDeducible" Text="Non-Deducible" />
            </td>
        </tr>
        <tr>
            <td style="width: 30%; text-align: right;">
                File Upload or Manual Entry:
            </td>
            <td style="width: 68%; text-align: left;">
                <asp:RadioButton ID="rbManualEntry" runat="server" GroupName="rbFileUploadOrManualEntry" Checked="true" Text="Manual Entry" />
                <asp:RadioButton ID="rbFileUpload" runat="server" GroupName="rbFileUploadOrManualEntry" Text="File Upload" />
            </td>
        </tr>
        <tr>
            <td style="width: 30%; text-align: right;">
                Evidence:
            </td>
            <td style="width: 68%; text-align: left;">
                <asp:FileUpload ID="fuEvidence" runat="server" />
            </td>
        </tr>    
        <tr>
            <td style="width: 30%; text-align: right;">
                Comaprison Profiles:
            </td>
            <td style="width: 68%; text-align: left;">
                <asp:FileUpload ID="fuComparisonProfiles" runat="server" />
            </td>
        </tr>    
        <tr>
            <td style="width: 30%; text-align: right;">
                Known 1 Profile:
            </td>
            <td style="width: 68%; text-align: left;">
                <asp:FileUpload ID="fuKnown1Profile" runat="server" />
            </td>
        </tr>
        <tr>
            <td style="width: 30%; text-align: right;">
                Known 2 Profile:
            </td>
            <td style="width: 68%; text-align: left;">
                <asp:FileUpload ID="fuKnown2Profile" runat="server" />
            </td>
        </tr>
        <tr>
            <td style="width: 30%; text-align: right;">
                Known 3 Profile:
            </td>
            <td style="width: 68%; text-align: left;">
                <asp:FileUpload ID="fuKnown3Profile" runat="server" />
            </td>
        </tr>        
        <tr>
            <td style="width: 30%; text-align: right;">
                <asp:Button ID="btnRunTestNew" runat="server" Text="Run NEW Test" OnClick="btnRunTestNew_Click" />
            </td>
            <td style="width: 68%; text-align: left;">
                <asp:Button ID="btnRunTest" runat="server" Text="Run OLD Test" OnClick="btnRunTest_Click" />
            </td>
        </tr>  
    </table>
    <asp:ScriptManager runat="server" ID="ScriptManager2" />
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
      <ContentTemplate>
        <asp:Timer runat="server" ID="Timer1" 
          OnTick="Timer1_Tick" Interval="3600" />
        <asp:Literal runat="server" ID="Literal1" />
      </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
