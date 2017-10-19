<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" Inherits="frmEditLabKits" Title="Edit Lab Kits" Codebehind="frmEditLabKits.aspx.cs" %>

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

<asp:Panel ID="pnlLabKits" runat="server" Visible="true">
    <asp:Label ID="Label1" runat="server" CssClass="bluetextBold" Text="Lab Kits:" />
    <asp:GridView ID="gvLabKits" runat="server" AutoGenerateColumns="false" Width="100%">
        <HeaderStyle Height="20" CssClass="datagridRowHead" HorizontalAlign="Center" />
        <RowStyle CssClass="datagridRow1" ></RowStyle>
        <AlternatingRowStyle CssClass="datagridRow2" />
        <EditRowStyle CssClass="comboBg" />
        <SelectedRowStyle CssClass="comboBg" />                        
        <FooterStyle Height="20" CssClass="datagridRowHead" />			
        <Columns>
			<asp:BoundField HeaderText="Name" HtmlEncode="false" DataField="FieldName">
				<HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
				<ItemStyle Width="250" Wrap="false" />
			</asp:BoundField>
			<asp:BoundField HeaderText="Unique Identifier" HtmlEncode="false" DataField="FieldValue">
				<HeaderStyle CssClass="datagridRowHead cssHide" HorizontalAlign="Center" Wrap="false" />
				<ItemStyle Width="250" Wrap="false" CssClass="cssHide" />
			</asp:BoundField>
            <asp:TemplateField>
                <HeaderStyle HorizontalAlign="Center" CssClass="datagridRowHead" Wrap="false" />
                <ItemStyle HorizontalAlign="Center" Width="100" Wrap="false" />
                <ItemTemplate>
                    <center>
                        <table cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <td >
                                    <asp:LinkButton ID="lnkEdit" runat="server" Text="Edit" OnClick="lnkEdit_Click" ></asp:LinkButton>
                                </td>
                            </tr>
                        </table>
                    </center>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle HorizontalAlign="Center" Width="100" Wrap="false" />
                    <ItemTemplate>
                    <center>
                        <table cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <td >
                                    <asp:LinkButton ID="lnkDelete" runat="server" Text="Delete" OnClick="lnkDelete_Click" onclientclick="return lnkDelete_OnClientClick(this);" ></asp:LinkButton>
                                </td>
                            </tr>
                        </table>
                    </center>
                </ItemTemplate>
            </asp:TemplateField>	
        </Columns>
    </asp:GridView>
    <script language="javascript" type="text/javascript">
        function lnkDelete_OnClientClick(elem) {
            var name = elem.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.childNodes[0].innerText;
            if (!confirm('Are you sure you want to delete the "' + name + '" Lab Kit?')) {
                return false;
            }
        }
    </script>
    <div style=" text-align:right;">
        <asp:ImageButton ID="btnNewLabKit" runat="server" OnClick="btnNewLabKit_Click" Text="Upload New Lab Kit" ImageUrl="~/Images/Buttons/New1.png" OnClientClick="this.src='../Images/Buttons/New2.png';" />
    </div>
</asp:Panel>

<asp:Panel ID="pnlUploadNew" runat="server" Visible="false">
    <div style="width:auto; margin-left:auto; margin-right:auto; margin-top:20px; text-align:center">
        <asp:Label runat="server" ID="Label2" Text="Lab Kit Name: " CssClass="gridnormal" />
        <asp:TextBox ID="txtLabKitName" runat="server" Width="200" CssClass="uploader" />
        <asp:Label runat="server" ID="lblLabKitUpload" Text="File: " CssClass="gridnormal" />
        <div style="position:absolute; margin-top:-25px; margin-left:393px;">
            <img id="ImgUpload" style="cursor:arrow; margin-left:309px;" src="../Images/Buttons/Browse1.png" />
        </div>
        <asp:FileUpload 
            ID="FileUpload1" runat="server" 
            Width="303" CssClass="uploader" 
            style="position:relative; cursor:hand; height:22px; background-color:#fff; border-color:#000; border-width:1px; -ms-filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0); filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);"
            onchange="document.getElementById('txtUploadText').value = this.value;"
            />
        <div style="position:absolute; margin-top:-25px; margin-left:393px;">
            <input id="txtUploadText" type="text" readonly style="margin-left:76px; background-color:#fff; border-color:#000; border-width:1px; width: 229px; position:absolute; margin-top:3px;" />
        </div>
        <asp:ImageButton 
            ID="btnLabKitUpload" runat="server" 
            Text="Upload" OnClick="btnLabKitUpload_Click" 
            ImageUrl="~/Images/Buttons/Upload1.png" OnClientClick="this.src='../Images/Buttons/Upload2.png'" 
            style="position:absolute; margin-top:-2px; margin-left:6px;"
            />
        <br />
        <asp:Label runat="server" ID="lblUploadResult" CssClass="gridnormal"></asp:Label>
    </div>
