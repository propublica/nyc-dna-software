<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" Inherits="frmViewDropoutRates" Title="DropOut Rates" Codebehind="frmViewDropoutRates.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentHolder" Runat="Server">
<br />
<table width="100%" height="500" border="0" cellspacing="0" cellpadding="0">
    <tr>
    <td width="10"></td>
      <td bgcolor="#FFFFFF" align="center"><table width="99%" height="500" border="0" cellspacing="1" cellpadding="0" bgcolor="#CCCCCC">
          <tr>
            <td><table width="100%" height="500" border="0" cellspacing="0" cellpadding="0">
                <tr>
                  <td bgcolor="#FFFFFF" valign="top" >
         <!-------------------------------Content Here------------------->	
         
<div>
<table cellspacing="0" cellpadding="0" border="0" width="98%" align="center">
<tr>
<td align="left">

<asp:Panel ID="pnlSearch" runat="server" visible="true">
<asp:Label ID="lblSearch" runat="server" CssClass="bluetextBold" Text="Search:" />
<table width="100%" border="1" align="center" cellpadding="0" cellspacing="0" bordercolor="#94c4da">
					
<tr>
	<td>
   <table width="100%" border="0" cellpadding="0" cellspacing="0" bgcolor="#FFFFFF">
	  <tr><td>
        <table cellspacing="0" cellpadding="0" border="0" width="100%">
         <tr valign="middle" align="center">
          <td valign="middle" align="center">
              <asp:Label ID="lblLabKit" runat="server" Text="Lab Kit: " CssClass="gridnormal" Visible="true" ></asp:Label>
              <asp:DropDownList ID="ddLabKit" runat="server" CssClass="comboBg" Visible="true" DataTextField="FieldName" DataValueField="FieldValue">
              </asp:DropDownList>
          </td>
          <td valign="middle" align="center">
              <asp:Label ID="lblNoOfPersons" runat="server" Text="No of Persons Involved: " CssClass="gridnormal" Visible="true" ></asp:Label>
              <asp:DropDownList ID="ddlNoOfPersons" runat="server" CssClass="comboBg" Visible="true">
              </asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;          
          </td>
          <td valign="middle" align="center">
              <asp:Label ID="lblDeducible" runat="server" Text="Deducible: " CssClass="gridnormal" Visible="true" ></asp:Label>
              <asp:DropDownList ID="ddlDeducible" runat="server" CssClass="comboBg" Visible="true">
              <asp:ListItem>Yes</asp:ListItem>
              <asp:ListItem>No</asp:ListItem>
              </asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;
          </td>
          <td valign="middle" align="center">
              <asp:Label ID="lblLocus" runat="server" Text="Locus: " CssClass="gridnormal" Visible="true" ></asp:Label>
              <asp:DropDownList ID="ddlLocus" runat="server" CssClass="comboBg" Visible="true">
              </asp:DropDownList>
          </td>
          <td valign="middle" align="center">
              <asp:Label ID="lblDropOutType" runat="server" Text="DropOut Types: " CssClass="gridnormal" Visible="true" ></asp:Label>
              <asp:DropDownList ID="ddlDropOutType" runat="server" CssClass="comboBg" Visible="true">
              <asp:ListItem>PHET1</asp:ListItem>
              <asp:ListItem>PHET2</asp:ListItem>
              <asp:ListItem>PHOM1</asp:ListItem>
              <asp:ListItem></asp:ListItem>
              </asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp; 
          </td>
          <td valign="middle" align="center">
              <asp:Label ID="lblDropOutOption" runat="server" Text="DNA Template Amount (pg): " CssClass="gridnormal" Visible="true" ></asp:Label>
              <asp:TextBox ID="txtDropOutOption" runat="server" Width="70" CssClass="textboxSmall" Visible="true">
              </asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;                   
          </td>
          <td valign="middle" align="center">
              <asp:ImageButton ID="btnSearch" runat="server" ImageUrl="~/Images/searchIcon.GIF" ToolTip="Search" onclick="btnSearch_Click" />&nbsp;&nbsp;
          </td>
         </tr>         
        </table>
    </td>
	</tr>
	<tr><td height="4" /></tr>
 </table>
</td>
</tr>
</table>
</asp:Panel>

<asp:Panel ID="pnlGrid" runat="server" visible="true">
<asp:Label ID="lblDropOutRates" runat="server" CssClass="bluetextBold" Text="DropOut Rates:" />
<table width="100%" border="1" align="center" cellpadding="0" cellspacing="0" bordercolor="#94c4da">
					
<tr>
	<td>
   <table width="100%" border="0" cellpadding="0" cellspacing="0" bgcolor="#FFFFFF">
	  <tr><td>
        <table cellspacing="0" cellpadding="0" border="0" width="100%">
         <tr>
          <td align="center">
                                  
           <asp:GridView AllowPaging="true" AllowSorting="true" Width="100%" ID="gvDropOut" runat="server" 
                   CellPadding="0" AutoGenerateColumns="False" 
                    CssClass="SelectDataGrid" 
                    PageSize="15" PagerSettings-Visible="true" 
                    PagerSettings-Mode="Numeric" 
                    BorderWidth="1px" BorderColor="#B7BABC"  BorderStyle="Solid"
                  onpageindexchanging="gvDropOut_PageIndexChanging">
                    <HeaderStyle Height="20" CssClass="datagridRowHead" HorizontalAlign="Center" />
                    <RowStyle CssClass="datagridRow1" ></RowStyle>
                    <AlternatingRowStyle CssClass="datagridRow2" />
                    <EditRowStyle CssClass="comboBg" />
                    <SelectedRowStyle CssClass="comboBg" />                        
                    <FooterStyle Height="20" CssClass="datagridRowHead" />				
					<Columns>
					    <asp:BoundField HeaderText="Locus" HtmlEncode="false" DataField="LocusName">
							<HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
							<ItemStyle Width="100" Wrap="false" />
						</asp:BoundField>					    				    
					    <asp:BoundField HeaderText="Dropout Types" HtmlEncode="false" DataField="Type">
							<HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
							<ItemStyle Width="150" Wrap="false" />
						</asp:BoundField>						
						<asp:BoundField HeaderText="DropOut Rate" HtmlEncode="false" DataField="DropOutRate">
							<HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
							<ItemStyle Width="100" Wrap="false" />
						</asp:BoundField>	
				    </Columns>
			</asp:GridView>
          </td>
         </tr>
        </table>
    </td>
	</tr>
	<tr><td height="4" /></tr>
 </table>
</td>
</tr>
</table>
</asp:Panel>

      
  </td>
 </tr>
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

