<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" Inherits="frmDegradedDropOutRate" Title="Degraded DropOut Rates:" Codebehind="frmDegradedDropOutRate.aspx.cs" %>

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
         <tr>
          <td>
          <asp:Label ID="lblNoOfPersons" runat="server" Text="No of Persons:" CssClass="gridnormal" Visible="true" ></asp:Label>
          <asp:DropDownList ID="ddlNoOfPersons" runat="server" CssClass="comboBg" Visible="true">
          </asp:DropDownList>&nbsp;
          <asp:Label ID="lblDropOutType" runat="server" Text="DropOut Types:" CssClass="gridnormal" Visible="true" ></asp:Label>
          <asp:DropDownList ID="ddlDropOutType" runat="server" CssClass="comboBg" Visible="true">
          </asp:DropDownList>&nbsp;
          <asp:Label ID="lblDeducible" runat="server" Text="Deducible:" CssClass="gridnormal" Visible="true" ></asp:Label>
          <asp:DropDownList ID="ddlDeducible" runat="server" CssClass="comboBg" Visible="true">
          <asp:ListItem>Yes</asp:ListItem>
          <asp:ListItem>No</asp:ListItem>
          <asp:ListItem></asp:ListItem>
          </asp:DropDownList>&nbsp;
          <asp:Label ID="lblDegradationType" runat="server" Text="Degradation Type:" CssClass="gridnormal" Visible="true" ></asp:Label>
          <asp:DropDownList ID="ddlDegradationType" runat="server" CssClass="comboBg" Visible="true">
          <asp:ListItem>Mild</asp:ListItem>
          <asp:ListItem>Severe</asp:ListItem>
          <asp:ListItem></asp:ListItem>
          </asp:DropDownList>&nbsp;
          <asp:Label ID="lblDropOutOption" runat="server" Text="DropOut Options:" CssClass="gridnormal" Visible="true" ></asp:Label>
          <asp:DropDownList ID="ddlDropOutOption" runat="server" CssClass="comboBg" Visible="true">
          </asp:DropDownList>&nbsp;
          <asp:Label ID="lblLocus" runat="server" Text="Locus:" CssClass="gridnormal" Visible="true" ></asp:Label>
          <asp:DropDownList ID="ddlLocus" runat="server" CssClass="comboBg" Visible="true">
          </asp:DropDownList>&nbsp;       
          <asp:ImageButton ID="btnSearch" runat="server" ImageUrl="~/Images/searchIcon.GIF" ToolTip="Search" onclick="btnSearch_Click" />&nbsp;
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
<asp:Label ID="lblDropOutRates" runat="server" CssClass="bluetextBold" Text="Degraded DropOut Factors:" />
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
					    <asp:TemplateField HeaderText="DropoutRateID" Visible="false">
							<HeaderStyle HorizontalAlign="Center" CssClass="datagridRowHead" Wrap="false" />
							<ItemStyle HorizontalAlign="Center" Width="100" Wrap="false" />
							<ItemTemplate>
							<center>
								<table cellpadding="0" cellspacing="0" border="0">
									<tr>
										<td >
											<asp:Label ID="lblDropoutRateID" Text='<%# Bind("DropoutRateID") %>' runat="server" />
										</td>
									</tr>
								</table>
								</center>
							</ItemTemplate>
						</asp:TemplateField>				    
					    <asp:BoundField HeaderText="No Of Persons Involved" HtmlEncode="false" DataField="NoOfPersonsInvolvd">
							<HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
							<ItemStyle Width="100" Wrap="false" />
						</asp:BoundField>
					    <asp:BoundField HeaderText="Dropout Types" HtmlEncode="false" DataField="Description_Type">
							<HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
							<ItemStyle Width="150" Wrap="false" />
						</asp:BoundField>
						<asp:BoundField HeaderText="Deducible" HtmlEncode="false" DataField="Deducible">
							<HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
							<ItemStyle Width="80" Wrap="false" />
						</asp:BoundField>
						<asp:BoundField HeaderText="Degradation Type" HtmlEncode="false" DataField="Degradation_Type">
							<HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
							<ItemStyle Width="80" Wrap="false" />
						</asp:BoundField>
						<asp:BoundField HeaderText="Dropout Option" HtmlEncode="false" DataField="DropoutOption">
							<HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
							<ItemStyle Width="100" Wrap="false" />
						</asp:BoundField>
						<asp:BoundField HeaderText="Locus" HtmlEncode="false" DataField="Locus">
							<HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
							<ItemStyle Width="100" Wrap="false" />
						</asp:BoundField>
						<asp:TemplateField HeaderText="DropOut Rate">
							<HeaderStyle HorizontalAlign="Center" CssClass="datagridRowHead" Wrap="false" />
							<ItemStyle HorizontalAlign="Center" Width="70" Wrap="false" />
							<ItemTemplate>
							<center>
								<table cellpadding="0" cellspacing="0" border="0">
									<tr>
										<td >
											<asp:TextBox ID="txtDropOutRate" runat="server" Text='<%# Bind("DropOutRate") %>' BackColor="Transparent" BorderStyle="None" />
										</td>
									</tr>
								</table>
								</center>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="DropOut Rate Old" Visible="false">
							<HeaderStyle HorizontalAlign="Center" CssClass="datagridRowHead" Wrap="false" />
							<ItemStyle HorizontalAlign="Center" Width="100" Wrap="false" />
							<ItemTemplate>
							<center>
								<table cellpadding="0" cellspacing="0" border="0">
									<tr>
										<td >
											<asp:Label ID="lblOldDropOutRate" Text='<%# Bind("DropOutRate") %>' runat="server" />
										</td>
									</tr>
								</table>
								</center>
							</ItemTemplate>
						</asp:TemplateField>	
						<asp:BoundField HeaderText="typeID" HtmlEncode="false" Visible="false" DataField="typeID">
							<HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
							<ItemStyle Width="100" Wrap="false" />
						</asp:BoundField>
					    <asp:BoundField HeaderText="dropOptionID" HtmlEncode="false" Visible="false" DataField="dropOptionID">
							<HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
							<ItemStyle Width="100" Wrap="false" />
						</asp:BoundField>
						<asp:BoundField HeaderText="LocusID" HtmlEncode="false" Visible="false" DataField="LocusID">
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

