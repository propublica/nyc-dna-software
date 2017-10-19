<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="frmUpload.aspx.cs" Inherits="FST.Web.frmUpload" ValidateRequest="false" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<%@ Register Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentHolder" runat="server">
    <asp:ScriptManager ID="ScriptManager2" runat="server"> </asp:ScriptManager>
    <script language="javascript" src="Scripts/key.js"></script>
    <script type="text/javascript" language="javascript" src="Scripts/spin.min.js"></script>
    <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0">
        <tr>
            <td height="27" align="left" valign="middle" bgcolor="#FFFFFF">
                <ul id="crumbs">
                    <%--<li><a href="frmDefault.aspx">Home</a></li>--%>                   
                    <li style="width: auto"><asp:LinkButton ID="lblMenuLevel1" runat="server" Text="You Should Never See This"></asp:LinkButton></li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="left" valign="middle" bgcolor="#ededed">
                <asp:Table runat="server" ID="tblCaseData" width="100%" border="0" align="center" cellpadding="0" cellspacing="0">
                    <asp:TableRow>
                        <asp:TableCell height="37" align="left" valign="middle" bgcolor="#d4d9e1">
                            <table width="97%" border="0" align="center" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                  <table width="25%" border="0" cellspacing="0" cellpadding="0">
                                            <tr>
                                                <td class="gridheader">
                                                    <asp:Label ID="Label318" runat="server" Text="FB#1:"></asp:Label>
                                                </td>
                                                <td>&nbsp;
                                                    
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txFBNo" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>
                                        <table width="25%" border="0" cellspacing="0" cellpadding="0">
                                            <tr>
                                                <td class="gridheader">
                                                    <asp:Label ID="Label319" runat="server" Text="Comparison:"></asp:Label>
                                                </td>
                                                <td>&nbsp;
                                                    
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txSuspectNo" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>
                                        <table width="25%" border="0" cellspacing="0" cellpadding="0">
                                            <tr>
                                                <td class="gridheader">
                                                    <asp:Label ID="Label320" runat="server" Text="FB#2:"></asp:Label>
                                                </td>
                                                <td>&nbsp;
                                                    
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txFB2No" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>
                                        <table width="25%" border="0" cellspacing="0" cellpadding="0">
                                            <tr>
                                                <td class="gridheader">
                                                    <asp:Label ID="Label321" runat="server" Text="Item:"></asp:Label>
                                                </td>
                                                <td>&nbsp;
                                                    
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txItemNo" runat="server" value=''></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Panel ID="Panel1" runat="server" Width="100%">
                    <asp:Table runat="server" ID="tblFileUploads" align="left" cellpadding="5" style="width: 100%;">
                        <asp:TableRow ID="trComparison1">
                            <asp:TableCell align="right">
                                <div style="height: 13px;">
                                    &nbsp;</div>
                                <asp:Label ID="lblSusp1" runat="server" CssClass="bluetextBold" Text="Comparison Profile 1: "></asp:Label>

                            </asp:TableCell>
                            <asp:TableCell align="left">
                                <div style="height: 10px;">
                                    &nbsp;</div>
                                <div style="position: absolute; margin-top: -9px;">
                                    <img id="Img1" style="cursor: arrow; margin-left: 440px;" src="Images/Buttons/Browse1.png" />
                                </div>
                                <input id="txtFileSuspectInput" type="file" onclick="btnInput_MouseDownJs('Img1','<%=txtFileSuspectInput.ClientID %>');"
                                    onchange="document.getElementById('txtFileSuspectInputText').value = this.value; HideCompare();"
                                    runat="server" style="cursor: hand; height: 22px; background-color: #fff; border-color: #000;
                                    border-width: 1px; width: 515px; position: absolute; margin-top: -6px; -ms-filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);
                                    filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);" />
                                <!--class="uploader" -->
                                <div style="position: absolute; margin-top: -9px;">
                                    <input id="txtFileSuspectInputText" type="text" readonly style="background-color: #fff;
                                        border-color: #000; border-width: 1px; width: 433px; position: absolute; margin-top: 3px;" />
                                </div>
                            </asp:TableCell>
                            <asp:TableCell>
                                <div style="height: 10px;">
                                    &nbsp;</div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="trComparison2">
                            <asp:TableCell align="right">
                                <div style="height: 13px;">
                                    &nbsp;</div>
                                <asp:Label ID="lblSusp2" runat="server" CssClass="bluetextBold" Text="Comparison Profile 2: "></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell align="left">
                                <div style="position: absolute; margin-top: 5px;">
                                    <img id="Img2" style="cursor: arrow; margin-left: 440px;" src="Images/Buttons/Browse1.png" />
                                </div>
                                <input id="txtFileSuspect2Input" type="file" onclick="btnInput_MouseDownJs('Img2','<%=txtFileSuspect2Input.ClientID %>');"
                                    onchange="document.getElementById('txtFileSuspect2InputText').value = this.value; HideCompare();"
                                    runat="server" style="position: relative; cursor: hand; height: 22px; background-color: #fff;
                                    border-color: #000; border-width: 1px; width: 515px; margin-top: 8px; -ms-filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);
                                    filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);" />
                                <!--class="uploader" -->
                                <div style="position: absolute; margin-top: -24px;">
                                    <input id="txtFileSuspect2InputText" type="text" readonly style="background-color: #fff;
                                        border-color: #000; border-width: 1px; width: 433px; position: absolute; margin-top: 3px;" />
                                </div>
                            </asp:TableCell>
                            <asp:TableCell>
                                <div style="height: 10px;">
                                    &nbsp;</div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="trComparison3">
                            <asp:TableCell align="right">
                                <div style="height: 13px;">
                                    &nbsp;</div>
                                <asp:Label ID="lblSusp3" runat="server" CssClass="bluetextBold" Text="Comparison Profile 3: "></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell align="left">
                                <div style="position: absolute; margin-top: 5px;">
                                    <img id="Img3" style="cursor: arrow; margin-left: 440px;" src="Images/Buttons/Browse1.png" />
                                </div>
                                <input id="txtFileSuspect3Input" type="file" onclick="btnInput_MouseDownJs('Img3','<%=txtFileSuspect3Input.ClientID %>');"
                                    onchange="document.getElementById('txtFileSuspect3InputText').value = this.value; HideCompare();"
                                    runat="server" style="position: relative; cursor: hand; height: 22px; background-color: #fff;
                                    border-color: #000; border-width: 1px; width: 515px; margin-top: 8px; -ms-filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);
                                    filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);" />
                                <!--class="uploader" -->
                                <div style="position: absolute; margin-top: -24px;">
                                    <input id="txtFileSuspect3InputText" type="text" readonly style="background-color: #fff;
                                        border-color: #000; border-width: 1px; width: 433px; position: absolute; margin-top: 3px;" />
                                </div>
                            </asp:TableCell>
                            <asp:TableCell>
                                <div style="height: 10px;">
                                    &nbsp;</div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="trComparison4">
                            <asp:TableCell align="right">
                                <div style="height: 13px;">
                                    &nbsp;</div>
                                <asp:Label ID="lblSusp4" runat="server" CssClass="bluetextBold" Text="Comparison Profile 4: "></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell align="left">
                                <div style="position: absolute; margin-top: 5px;">
                                    <img id="Img4" style="cursor: arrow; margin-left: 440px;" src="Images/Buttons/Browse1.png" />
                                </div>
                                <input id="txtFileSuspect4Input" type="file" onclick="btnInput_MouseDownJs('Img4','<%=txtFileSuspect4Input.ClientID %>');"
                                    onchange="document.getElementById('txtFileSuspect4InputText').value = this.value; HideCompare();"
                                    runat="server" style="position: relative; cursor: hand; height: 22px; background-color: #fff;
                                    border-color: #000; border-width: 1px; width: 515px; margin-top: 8px; -ms-filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);
                                    filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);" />
                                <!--class="uploader" -->
                                <div style="position: absolute; margin-top: -24px;">
                                    <input id="txtFileSuspect4InputText" type="text" readonly style="background-color: #fff;
                                        border-color: #000; border-width: 1px; width: 433px; position: absolute; margin-top: 3px;" />
                                </div>
                            </asp:TableCell>
                            <asp:TableCell>
                                <div style="height: 10px;">
                                    &nbsp;</div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="trKnown1">
                            <asp:TableCell align="right">
                                <div style="height: 13px;">
                                    &nbsp;</div>
                                <asp:Label ID="lblSusp5" runat="server" CssClass="bluetextBold" Text="Known Profile 1: "></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell align="left">
                                <div style="position: absolute; margin-top: 5px;">
                                    <img id="Img5" style="cursor: arrow; margin-left: 440px;" src="Images/Buttons/Browse1.png" />
                                </div>
                                <input id="txtFileSuspect5Input" type="file" onclick="btnInput_MouseDownJs('Img5','<%=txtFileSuspect5Input.ClientID %>');"
                                    onchange="document.getElementById('txtFileSuspect5InputText').value = this.value; HideCompare();"
                                    runat="server" style="position: relative; cursor: hand; height: 22px; background-color: #fff;
                                    border-color: #000; border-width: 1px; width: 515px; margin-top: 8px; -ms-filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);
                                    filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);" />
                                <!--class="uploader" -->
                                <div style="position: absolute; margin-top: -24px;">
                                    <input id="txtFileSuspect5InputText" type="text" readonly style="background-color: #fff;
                                        border-color: #000; border-width: 1px; width: 433px; position: absolute; margin-top: 3px;" />
                                </div>
                            </asp:TableCell>
                            <asp:TableCell>
                                <div style="height: 10px;">
                                    &nbsp;</div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="trKnown2">
                            <asp:TableCell align="right">
                                <div style="height: 13px;">
                                    &nbsp;</div>
                                <asp:Label ID="lblSusp6" runat="server" CssClass="bluetextBold" Text="Known Profile 2: "></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell align="left">
                                <div style="position: absolute; margin-top: 5px;">
                                    <img id="Img6" style="cursor: arrow; margin-left: 440px;" src="Images/Buttons/Browse1.png" />
                                </div>
                                <input id="txtFileSuspect6Input" type="file" onclick="btnInput_MouseDownJs('Img6','<%=txtFileSuspect6Input.ClientID %>');"
                                    onchange="document.getElementById('txtFileSuspect6InputText').value = this.value; HideCompare();"
                                    runat="server" style="position: relative; cursor: hand; height: 22px; background-color: #fff;
                                    border-color: #000; border-width: 1px; width: 515px; margin-top: 8px; -ms-filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);
                                    filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);" />
                                <!--class="uploader" -->
                                <div style="position: absolute; margin-top: -24px;">
                                    <input id="txtFileSuspect6InputText" type="text" readonly style="background-color: #fff;
                                        border-color: #000; border-width: 1px; width: 433px; position: absolute; margin-top: 3px;" />
                                </div>
                            </asp:TableCell>
                            <asp:TableCell>
                                <div style="height: 10px;">
                                    &nbsp;</div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="trKnown3">
                            <asp:TableCell align="right">
                                <div style="height: 13px;">
                                    &nbsp;</div>
                                <asp:Label ID="lblSusp7" runat="server" CssClass="bluetextBold" Text="Known Profile 3: "></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell align="left">
                                <div style="position: absolute; margin-top: 5px;">
                                    <img id="Img7" style="cursor: arrow; margin-left: 440px;" src="Images/Buttons/Browse1.png" />
                                </div>
                                <input id="txtFileSuspect7Input" type="file" onclick="btnInput_MouseDownJs('Img7','<%=txtFileSuspect7Input.ClientID %>');"
                                    onchange="document.getElementById('txtFileSuspect7InputText').value = this.value; HideCompare();"
                                    runat="server" style="position: relative; cursor: hand; height: 22px; background-color: #fff;
                                    border-color: #000; border-width: 1px; width: 515px; margin-top: 8px; -ms-filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);
                                    filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);" />
                                <!--class="uploader" -->
                                <div style="position: absolute; margin-top: -24px;">
                                    <input id="txtFileSuspect7InputText" type="text" readonly style="background-color: #fff;
                                        border-color: #000; border-width: 1px; width: 433px; position: absolute; margin-top: 3px;" />
                                </div>
                            </asp:TableCell>
                            <asp:TableCell>
                                <div style="height: 10px;">
                                    &nbsp;</div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="trKnown4">
                            <asp:TableCell align="right">
                                <div style="height: 13px;">
                                    &nbsp;</div>
                                <asp:Label ID="lblSusp8" runat="server" CssClass="bluetextBold" Text="Known Profile 4: "></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell align="left">
                                <div style="position: absolute; margin-top: 5px;">
                                    <img id="Img8" style="cursor: arrow; margin-left: 440px;" src="Images/Buttons/Browse1.png" />
                                </div>
                                <input id="txtFileSuspect8Input" type="file" onclick="btnInput_MouseDownJs('Img8','<%=txtFileSuspect8Input.ClientID %>');"
                                    onchange="document.getElementById('txtFileSuspect8InputText').value = this.value; HideCompare();"
                                    runat="server" style="position: relative; cursor: hand; height: 22px; background-color: #fff;
                                    border-color: #000; border-width: 1px; width: 515px; margin-top: 8px; -ms-filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);
                                    filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);" />
                                <!--class="uploader" -->
                                <div style="position: absolute; margin-top: -24px;">
                                    <input id="txtFileSuspect8InputText" type="text" readonly style="background-color: #fff;
                                        border-color: #000; border-width: 1px; width: 433px; position: absolute; margin-top: 3px;" />
                                </div>
                            </asp:TableCell>
                            <asp:TableCell>
                                <div style="height: 10px;">
                                    &nbsp;</div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="trEvidence">
                            <asp:TableCell align="right">
                                <div style="height: 13px;">
                                    &nbsp;</div>
                                <asp:Label ID="lblEvidence" runat="server" CssClass="bluetextBold" Text="Evidence: "></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell align="left">
                                <div style="position: absolute; margin-top: 5px;">
                                    <img id="Img9" style="cursor: arrow; margin-left: 440px;" src="Images/Buttons/Browse1.png" />
                                </div>
                                <input id="txtFileUnknownInput" type="file" onclick="btnInput_MouseDownJs('Img9','<%=txtFileUnknownInput.ClientID %>');"
                                    onchange="document.getElementById('txtFileUnknownInputText').value = this.value; HideCompare();"
                                    runat="server" style="position: relative; cursor: hand; height: 22px; background-color: #fff;
                                    border-color: #000; border-width: 1px; width: 515px; margin-top: 8px; -ms-filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);
                                    filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);" />
                                <!--class="uploader" -->
                                <div style="position: absolute; margin-top: -24px;">
                                    <input id="txtFileUnknownInputText" type="text" readonly style="background-color: #fff;
                                        border-color: #000; border-width: 1px; width: 433px; position: absolute; margin-top: 3px;" />
                                </div>
                            </asp:TableCell>
                            <asp:TableCell>
                                <div style="height: 10px;">
                                    &nbsp;</div>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                    <table align="left" cellpadding="5" style="width: 100%;">
                        <tr>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td align="right" valign="middle">
                                            <asp:Label ID="lblDeducible" runat="server" Text="Deducible:" CssClass="graynormalText"></asp:Label>
                                        </td>
                                        <td align="left" valign="middle">
                                            <asp:DropDownList ID="dlDeducible" runat="server" CssClass="comboBg">
                                                <asp:ListItem>Yes</asp:ListItem>
                                                <asp:ListItem>No</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="right" valign="middle">
                                            <asp:Label ID="lblTypes" runat="server" Text="Types:" CssClass="graynormalText"></asp:Label>
                                        </td>
                                        <td align="left" valign="middle">
                                            <asp:DropDownList ID="dlTypes" runat="server" CssClass="comboBg" onchange="HideCompare(); return ddlTypes_OnChange();">
                                                <asp:ListItem>Lab Types</asp:ListItem>
                                                <asp:ListItem>Population</asp:ListItem>
                                                <asp:ListItem>From File</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" valign="middle">
                                            <asp:Panel ID="pnlPopulationUpload" runat="server" Style="width: auto; margin-left: 3px;
                                                margin-right: 3px; margin-top: auto; visibility: collapse;">
                                                <asp:Label runat="server" ID="lblPopulationUpload" Text="File: " CssClass="graynormalText" />
                                                <div style="position: absolute; margin-top: -4px;">
                                                    <img id="ImgUp" style="cursor: arrow; margin-left: 180px; margin-top: -25px;" src="Images/Buttons/Browse1.png" />
                                                </div>
                                                <asp:FileUpload ID="fuPopulationUpload" runat="server" Height="22" Width="224" CssClass="uploader"
                                                    onchange="HideCompare(this);" Style="position: relative; cursor: hand; -ms-filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);
                                                    filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);" />
                                                <div style="position: absolute; margin-top: -4px;">
                                                    <input id="fuPopulationUploadText" type="text" readonly style="margin-left: 26px;
                                                        background-color: #fff; border-color: #000; border-width: 1px; width: 148px;
                                                        position: absolute; margin-top: -17px;" />
                                                </div>
                                                <br />
                                                <asp:Label runat="server" ID="lblUploadResult"></asp:Label>
                                            </asp:Panel>
                                        </td>
                                        <td align="right" valign="middle">
                                            <asp:Label ID="lblDropOutOption" runat="server" Text="DNA Template Amount (pg):"
                                                CssClass="graynormalText"></asp:Label>
                                        </td>
                                        <td align="left" valign="middle">
                                            <asp:TextBox ID="txDropout" runat="server" CssClass="textboxBg" Width="100px"></asp:TextBox>
                                        </td>
                                        <td valign="middle">
                                            <div>
                                                <asp:ImageButton ID="btnLoadData" runat="server" Text="Preview" ImageUrl="~/Images/Buttons/Preview1.png"
                                                    OnClientClick="return CheckAllFilesSelected(this);" OnClick="btnLoadData_Click" />
                                                <asp:ImageButton ID="btnRead" runat="server" OnClick="btnRead_Click" ImageUrl="~/Images/Buttons/Compare1.png"
                                                    OnClientClick="btnCompare_ClickJs(this);" Text="Calculate" Visible="False" />
                                                <asp:ImageButton ID="btnEdit" runat="server" OnClick="btnEdit_Click" ImageUrl="~/Images/Buttons/Edit1.png"
                                                    OnClientClick="this.src='Images/Buttons/Edit2.png'" Text="Back" />
                                            </div>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
                <div id="showAlleles">
                    <asp:Panel ID="Panel2" runat="server" Width="80%">
                        <br />
                        <table style="width: 100%;">
                            <tr>
                                <td>
                                </td>
                                <td>
                                    <asp:Label ID="lblKnownFileName" runat="server" CssClass="graynormalText"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="lblSuspect" runat="server" Text="" CssClass="graynormalText"></asp:Label>
                                </td>
                                <td>
                                    <asp:Panel ID="pnlSuspect" runat="server" Width="920" ScrollBars="Horizontal" Visible="false">
                                        <asp:GridView ID="gvSuspect" Width="100%" BorderWidth="1px" BorderColor="#B7BABC"
                                            BorderStyle="Solid" runat="server">
                                            <RowStyle CssClass="datagridRow1" />
                                            <SelectedRowStyle CssClass="comboBg" />
                                            <HeaderStyle Height="20" CssClass="datagridRowHead" HorizontalAlign="Center" />
                                            <EditRowStyle CssClass="comboBg" />
                                            <AlternatingRowStyle CssClass="datagridRow2" />
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="lblSuspect2" runat="server" CssClass="graynormalText" Text=""></asp:Label>
                                </td>
                                <td>
                                    <asp:Panel ID="pnlSuspect2" runat="server" Width="920" ScrollBars="Horizontal" Visible="false">
                                        <asp:GridView ID="gvSuspect2" Width="100%" CssClass="SelectDataGrid" runat="server"
                                            BorderWidth="1px" BorderColor="#B7BABC" BorderStyle="Solid">
                                            <RowStyle CssClass="datagridRow1" />
                                            <SelectedRowStyle CssClass="comboBg" />
                                            <HeaderStyle Height="20" CssClass="datagridRowHead" HorizontalAlign="Center" />
                                            <EditRowStyle CssClass="comboBg" />
                                            <AlternatingRowStyle CssClass="datagridRow2" />
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="lblSuspect3" runat="server" CssClass="graynormalText" Text=""></asp:Label>
                                </td>
                                <td>
                                    <asp:Panel ID="pnlSuspect3" runat="server" Width="920" ScrollBars="Horizontal" Visible="false">
                                        <asp:GridView ID="gvSuspect3" Width="100%" CssClass="SelectDataGrid" runat="server"
                                            BorderWidth="1px" BorderColor="#B7BABC" BorderStyle="Solid">
                                            <RowStyle CssClass="datagridRow1" />
                                            <SelectedRowStyle CssClass="comboBg" />
                                            <HeaderStyle Height="20" CssClass="datagridRowHead" HorizontalAlign="Center" />
                                            <EditRowStyle CssClass="comboBg" />
                                            <AlternatingRowStyle CssClass="datagridRow2" />
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="lblSuspect4" runat="server" CssClass="graynormalText" Text=""></asp:Label>
                                </td>
                                <td>
                                    <asp:Panel ID="pnlSuspect4" runat="server" Width="920" ScrollBars="Horizontal" Visible="false">
                                        <asp:GridView ID="gvSuspect4" Width="100%" CssClass="SelectDataGrid" runat="server"
                                            BorderWidth="1px" BorderColor="#B7BABC" BorderStyle="Solid">
                                            <RowStyle CssClass="datagridRow1" />
                                            <SelectedRowStyle CssClass="comboBg" />
                                            <HeaderStyle Height="20" CssClass="datagridRowHead" HorizontalAlign="Center" />
                                            <EditRowStyle CssClass="comboBg" />
                                            <AlternatingRowStyle CssClass="datagridRow2" />
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="lblSuspect5" runat="server" CssClass="graynormalText" Text=""></asp:Label>
                                </td>
                                <td>
                                    <asp:Panel ID="pnlSuspect5" runat="server" Width="920" ScrollBars="Horizontal" Visible="false">
                                        <asp:GridView ID="gvSuspect5" Width="100%" CssClass="SelectDataGrid" runat="server"
                                            BorderWidth="1px" BorderColor="#B7BABC" BorderStyle="Solid">
                                            <RowStyle CssClass="datagridRow1" />
                                            <SelectedRowStyle CssClass="comboBg" />
                                            <HeaderStyle Height="20" CssClass="datagridRowHead" HorizontalAlign="Center" />
                                            <EditRowStyle CssClass="comboBg" />
                                            <AlternatingRowStyle CssClass="datagridRow2" />
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="lblSuspect6" runat="server" CssClass="graynormalText" Text=""></asp:Label>
                                </td>
                                <td>
                                    <asp:Panel ID="pnlSuspect6" runat="server" Width="920" ScrollBars="Horizontal" Visible="false">
                                        <asp:GridView ID="gvSuspect6" Width="100%" CssClass="SelectDataGrid" runat="server"
                                            BorderWidth="1px" BorderColor="#B7BABC" BorderStyle="Solid">
                                            <RowStyle CssClass="datagridRow1" />
                                            <SelectedRowStyle CssClass="comboBg" />
                                            <HeaderStyle Height="20" CssClass="datagridRowHead" HorizontalAlign="Center" />
                                            <EditRowStyle CssClass="comboBg" />
                                            <AlternatingRowStyle CssClass="datagridRow2" />
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="lblSuspect7" runat="server" CssClass="graynormalText" Text=""></asp:Label>
                                </td>
                                <td>
                                    <asp:Panel ID="pnlSuspect7" runat="server" Width="920" ScrollBars="Horizontal" Visible="false">
                                        <asp:GridView ID="gvSuspect7" Width="100%" CssClass="SelectDataGrid" runat="server"
                                            BorderWidth="1px" BorderColor="#B7BABC" BorderStyle="Solid">
                                            <RowStyle CssClass="datagridRow1" />
                                            <SelectedRowStyle CssClass="comboBg" />
                                            <HeaderStyle Height="20" CssClass="datagridRowHead" HorizontalAlign="Center" />
                                            <EditRowStyle CssClass="comboBg" />
                                            <AlternatingRowStyle CssClass="datagridRow2" />
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="lblSuspect8" runat="server" CssClass="graynormalText" Text=""></asp:Label>
                                </td>
                                <td>
                                    <asp:Panel ID="pnlSuspect8" runat="server" Width="920" ScrollBars="Horizontal" Visible="false">
                                        <asp:GridView ID="gvSuspect8" Width="100%" CssClass="SelectDataGrid" runat="server"
                                            BorderWidth="1px" BorderColor="#B7BABC" BorderStyle="Solid">
                                            <RowStyle CssClass="datagridRow1" />
                                            <SelectedRowStyle CssClass="comboBg" />
                                            <HeaderStyle Height="20" CssClass="datagridRowHead" HorizontalAlign="Center" />
                                            <EditRowStyle CssClass="comboBg" />
                                            <AlternatingRowStyle CssClass="datagridRow2" />
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="lblUnknown" runat="server" Text="" CssClass="graynormalText"></asp:Label>
                                </td>
                                <td>
                                    <asp:Panel ID="pnlUnknown" runat="server" Width="920" ScrollBars="Horizontal" Visible="false">
                                        <asp:GridView ID="gvUnknown" Width="100%" runat="server" CellPadding="4" CssClass="SelectDataGrid"
                                            BorderWidth="1px" BorderColor="#B7BABC" BorderStyle="Solid">
                                            <RowStyle CssClass="datagridRow1" />
                                            <SelectedRowStyle CssClass="comboBg" />
                                            <HeaderStyle Height="20" CssClass="datagridRowHead" HorizontalAlign="Center" />
                                            <EditRowStyle CssClass="comboBg" />
                                            <AlternatingRowStyle CssClass="datagridRow2" />
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </div>
            </td>
        </tr>
    </table>
     <script language="javascript" type="text/javascript">
        ddlTypes_OnChange();
        var fuPopulationUploadButton = document.getElementById('<%=fuPopulationUpload.ClientID %>');
        // <=ie8
        if(fuPopulationUploadButton != null) fuPopulationUploadButton.attachEvent('onclick', function () { btnInput_MouseDownJs('ImgUp', '<%=btnLoadData.ClientID %>'); });
        // >ie8
        if(fuPopulationUploadButton != null) fuPopulationUploadButton.attachEvent('click', function () { btnInput_MouseDownJs('ImgUp', '<%=btnLoadData.ClientID %>'); });
        function ddlTypes_OnChange() {
            <%=dlTypes.Visible ? "" : "return;" %>
            var fuPanel = document.getElementById("<%=pnlPopulationUpload.ClientID %>");
            var ddlTypeList = document.getElementById("<%=dlTypes.Visible ? dlTypes.ClientID : btnLoadData.ClientID %>");
            if (ddlTypeList.value == "From File")
                fuPanel.style.visibility = "visible";
            else
                fuPanel.style.visibility = "hidden";
        }

        function HideCompare(btn) {
            if (btn != null) document.getElementById('fuPopulationUploadText').value = btn.value;
            var btnRd = document.getElementById("<%=btnRead.ClientID %>");
            if (btnRd != null)
                btnRd.style.visibility = "hidden";
        }

        function CheckAllFilesSelected(btn) {
            var inputs, index;
            btn.src = 'Images/Buttons/Preview2.png'
            inputs = document.getElementsByTagName('input');
            for (index = 0; index < inputs.length; ++index) {
                if (inputs[index].type == "file" && inputs[index].value == "")
                    if (inputs[index].style.visibility != "hidden" && inputs[index].parentNode.style.visibility != "hidden") {
                        alert("Please choose all files before clicking preview.");
                        btn.src = 'Images/Buttons/Preview1.png'
                        return false;
                    }
            }
            return true;
        }

        function btnInput_MouseDownJs(imgId, upload) {
            var img = document.getElementById(imgId);
            img.src = 'Images/Buttons/Browse2.png';
            window.setTimeout(function () {
                img.src = 'Images/Buttons/Browse1.png';
            }, 250);
        }
        function btnCompare_ClickJs(btn) {
            btn.src = 'Images/Buttons/Compare2.png'
            window.setTimeout(function () {
                btn.src = 'Images/Buttons/Compare1.png';
            }, 250);

            var spinningIndicator = document.getElementById('divSpinningIndicator');
            spinningIndicator.style.visibility = 'visible';
            spinningIndicator.style.zIndex = 1;
            var txtTarget = document.getElementById('<%= txSpinningIndicatorGuid.ClientID %>');
            var spinningIndicatorStopGuid = guid();
            txtTarget.value = spinningIndicatorStopGuid;
            var intervalId = window.setInterval(function () {
                var cookieValue = getCookie('ComparisonDone');
                if (cookieValue == spinningIndicatorStopGuid) {
                    var spinningIndicator2 = document.getElementById('divSpinningIndicator');
                    spinningIndicator2.style.visibility = 'hidden';
                    spinningIndicator2.style.zIndex = -1;
                    window.clearInterval(intervalId);
                }
            }, 1000);
        }
        function getCookie(c_name) {
            var i, x, y, ARRcookies = document.cookie.split(";");
            for (i = 0; i < ARRcookies.length; i++) {
                x = ARRcookies[i].substr(0, ARRcookies[i].indexOf("="));
                y = ARRcookies[i].substr(ARRcookies[i].indexOf("=") + 1);
                x = x.replace(/^\s+|\s+$/g, "");
                if (x == c_name) {
                    return unescape(y);
                }
            }
        }
        var images = new Array();
        function preload() {
            for (i = 0; i < preload.arguments.length; i++) {
                images[i] = new Image();
                images[i].src = preload.arguments[i];
            }
        }
        preload(
	"Images/Buttons/Compare2.png",
	"Images/Buttons/Preview2.png",
    "Images/Buttons/Edit2.png",
    "Images/Buttons/Browse2.png"
);
    </script>
    <div id="divSpinningIndicator" style="border-width: 1px; border-color: #000; border-style: solid;
        height: 100px; width: 320px; position: absolute; top: 280px; left: 38%; background: #fff;
        visibility: visible; z-index: -1;">
        <div style="margin-top: 44px; font-family: Verdana; font-size: 12px;">
            <asp:TextBox ID="txSpinningIndicatorGuid" runat="server" Style="visibility: hidden;
                float: right" Width="1"></asp:TextBox>
            <div id="imgSpinningIndicator" style="float: left; margin-left: 20px; margin-right: 20px;
                margin-top: -13px; height: 40px; width: 40px;">
                &nbsp;</div>
            Downloading Report... Please Wait.
        </div>
        <script language="javascript" type="text/javascript">
            // borrowed from stackoverflow
            function s4() {
                return Math.floor((1 + Math.random()) * 0x10000)
             .toString(16)
             .substring(1);
            };
            function guid() {
                return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
            }
            var opts = {
                lines: 13, // The number of lines to draw
                length: 13, // The length of each line
                width: 4, // The line thickness
                radius: 1, // The radius of the inner circle
                corners: 1, // Corner roundness (0..1)
                rotate: 0, // The rotation offset
                color: '#09c', // #rgb or #rrggbb
                speed: 1.6, // Rounds per second
                trail: 100, // Afterglow percentage
                shadow: false, // Whether to render a shadow
                hwaccel: false, // Whether to use hardware acceleration
                className: 'spinner', // The CSS class to assign to the spinner
                zIndex: 2e9, // The z-index (defaults to 2000000000)
                top: 'auto', // Top position relative to parent in px
                left: 'auto' // Left position relative to parent in px
            };
            var target = document.getElementById('imgSpinningIndicator');
            var spinner = new Spinner(opts).spin(target);
        </script>
    </div>
</asp:Content>
