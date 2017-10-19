<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EvidenceLocus.ascx.cs" Inherits="FST.Web.Controls.EvidenceLocus" %>
<table width="98%" border="0" align="center" cellpadding="0" cellspacing="0">
    <tr>
        <td colspan="3" align="left" valign="top" class="gridnormal">
            <asp:Label runat="server" ID="lblLocusName"></asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="3">
            <img src="images/spacer.gif" width="1" height="3" />
        </td>
    </tr>
    <tr>
        <td align="left" valign="top">
            <asp:DropDownList ID="ddAlleles" runat="server" Width="50px" CssClass="comboBg">
            </asp:DropDownList>
        </td>
        <td align="left" valign="top">
            &nbsp;
        </td>
        <td align="left" valign="top">
            <asp:TextBox ID="txtAlleles" runat="server" CssClass="textboxBg" Width="100px"></asp:TextBox>
        </td>
    </tr>
</table>