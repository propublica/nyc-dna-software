<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfileLocus.ascx.cs" Inherits="FST.Web.Controls.ProfileLocus" %>
<table width="98%" border="0" align="center" cellpadding="0" cellspacing="0">
    <tr>
        <td colspan="5" align="left" valign="top" class="gridnormal">
            <asp:Label runat="server" ID="lblLocusName"></asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="5">
            <img src="images/spacer.gif" width="1" height="3" />
        </td>
    </tr>
    <tr>
        <td align="left" valign="top">
            <asp:DropDownList ID="ddAlleles1" runat="server" Width="50px" CssClass="comboBg">
            </asp:DropDownList>
        </td>
        <td align="left" valign="top">
            &nbsp;
        </td>
        <td align="left" valign="top">
            <asp:DropDownList ID="ddAlleles2" runat="server" Width="50px" CssClass="comboBg">
            </asp:DropDownList>
        </td>
        <td align="left" valign="top">
            &nbsp;
        </td>
        <td align="left" valign="top">
            <asp:TextBox ID="txtAlleles" runat="server" CssClass="textboxSmall"></asp:TextBox>
        </td>
    </tr>
</table>