</asp:Panel>

<asp:Panel ID="pnlSearch" runat="server" visible="false">
<asp:Label ID="lblLabKitNameEdit" runat="server" CssClass="bluetextBold" Text="Currently Editing: " /><br />
<asp:Label ID="lblSearch" runat="server" CssClass="bluetextBold" Text="Search:" />
<table width="100%" border="1" align="center" cellpadding="0" cellspacing="0" bordercolor="#94c4da">
					
<tr>
	<td>
   <table width="100%" border="0" cellpadding="0" cellspacing="0" bgcolor="#FFFFFF">
	  <tr><td>
        <table cellspacing="0" cellpadding="0" border="0" width="100%">
         <tr>
          <td>
          <asp:Label ID="lblNoOfPersons" runat="server" Text="No of Persons Involved: " CssClass="gridnormal" Visible="true" ></asp:Label>
          <asp:DropDownList ID="ddlNoOfPersons" runat="server" CssClass="comboBg" Visible="true">
          </asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;
          <asp:Label ID="lblDropOutType" runat="server" Text="DropOut Types: " CssClass="gridnormal" Visible="true" ></asp:Label>
          <asp:DropDownList ID="ddlDropOutType" runat="server" CssClass="comboBg" Visible="true">
          </asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;
          <asp:Label ID="lblDeducible" runat="server" Text="Deducible: " CssClass="gridnormal" Visible="true" ></asp:Label>
          <asp:DropDownList ID="ddlDeducible" runat="server" CssClass="comboBg" Visible="true">
          <asp:ListItem>Yes</asp:ListItem>
          <asp:ListItem>No</asp:ListItem>
          <asp:ListItem></asp:ListItem>
          </asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;
          <asp:Label ID="lblDropOutOption" runat="server" Text="DropOut Options: " CssClass="gridnormal" Visible="true" ></asp:Label>
          <asp:DropDownList ID="ddlDropOutOption" runat="server" CssClass="comboBg" Visible="true">
          </asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;
          <asp:Label ID="lblLocus" runat="server" Text="Locus: " CssClass="gridnormal" Visible="true" ></asp:Label>
          <asp:DropDownList ID="ddlLocus" runat="server" CssClass="comboBg" Visible="true">
          </asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;          
          <asp:ImageButton ID="btnSearch" runat="server" ImageUrl="~/Images/searchIcon.GIF" ToolTip="Search" onclick="btnSearch_Click" />&nbsp;&nbsp;
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

<asp:Panel ID="pnlGrid" runat="server" visible="false">
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

<asp:Panel ID="pnlDropIn" runat="server" visible="false">
<asp:Label ID="lblDropInRates" runat="server" CssClass="bluetextBold" Text="DropIn Rates:" />
<table width="100%" border="1" align="center" cellpadding="0" cellspacing="0" bordercolor="#94c4da">
					
