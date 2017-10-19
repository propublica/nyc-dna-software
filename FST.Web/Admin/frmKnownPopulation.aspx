<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Inherits="Admin_frmKnownPopulation" Title="Known Population"
    MaintainScrollPositionOnPostback="true" CodeBehind="frmKnownPopulation.aspx.cs" %>

<%@ Register Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register TagPrefix="FST" TagName="Profile" Src="~/Controls/Profile.ascx" %>
<asp:Content id="Content1" contentplaceholderid="MainContentHolder" runat="Server">
    <asp:ScriptManager id="ScriptManager2" runat="server">
</asp:ScriptManager>
    <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0">
        <tr>
            <td align="left" valign="top">
                <table width="99%" border="0" align="center" cellpadding="0" cellspacing="0" bgcolor="#ededed">
                    <tr>
                        <td align="left" valign="top">
                            <img src="../Images/spacer.gif" width="10" height="2" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left" valign="top">
                            <!-------------------------------Content Here------------------->
                            <asp:updatepanel id="UpdatePanel1" runat="server">
                                <contenttemplate>
                                    <table cellspacing="0" cellpadding="0" border="0" width="99%" align="center">
                                        <tr>
                                            <td width="33%" align="left">
                                                <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                                    <tr>
                                                        <td valign="top" align="right">
                                                            <asp:Label ID="lblRace" runat="server" Text="Race:" CssClass="bluetextBold"></asp:Label>&nbsp;
                                                            <asp:DropDownList ID="dlRace" CssClass="comboBg" AutoPostBack="True" runat="server"
                                                                OnSelectedIndexChanged="dlRace_SelectedIndexChanged">
                                                            </asp:DropDownList>&nbsp;
                                                            <asp:Label ID="lblActive" runat="server" Text="Activate All:" CssClass="bluetextBold"></asp:Label>&nbsp;
                                                            <asp:CheckBox ID="chkActive" CssClass="comboBg" AutoPostBack="True" runat="server" 
                                                            TextAlign="Left" oncheckedchanged="chkActive_CheckedChanged" >
                                                            </asp:CheckBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left" valign="top">
                                                            <img src="../Images/spacer.gif" width="10" height="2" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:GridView AllowPaging="true" AllowSorting="true" Width="100%" ID="gvKnownPopulation" runat="server" 
                                                               CellPadding="0" AutoGenerateColumns="False" DataKeyNames ="ID"  
                                                                CssClass="SelectDataGrid" 
                                                                PageSize="30" PagerSettings-Visible="true" 
                                                                PagerSettings-Mode="Numeric" 
                                                                BorderWidth="1px" BorderColor="#B7BABC"  BorderStyle="Solid"
                                                              onpageindexchanging="gvKnownPopulation_PageIndexChanging" OnRowDataBound="gvKnownPopulation_RowDataBound"
                                                                OnSelectedIndexChanged="gvKnownPopulation_SelectedIndexChanged" onsorting="gvKnownPopulation_Sorting">
                                                                <HeaderStyle Height="20" CssClass="datagridRowHead" HorizontalAlign="Center" />
                                                                <RowStyle CssClass="datagridRow1" ></RowStyle>
                                                                <AlternatingRowStyle CssClass="datagridRow2" />
                                                                <EditRowStyle CssClass="comboBg" />
                                                                <SelectedRowStyle CssClass="comboBg" BackColor="#0099cc" ForeColor="White" Font-Bold="true" />                        
                                                                <FooterStyle Height="20" CssClass="datagridRowHead" />				
					                                            <Columns>
                                                                    <asp:TemplateField >
                                                                        <HeaderTemplate>
                                                                            <asp:ImageButton runat="server" Width="100%" Text="Delete" ID="btnDelete" OnClick="btnDelete_Click" ImageUrl="../Images/Buttons/Delete1.png" OnClientClick="this.src='../Images/Buttons/Delete2.png';" />
                                                                        </HeaderTemplate>
                                                                        <HeaderStyle Width="60px" />
                                                                        <ItemTemplate>
                                                                            <asp:CheckBox ID="chkDelete" runat="server" />
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
					                                                <%--<asp:BoundField HeaderText="Race" HtmlEncode="false" DataField="Race" SortExpression="Race">
							                                            <HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
							                                            <ItemStyle Width="60" Wrap="false" HorizontalAlign="Center" />
						                                            </asp:BoundField>--%>
					                                                <asp:TemplateField HeaderText="ID" SortExpression="ID">
							                                            <HeaderStyle HorizontalAlign="Center" CssClass="datagridRowHead" Wrap="false" />
							                                            <ItemStyle HorizontalAlign="Center" Width="70" Wrap="false" />
							                                            <ItemTemplate>
							                                            <center>
								                                            <table cellpadding="0" cellspacing="0" border="0">
									                                            <tr>
										                                            <td>
											                                            <asp:Label ID="lblID" Text='<%# Bind("ID") %>' runat="server" BackColor="Transparent" CssClass="gridnormal" BorderStyle="None" />
										                                            </td>
									                                            </tr>
								                                            </table>
								                                            </center>
							                                            </ItemTemplate>
						                                            </asp:TemplateField>
						                                            <asp:BoundField HeaderText="Active" HtmlEncode="false" DataField="Active" SortExpression="Active">
							                                            <HeaderStyle CssClass="datagridRowHead" HorizontalAlign="Center" Wrap="false" />
							                                            <ItemStyle Width="40" Wrap="false" HorizontalAlign="Center" />
						                                            </asp:BoundField>				    
					                                                <%--<asp:TemplateField HeaderText="Active">
							                                            <HeaderStyle HorizontalAlign="Center" CssClass="datagridRowHead" Wrap="false" />
							                                            <ItemStyle HorizontalAlign="Center" Width="70" Wrap="false" />
							                                            <ItemTemplate>
							                                            <center>
								                                            <table cellpadding="0" cellspacing="0" border="0">
									                                            <tr>
										                                            <td >
											                                            <asp:DropDownList ID="dlGridActive" runat="server" Text='<%# Bind("Active") %>' BackColor="Transparent" BorderStyle="None" >
											                                            <asp:ListItem>Yes</asp:ListItem>
											                                            <asp:ListItem>No</asp:ListItem>
											                                            </asp:DropDownList>
										                                            </td>
									                                            </tr>
								                                            </table>
								                                            </center>
							                                            </ItemTemplate>
						                                            </asp:TemplateField>--%>
                                                                    <asp:TemplateField >
                                                                        <HeaderTemplate>
                                                                            <asp:ImageButton ID="btnOn" OnClick="btnOn_Click" Width="50px" runat="server" Text="On" ImageUrl="../Images/Buttons/On1.png" OnClientClick="this.src='../Images/Buttons/On2.png';" />
                                                                            <asp:ImageButton ID="btnOff" OnClick="btnOff_Click" Width="50px" runat="server" Text="Off" ImageUrl="../Images/Buttons/Off1.png" OnClientClick="this.src='../Images/Buttons/Off2.png';" />
                                                                        </HeaderTemplate>
                                                                        <HeaderStyle Width="48px" />
                                                                        <ItemTemplate>
                                                                            <asp:CheckBox ID="chkActivateDeactivate" runat="server" />
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
				                                                </Columns>
			                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td align="left" valign="top">
                                                <img src="../Images/spacer.gif" width="2" height="2" />
                                            </td>
                                            <td valign="top" width="66%" align="left">
                                                <table cellspacing="0" cellpadding="0" border="1" style="border-color: #cccccc" width="99%"
                                                    align="center" class="tableheader">
                                                    <tr>
                                                        <td colspan="2">
                                                            <h3>
                                                                <a href="#"> 
                                                                    <asp:Label ID="lblKnownPopulation" Text="Population:" runat="server"></asp:Label>
                                                                </a>
                                                            </h3>
                                                            <div>
		                                                        <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" class="tablebg">
                                                                    <tr style="height:40px;">
                                                                        <td class="gridnormal" valign="middle">
                                                                            <div style="margin-left:20px; vertical-align:middle; ">
                                                                                ID: <asp:TextBox ID="tID" runat="server" Width="100" CssClass="textboxSmall"></asp:TextBox>
                                                                            </div>
                                                                        </td>
                                                                        <td align="left" class="gridnormal" valign="middle" style="width:50px;">
                                                                            Active:
                                                                        </td>
                                                                        <td align="left" valign="middle" style="width:50px;">
                                                                            <asp:DropDownList ID="dlActive" runat="server" Width="50px" CssClass="comboBg">
                                                                            <asp:ListItem>Yes</asp:ListItem>
											                                <asp:ListItem>No</asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        </td>        
                                                                        <td align="left" class="gridnormal" valign="middle" style="width:50px;">
                                                                            Race:
                                                                        </td>
                                                                        <td align="left" valign="middle" style="width:150px;">
                                                                            <asp:DropDownList ID="dlRaceEdit" runat="server" Width="70px" CssClass="comboBg">
                                                                            </asp:DropDownList>
                                                                        </td>     
                                                                        <td align="left" class="gridnormal" valign="middle" style="width:250px;">
                                                                            &nbsp;
                                                                        </td>                                                                    
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="111">
                                                                            <FST:Profile runat="server" ID="knownProfile" LociPerRow="4" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    
                                                    <tr>
                                                        <td align="left">
                                                            <asp:LinkButton runat="server" ID="lnkDownload" OnClick="lnkDownload_Click">Download Population</asp:LinkButton>
                                                        </td>
                                                        <td align="right">
                                                            <asp:Label ID="lblId" runat="server" Visible="false" />
                                                            <asp:ImageButton ID="btnNew" Width="61" Height="21" runat="server" ImageUrl="~/Images/Buttons/New1.png" OnClientClick="this.src='../Images/Buttons/New2.png';"
                                                                ToolTip="New" OnClick="btnNew_Click" style="margin-top:5px;" />
                                                            <asp:ImageButton ID="btnSave" Width="61" Height="21" runat="server" ImageUrl="~/Images/Buttons/Save1.png" OnClientClick="this.src='../Images/Buttons/Save2.png';"
                                                                ToolTip="Save" OnClick="btnSave_Click" style="margin-top:5px;" />
                                                        </td>
                                                    </tr>
                                                </table>
                                                <div style="width:500px; margin-left:auto; margin-right:auto; margin-top:20px;">
                                                    <asp:Label runat="server" ID="lblUpload" Text="Upload Profiles" CssClass="gridnormal" />
                                                    <div style="position:absolute; margin-top:-25px;">
                                                        <img id="ImgUpload" style="cursor:arrow; margin-left:309px;" src="../Images/Buttons/Browse1.png" />
                                                    </div>
                                                    <asp:FileUpload 
                                                        ID="FileUpload1" runat="server" 
                                                        Width="303" CssClass="uploader" 
                                                        style="position:relative; cursor:hand; height:22px; background-color:#fff; border-color:#000; border-width:1px; -ms-filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0); filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);"
                                                        onchange="document.getElementById('txtUploadText').value = this.value;"
                                                        />
                                                    <div style="position:absolute; margin-top:-25px;">
                                                        <input id="txtUploadText" type="text" readonly style="margin-left:76px; background-color:#fff; border-color:#000; border-width:1px; width: 229px; position:absolute; margin-top:3px;" />
                                                    </div>
                                                    <asp:ImageButton 
                                                        ID="Upload" runat="server" 
                                                        Text="Upload" OnClick="Upload_Click" 
                                                        ImageUrl="~/Images/Buttons/Upload1.png" OnClientClick="this.src='../Images/Buttons/Upload2.png'" 
                                                        style="position:absolute; margin-top:-2px; margin-left:6px;"
                                                        />
                                                    <br />
                                                    <asp:Label runat="server" ID="lblUploadResult" CssClass="gridnormal"></asp:Label>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </contenttemplate>
                                <triggers>
                                    <asp:PostBackTrigger ControlID="lnkDownload" />
                                    <asp:PostBackTrigger ControlID="btnSave" />
                                    <asp:PostBackTrigger ControlID="Upload" />
                                </triggers>
                            </asp:updatepanel>
                            <!-------------------------------Content End------------------->
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</div>

        <script type="text/javascript">
            var images = new Array();
            var UploadButton = document.getElementById('<%=FileUpload1.ClientID %>');
            // the line below is not compatible with IE9+ and other browsers. sorry, whoever has to work on this...
            UploadButton.attachEvent('onclick', function () { btnInput_MouseDownJs('ImgUpload', '<%=FileUpload1.ClientID %>'); });
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
	            "../Images/Buttons/On2.png",
                "../Images/Buttons/Off2.png",
                "../Images/Buttons/Delete2.png",
                "../Images/Buttons/New2.png",
                "../Images/Buttons/Save2.png",
                "../Images/Buttons/Browse2.png",
                "../Images/Buttons/Upload2.png"
            );
        </script>
</asp:content>
