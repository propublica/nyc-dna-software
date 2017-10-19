<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" Inherits="Admin_frmScenarioManagement" Codebehind="frmScenarioManagement.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentHolder" Runat="Server">
    <asp:GridView Width="100%" ID="gvCases" runat="server" 
        CellPadding="0" AutoGenerateColumns="false" DataKeyNames="Case_ID"  
        CssClass="SelectDataGrid" 
        BorderWidth="1px" BorderColor="#B7BABC"  BorderStyle="Solid">
        <HeaderStyle Height="20" CssClass="datagridRowHead" HorizontalAlign="Center" />
        <RowStyle CssClass="datagridRow1" ></RowStyle>
        <AlternatingRowStyle CssClass="datagridRow2" />
        <EditRowStyle CssClass="comboBg" />
        <SelectedRowStyle CssClass="comboBg" />                        
        <FooterStyle Height="20" CssClass="datagridRowHead" />				
		<Columns>
            <asp:TemplateField >
                <HeaderTemplate>
                    <asp:ImageButton ID="btnOn" OnClick="btnOn_Click" Width="48%" runat="server" Text="On" ImageUrl="../Images/Buttons/On1.png" OnClientClick="this.src='../Images/Buttons/On2.png';" />
                    <asp:ImageButton ID="btnOff" OnClick="btnOff_Click" Width="48%" runat="server" Text="Off" ImageUrl="../Images/Buttons/Off1.png" OnClientClick="this.src='../Images/Buttons/Off2.png';" />
                </HeaderTemplate>
                <HeaderStyle Width="100px" />
                <ItemTemplate>
                    <asp:CheckBox ID="chkActivateDeactivate" runat="server" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="ID" SortExpression="ID">
			    <HeaderStyle HorizontalAlign="Center" CssClass="datagridRowHead" Wrap="false" />
			    <ItemStyle HorizontalAlign="Center" Width="70" Wrap="false" />
			    <ItemTemplate>
			    <center>
				    <table cellpadding="0" cellspacing="0" border="0">
					    <tr>
						    <td >
							    <asp:Label ID="lblID" Text='<%# Bind("Case_Id") %>' runat="server" BackColor="Transparent" CssClass="gridnormal" BorderStyle="None" />
						    </td>
					    </tr>
				    </table>
				    </center>
			    </ItemTemplate>
		    </asp:TemplateField>
            <asp:BoundField DataField="Case_Desc" HeaderText="Scenario Title" />
            <asp:BoundField DataField="Status" HeaderText="Enabled">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="Ordinal" HeaderText="Ordinal">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
        </Columns>
</asp:GridView>
        <script type="text/javascript">
            var images = new Array();
            function preload() {
                for (i = 0; i < preload.arguments.length; i++) {
                    images[i] = new Image();
                    images[i].src = preload.arguments[i];
                }
            }
            preload(
	            "../Images/Buttons/On2.png",
                "../Images/Buttons/Off2.png"
            );
        </script>
</asp:Content>

