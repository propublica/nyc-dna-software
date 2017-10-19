<%@ Page Title="Frequency" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" Inherits="frmEditFrequency" Codebehind="frmEditFrequency.aspx.cs" %>

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
<table cellspacing="0" cellpadding="0" border="0" width="90%" align="center">
<tr>
<td align="left">

<asp:Panel ID="pnlSearch" runat="server" visible="true">
<asp:Label ID="lblSearch" runat="server" CssClass="bluetextBold" Text="Search:" />
<table width="100%" border="1" align="center" cellpadding="0" cellspacing="0" style="border-color:#94c4da;">
					
<tr>
	<td>
   <table width="100%" border="0" cellpadding="0" cellspacing="0" bgcolor="#FFFFFF">
	  <tr><td>
        <table cellspacing="0" cellpadding="0" border="0" width="100%">
         <tr>
          <td>
          <asp:Label ID="lblRace" runat="server" Text="Race: " CssClass="gridnormal" Visible="true" ></asp:Label>
          <asp:DropDownList ID="ddlRace" runat="server" CssClass="comboBg" Visible="true">
           </asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
          <asp:Label ID="lblLocus" runat="server" Text="Locus: " CssClass="gridnormal" Visible="true" ></asp:Label>
          <asp:DropDownList ID="ddlLocus" runat="server" CssClass="comboBg" Visible="true" 
                  onselectedindexchanged="ddlLocus_SelectedIndexChanged" AutoPostBack="True">
           </asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
           <asp:Label ID="lblAlleleNo" runat="server" Text="AlleleNo: " CssClass="gridnormal" Visible="true" ></asp:Label>
          <asp:DropDownList ID="ddlAlleleNo" runat="server" CssClass="comboBg" Visible="true">
           </asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
          <asp:ImageButton ID="btnSearch" runat="server" ImageUrl="~/Images/searchIcon.GIF" ToolTip="Search" onclick="btnSearch_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
          <asp:ImageButton ID="btnSave" runat="server" ImageUrl="~/Images/save-icon.gif" ToolTip="Save" onclick="btnSave_Click" />
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
<asp:Label ID="lblFreqView" runat="server" CssClass="bluetextBold" Text="Frequency View:" />
<table width="100%" border="1" align="center" cellpadding="0" cellspacing="0" bordercolor="#94c4da">
					
<tr>
	<td>
   <table width="100%" border="0" cellpadding="0" cellspacing="0" bgcolor="#FFFFFF">
	  <tr><td>
        <table cellspacing="0" cellpadding="0" border="0" width="100%">
         <tr>
          <td align="center">
                                  
           <asp:GridView AllowPaging="true" AllowSorting="true" Width="100%" ID="gvFreqView" runat="server" 
                   CellPadding="0" AutoGenerateColumns="False" 
                    CssClass="SelectDataGrid" 
                    PageSize="15" PagerSettings-Visible="true" 
                    PagerSettings-Mode="Numeric" 
                    BorderWidth="1px" BorderColor="#B7BABC"  BorderStyle="Solid"
                  onpageindexchanging="gvFreqView_PageIndexChanging">
                    <HeaderStyle Height="20" CssClass="datagridRowHead" HorizontalAlign="Center" />
                    <RowStyle CssClass="datagridRow1" ></RowStyle>
                    <AlternatingRowStyle CssClass="datagridRow2" />
                    <EditRowStyle CssClass="comboBg" />
                    <SelectedRowStyle CssClass="comboBg" />                         
                    <FooterStyle Height="20" CssClass="datagridRowHead" />				
					<Columns>	
					    <asp:TemplateField HeaderText="Frequency ID" Visible="false">
							<HeaderStyle HorizontalAlign="Center" CssClass="datagridRowHead" Wrap="false" />
							<ItemStyle HorizontalAlign="Center" Width="100" Wrap="false" />
							<ItemTemplate>
							<center>
								<table cellpadding="0" cellspacing="0" border="0">
									<tr>
										<td >
											<asp:Label ID="lblFrequency_ID" Text='<%# Bind("Frequency_ID") %>' runat="server" />
										</td>
									</tr>
								</table>
								</center>
							</ItemTemplate>
						</asp:TemplateField>				    
					    <asp:BoundField HeaderText="Race" HtmlEncode="false" DataField="Race">
							<HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
							<ItemStyle Width="100" Wrap="false" />
						</asp:BoundField>
					    <asp:BoundField HeaderText="Locus" HtmlEncode="false" DataField="Locus">
							<HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
							<ItemStyle Width="100" Wrap="false" />
						</asp:BoundField>
						<asp:BoundField HeaderText="Allele No" HtmlEncode="false" DataField="AlleleNo">
							<HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
							<ItemStyle Width="100" Wrap="false" />
						</asp:BoundField>
						<asp:TemplateField HeaderText="Frequency">
							<HeaderStyle HorizontalAlign="Center" CssClass="datagridRowHead" Wrap="false" />
							<ItemStyle HorizontalAlign="Center" Width="100" Wrap="false" />
							<ItemTemplate>
							<center>
								<table cellpadding="0" cellspacing="0" border="0">
									<tr>
										<td >
											<asp:TextBox ID="txtFrequency" runat="server" Text='<%# Bind("Frequency") %>' BackColor="Transparent" BorderStyle="None"  />
										</td>
									</tr>
								</table>
								</center>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Frequency Old" Visible="false">
							<HeaderStyle HorizontalAlign="Center" CssClass="datagridRowHead" Wrap="false" />
							<ItemStyle HorizontalAlign="Center" Width="100" Wrap="false" />
							<ItemTemplate>
							<center>
								<table cellpadding="0" cellspacing="0" border="0">
									<tr>
										<td >
											<asp:Label ID="lblOldFrequency" Text='<%# Bind("Frequency") %>' runat="server" />
										</td>
									</tr>
								</table>
								</center>
							</ItemTemplate>
						</asp:TemplateField>	
						<asp:BoundField HeaderText="EthnicID" HtmlEncode="false" Visible="false" DataField="EthnicID">
							<HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
							<ItemStyle Width="100" Wrap="false" />
						</asp:BoundField>
					    <asp:BoundField HeaderText="LocusID" HtmlEncode="false" Visible="false" DataField="LocusID">
							<HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
							<ItemStyle Width="100" Wrap="false" />
						</asp:BoundField>
						<asp:BoundField HeaderText="AlleleID" HtmlEncode="false" Visible="false" DataField="AlleleID">
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