<tr>
	<td>
   <table width="100%" border="0" cellpadding="0" cellspacing="0" bgcolor="#FFFFFF">
	  <tr><td>
        <table cellspacing="0" cellpadding="0" border="0" width="100%">
         <tr>
          <td align="center">
                                  
           <asp:GridView AllowPaging="true" AllowSorting="true" Width="100%" ID="gvDropIn" runat="server" 
                   CellPadding="0" AutoGenerateColumns="False" 
                    CssClass="SelectDataGrid" 
                    PageSize="15" PagerSettings-Visible="true" 
                    PagerSettings-Mode="Numeric" 
                    BorderWidth="1px" BorderColor="#B7BABC"  BorderStyle="Solid">
                    <HeaderStyle Height="20" CssClass="datagridRowHead" HorizontalAlign="Center" />
                    <RowStyle CssClass="datagridRow1" ></RowStyle>
                    <AlternatingRowStyle CssClass="datagridRow2" />
                    <EditRowStyle CssClass="comboBg" />
                    <SelectedRowStyle CssClass="comboBg" />                        
                    <FooterStyle Height="20" CssClass="datagridRowHead" />				
					<Columns>	
					    <asp:TemplateField HeaderText="ID" Visible="false">
							<HeaderStyle HorizontalAlign="Center" CssClass="datagridRowHead" Wrap="false" />
							<ItemStyle HorizontalAlign="Center" Width="100" Wrap="false" />
							<ItemTemplate>
							<center>
								<table cellpadding="0" cellspacing="0" border="0">
									<tr>
										<td >
											<asp:Label ID="lblID" Text='<%# Bind("ID") %>' runat="server" BackColor="Transparent" CssClass="gridnormal" BorderStyle="None"/>
										</td>
									</tr>
								</table>
								</center>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:BoundField HeaderText="DropinRateID" HtmlEncode="false" DataField="DropinRateID">
							<HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
							<ItemStyle Width="100" Wrap="false" />
						</asp:BoundField>
						<asp:BoundField HeaderText="Type" HtmlEncode="false" DataField="Type">
							<HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
							<ItemStyle Width="180" Wrap="false" />
						</asp:BoundField>
						<asp:TemplateField HeaderText="DropIn Rate">
							<HeaderStyle HorizontalAlign="Center" CssClass="datagridRowHead" Wrap="false" />
							<ItemStyle HorizontalAlign="Center" Width="70" Wrap="false" />
							<ItemTemplate>
							<center>
								<table cellpadding="0" cellspacing="0" border="0">
									<tr>
										<td >
											<asp:TextBox ID="txtDropInRate" runat="server" Text='<%# Bind("DropInRate") %>' BackColor="Transparent" BorderStyle="None" />
										</td>
									</tr>
								</table>
								</center>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="DropIn Rate Old" Visible="false">
							<HeaderStyle HorizontalAlign="Center" CssClass="datagridRowHead" Wrap="false" />
							<ItemStyle HorizontalAlign="Center" Width="100" Wrap="false" />
							<ItemTemplate>
							<center>
								<table cellpadding="0" cellspacing="0" border="0">
									<tr>
										<td >
											<asp:Label ID="lblOldDropInRate" Text='<%# Bind("DropInRate") %>' runat="server" />
										</td>
									</tr>
								</table>
								</center>
							</ItemTemplate>
						</asp:TemplateField>					
				    </Columns>
			</asp:GridView>
          </td>
         </tr>
        </table>
    </td>
	</tr>
	<tr><td /><asp:ImageButton ID="btnSaveDropIns" runat="server" ImageUrl="~/Images/save-icon.gif" ToolTip="Save" onclick="btnSaveDropIns_Click" /></tr>
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
<script type="text/javascript">
    var images = new Array();
    var UploadButton = document.getElementById('<%=FileUpload1.ClientID %>');
    // the line below is not compatible with IE9+ and other browsers. sorry, whoever has to work on this...
    if(UploadButton != null) UploadButton.attachEvent('onclick', function () { btnInput_MouseDownJs('ImgUpload', '<%=FileUpload1.ClientID %>'); });
    function btnInput_MouseDownJs(imgId, upload) {
        var img = document.getElementById(imgId);
        img.src = '../Images/Buttons/Browse2.png';
        window.setTimeout(function () {
            img.src = '../Images/Buttons/Browse1.png';
        }, 250);
    }
    function preload() {
        for (i = 0; i < preload.arguments.length; i++) {
            images[i] = new Image();
            images[i].src = preload.arguments[i];
        }
    }
    preload(
        "../Images/Buttons/New2.png",
        "../Images/Buttons/Browse2.png",
        "../Images/Buttons/Upload2.png"
    );
</script>
            
</asp:Content>

