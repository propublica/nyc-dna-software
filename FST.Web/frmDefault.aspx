<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="frmDefault.aspx.cs" Inherits="FST.Web.frmDefault" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentHolder" runat="server">
    <table width="100%" border="0" height="100%" align="center" cellpadding="0" cellspacing="0">
        <tr>
            <td height="27" align="left" valign="top" bgcolor="#FFFFFF">
                <ul id="crumbs">
                    <%--<li><a href="Admin/Admin.aspx">Admin</a></li>--%>
                    <li>Home</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td height="600" valign="top">
                <!-------------------------------Content Here------------------->
                <table cellspacing="0" cellpadding="0" border="0" width="80%" align="center">
                    <tr>
                        <td align="left" valign="top">
                            <asp:Panel ID="PnlCaseTypes" runat="server" Visible="true">
                                <span class="bluetextBold">Scenarios:</span>
                                <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" style="border: 1px solid #aab3b3;">
                                    <tr>
                                        <td>
                                            <table width="100%" border="0" cellpadding="0" cellspacing="0" bgcolor="#FFFFFF">
                                                <tr>
                                                    <td valign="top">
                                                        <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                                            <tr>
                                                                <td align="right">
                                                                    <asp:Label ID="lblCase_Type" runat="server" CssClass="graynormalText" Text="Select Scenario: "
                                                                        Visible="true"></asp:Label>
                                                                </td>
                                                                <td align="left">
                                                                    <asp:DropDownList ID="ddlCase_Type" runat="server" OnSelectedIndexChanged="ddlCase_Type_SelectedIndexChanged"
                                                                        Visible="true" AutoPostBack="True" CssClass="comboBg">
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td align="right">
                                                                    <table height="43" cellspacing="0" cellpadding="0" width="43">
                                                                        <tr>
                                                                            <td width="43" background="images/theta.jpg" height="43">
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                                <td align="left">
                                                                    :
                                                                    <asp:DropDownList ID="ddlTheta" runat="server" Visible="true" CssClass="comboBg">
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right">
                                                                    <asp:Label ID="Label1" runat="server" Text="Lab Kit: " CssClass="graynormalText" />
                                                                </td>
                                                                <td align="left">
                                                                    <asp:DropDownList runat="server" ID="ddlLabKit" CssClass="comboBg" OnSelectedIndexChanged="ddlKabKit_SelectedIndexChanged"
                                                                        AutoPostBack="True">
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                            <tr style="visbility: collapse; display: none;">
                                                                <td align="right">
                                                                    <asp:Label ID="lblDegradedType" runat="server" Text="Degraded Type: " CssClass="graynormalText" />
                                                                </td>
                                                                <td align="left">
                                                                    <asp:DropDownList ID="ddlDegradedType" runat="server" CssClass="comboBg" ViewStateMode="Enabled">
                                                                        <asp:ListItem Value="ND" Selected="True">Not Degraded</asp:ListItem>
                                                                        <asp:ListItem Value="MD">Mildly Degraded</asp:ListItem>
                                                                        <asp:ListItem Value="SD">Severely Degraded</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="4" />
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="pnlSuspPrfl1" runat="server" Visible="true">
                                <span class="bluetextBold">Comparison Profile 1:</span>
                                <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" style="border: 1px solid #aab3b3;">
                                    <tr>
                                        <td>
                                            <table width="100%" height="50" border="0" cellpadding="0" cellspacing="0" bgcolor="#FFFFFF">
                                                <tr>
                                                    <td>
                                                        <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Label ID="lblSuspPrfl1_Nm" runat="server" Text="Comparison 1 Name" CssClass="graynormalText" />
                                                                    <asp:TextBox ID="txtSuspPrfl1_Nm" runat="server" Width="80%" CssClass="textboxBg" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="4" />
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="pnlSuspPrfl2" runat="server" Visible="false">
                                <span class="bluetextBold">Comparison Profile 2:</span>
                                <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" style="border: 1px solid #aab3b3;">
                                    <tr>
                                        <td>
                                            <table width="100%" height="50" border="0" cellpadding="0" cellspacing="0" bgcolor="#FFFFFF">
                                                <tr>
                                                    <td>
                                                        <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Label ID="lblSuspPrfl2_Nm" runat="server" Text="Comparison 2 Name" CssClass="graynormalText" />
                                                                    <asp:TextBox ID="txtSuspPrfl2_Nm" runat="server" Width="80%" CssClass="textboxBg" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="4" />
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="pnlSuspPrfl3" runat="server" Visible="false">
                                <span class="bluetextBold">Comparison Profile 3:</span>
                                <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" style="border: 1px solid #aab3b3;">
                                    <tr>
                                        <td>
                                            <table width="100%" height="50" border="0" cellpadding="0" cellspacing="0" bgcolor="#FFFFFF">
                                                <tr>
                                                    <td>
                                                        <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Label ID="lblSuspPrfl3_Nm" runat="server" Text="Comparison 3 Name" CssClass="graynormalText" />
                                                                    <asp:TextBox ID="txtSuspPrfl3_Nm" runat="server" Width="80%" CssClass="textboxBg" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="4" />
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="pnlSuspPrfl4" runat="server" Visible="false">
                                <span class="bluetextBold">Comparison Profile 3:</span>
                                <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" style="border: 1px solid #aab3b3;">
                                    <tr>
                                        <td>
                                            <table width="100%" height="50" border="0" cellpadding="0" cellspacing="0" bgcolor="#FFFFFF">
                                                <tr>
                                                    <td>
                                                        <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Label ID="lblSuspPrfl4_Nm" runat="server" Text="Comparison 4 Name" CssClass="graynormalText" />
                                                                    <asp:TextBox ID="txtSuspPrfl4_Nm" runat="server" Width="80%" CssClass="textboxBg" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="4" />
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="pnlKnownPrfl1" runat="server" Visible="false">
                                <span class="bluetextBold">Known Profile 1:</span>
                                <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" style="border: 1px solid #aab3b3;">
                                    <tr>
                                        <td>
                                            <table width="100%" height="50" border="0" cellpadding="0" cellspacing="0" bgcolor="#FFFFFF">
                                                <tr>
                                                    <td>
                                                        <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Label ID="lblKnownPrfl1_Nm" runat="server" Text="Known 1 Name" CssClass="graynormalText" />
                                                                    <asp:TextBox ID="txtKnownPrfl1_Nm" runat="server" Width="80%" CssClass="textboxBg" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="4" />
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="pnlKnownPrfl2" runat="server" Visible="true">
                                <span class="bluetextBold">Known Profile 2:</span>
                                <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" style="border: 1px solid #aab3b3;">
                                    <tr>
                                        <td>
                                            <table width="100%" height="50" border="0" cellpadding="0" cellspacing="0" bgcolor="#FFFFFF">
                                                <tr>
                                                    <td>
                                                        <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Label ID="lblKnownPrfl2_Nm" runat="server" Text="Known 2 Name" CssClass="graynormalText" />
                                                                    <asp:TextBox ID="txtKnownPrfl2_Nm" runat="server" Width="80%" CssClass="textboxBg" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="4" />
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="pnlKnownPrfl3" runat="server" Visible="false">
                                <span class="bluetextBold">Known Profile 3:</span>
                                <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" style="border: 1px solid #aab3b3;">
                                    <tr>
                                        <td>
                                            <table width="100%" height="50" border="0" cellpadding="0" cellspacing="0" bgcolor="#FFFFFF">
                                                <tr>
                                                    <td>
                                                        <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Label ID="lblKnownPrfl3_Nm" runat="server" Text="Known 3 Name" CssClass="graynormalText" />
                                                                    <asp:TextBox ID="txtKnownPrfl3_Nm" runat="server" Width="80%" CssClass="textboxBg" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="4" />
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="pnlKnownPrfl4" runat="server" Visible="true">
                                <span class="bluetextBold">Known Profile 4:</span>
                                <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" style="border: 1px solid #aab3b3;">
                                    <tr>
                                        <td>
                                            <table width="100%" height="50" border="0" cellpadding="0" cellspacing="0" bgcolor="#FFFFFF">
                                                <tr>
                                                    <td>
                                                        <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Label ID="lblKnownPrfl4_Nm" runat="server" Text="Known 4 Name" CssClass="graynormalText" />
                                                                    <asp:TextBox ID="txtKnownPrfl4_Nm" runat="server" Width="80%" CssClass="textboxBg" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="4" />
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="pnlUserWarning" runat="server" Visible="false">
                                <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" style="border: 1px solid #aab3b3;">
                                    <tr>
                                        <td align="center" valign="middle">
                                            <h3>
                                                <a href="#">
                                                    <asp:Label ID="lblWarningMsg" Text="You are not an authorized user. Please contact the Administrator"
                                                        runat="server"></asp:Label></a></h3>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <table cellspacing="0" cellpadding="0" border="0" width="100%" style="margin-top: 10px;">
                                <tr>
                                    <td align="right">
                                        <asp:ImageButton ID="btnBulk" runat="server" OnClick="btnBulk_Click" OnClientClick="this.src='Images/Buttons/Bulk2.png'"
                                            ImageUrl="~/Images/Buttons/Bulk1.png" />
                                        <asp:ImageButton ID="btnGo" runat="server" OnClick="btnGo_Click" OnClientClick="this.src='Images/Buttons/Go2.png'"
                                            ImageUrl="~/Images/Buttons/Go1.png" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <!-------------------------------Content End------------------->
            </td>
        </tr>
    </table>
    <script language="javascript" type="text/javascript">
        var ddcasetypes = document.getElementById('<%= ddlCase_Type.ClientID %>');
        for (i = 0; i < ddcasetypes.length; i++) {
            if (ddcasetypes.options[i].text == '') {
                ddcasetypes.options[i].disabled = true;
            }
        }
    </script>
    <div style="display: none;">
        <script type="text/javascript">
            var images = new Array();
            function preload() {
                for (i = 0; i < preload.arguments.length; i++) {
                    images[i] = new Image();
                    images[i].src = preload.arguments[i];
                }
            }
            preload(
	            "Images/Buttons/Go2.png",
	            "Images/Buttons/Bulk2.png"
            );
        </script>
    </div>
</asp:Content>
