<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Evidence.ascx.cs" Inherits="FST.Web.Controls.Evidence" %>
<div class="demo">
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Menu ID="Menu1" Width="350px" runat="server" Orientation="Horizontal" StaticEnableDefaultPopOutImage="False"
                    OnMenuItemClick="Menu1_MenuItemClick">
                    <Items>
                        <asp:MenuItem ImageUrl="~/images/rep1_on.gif" Text="" Value="0" Selected="true">
                        </asp:MenuItem>
                        <asp:MenuItem ImageUrl="~/images/rep2_off.gif" Text=" " Value="1"></asp:MenuItem>
                        <asp:MenuItem ImageUrl="~/images/rep3_off.gif" Text="" Value="2"></asp:MenuItem>
                    </Items>
                </asp:Menu>
                <asp:MultiView ID="viewReplicates" runat="server" ActiveViewIndex="0">
                    <asp:View runat="server" ID="tabRep1">
                        <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" class="tablebg">
                            <tr>
                                <td>
                                    <asp:Table runat="server" ID="tblRep1" width="96%" border="0" cellpadding="0" cellspacing="0" style="margin: 10px; margin-left: 20px;">
                                    </asp:Table>
                                </td>
                            </tr>
                        </table>
                    </asp:View>
                    <asp:View ID="tabRep2" runat="server">
                        <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" class="tablebg">
                            <tr>
                                <td>
                                    <asp:Table runat="server" ID="tblRep2" width="96%" border="0" cellpadding="0" cellspacing="0" style="margin: 10px; margin-left: 20px;">
                                    </asp:Table>
                                </td>
                            </tr>
                        </table>
                    </asp:View>
                    <asp:View ID="tabRep3" runat="server">
                        <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" class="tablebg">
                            <tr>
                                <td>
                                    <asp:Table runat="server" ID="tblRep3" width="96%" border="0" cellpadding="0" cellspacing="0" style="margin: 10px; margin-left: 20px;">
                                    </asp:Table>
                                </td>
                            </tr>
                        </table>
                    </asp:View>
                </asp:MultiView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
