<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeBehind="frmManualEntry.aspx.cs" Inherits="FST.Web.frmManualEntry" %>

<%@ Register TagPrefix="FST" TagName="Evidence" Src="~/Controls/Evidence.ascx"%>
<%@ Register TagPrefix="FST" TagName="Profile" Src="~/Controls/Profile.ascx"%>

<%@ Register Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContentHolder" runat="server">
    <script type="text/javascript">
        $(function () {
            $("#accordion").accordion({
                icons: {
                    header: "ui-icon-circle-arrow-e",
                    headerSelected: "ui-icon-circle-arrow-s"
                }
            });
        });
	</script>
    <script type="text/javascript" language="javascript" src="Scripts/spin.min.js"></script>
    <asp:ScriptManager ID="ScriptManager2" runat="server"> </asp:ScriptManager>
        <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0">
        <tr>
            <td height="27" align="left" valign="middle" bgcolor="#FFFFFF">
                <ul id="crumbs">
                    <%--<li><a href="frmDefault.aspx">Home</a></li>--%>
                    <li style="width:auto"><asp:LinkButton ID="lblMenuLevel1" runat="server" Text="You Should Never See This" OnClick="btnBack_Click" ></asp:LinkButton></li>
                    <li style="width:auto"><asp:LinkButton ID="lblMenuLevel2" runat="server" Text="You Should Never See This"></asp:LinkButton></li>
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
                                                    <asp:TextBox ID="txItemNo" runat="server"></asp:TextBox>
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
            <td align="left" valign="top" bgcolor="#ededed">
                <div style="width:995px; padding-left: 3px;">
                    <div class="demo">
                        <div id="accordion">
                            
                            <asp:Literal runat="server" ID="lblEvidence" />
                            <FST:Evidence runat="server" ID="eviAlleles" />

                            <asp:Literal runat="server" ID="lblComparison1" />
                            <FST:Profile runat="server" ID="proComparison1Alleles" LociPerRow="5" />

                            <asp:Literal runat="server" ID="lblComparison2" />
                            <FST:Profile runat="server" ID="proComparison2Alleles" LociPerRow="5" />

                            <asp:Literal runat="server" ID="lblComparison3" />
                            <FST:Profile runat="server" ID="proComparison3Alleles" LociPerRow="5" />

                            <asp:Literal runat="server" ID="lblComparison4" />
                            <FST:Profile runat="server" ID="proComparison4Alleles" LociPerRow="5" />

                            <asp:Literal runat="server" ID="lblKnown1" />
                            <FST:Profile runat="server" ID="proKnown1Alleles" LociPerRow="5" />

                            <asp:Literal runat="server" ID="lblKnown2" />
                            <FST:Profile runat="server" ID="proKnown2Alleles" LociPerRow="5" />

                            <asp:Literal runat="server" ID="lblKnown3" />
                            <FST:Profile runat="server" ID="proKnown3Alleles" LociPerRow="5" />

                            <asp:Literal runat="server" ID="lblKnown4" />
                            <FST:Profile runat="server" ID="proKnown4Alleles" LociPerRow="5" />

                        </div>
                    </div>
                </div>
            </td>
        </tr>        
        <tr>
            <td bgcolor="#eaeaea">
                <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0">
                    <tr>
                        <td height="47" align="left" valign="middle" bgcolor="#d4d9e1">
                            <table border="0" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td align="left" valign="middle">
                                        <img src="images/spacer.gif" width="10" height="3" />
                                    </td>
                                    <td align="left">
                                        <asp:Label ID="lblDeducible" runat="server" Text="Deducible:" CssClass="graynormalText"></asp:Label>
                                    </td>
                                    <td align="left" valign="middle">
                                        <asp:DropDownList ID="dlDeducible" runat="server" CssClass="comboBg">
                                        <asp:ListItem>Yes</asp:ListItem>
                                        <asp:ListItem>No</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td align="left" valign="middle">&nbsp;                                        
                                    </td>
                                    <td align="left">
                                        <asp:Label ID="lblTypes" runat="server" Text="Types:" CssClass="graynormalText" Visible="false"></asp:Label>
                                    </td>
                                    <td align="left" valign="middle">
                                        <asp:DropDownList ID="dlTypes" runat="server" CssClass="comboBg" AutoPostBack="true" OnSelectedIndexChanged="dlTypes_SelectedIndexChanged" Visible="false">
                                        <asp:ListItem>Lab Types</asp:ListItem>
                                        <asp:ListItem>Population</asp:ListItem>
                                        <asp:ListItem>From File</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td align="left" valign="middle">
                                        <asp:Panel ID="pnlPopulationUpload" runat="server" style="width:auto; margin-left:3px; margin-right:3px; margin-top:auto;" Visible="false">
                                            <asp:Label runat="server" ID="lblPopulationUpload" Text="File:" CssClass="graynormalText" />
                                            <div style="position:absolute; margin-top:-4px;">
                                                <img id="ImgUp" style="cursor:arrow; margin-left:180px; margin-top:-25px;" src="Images/Buttons/Browse1.png" />
                                            </div>
                                            <asp:FileUpload ID="fuPopulationUpload" runat="server" Height="22" Width="224" CssClass="uploader" onchange="document.getElementById('fuPopulationUploadText').value = this.value;" style="position:relative; cursor:hand; -ms-filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0); filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=0);" />
                                            <div style="position:absolute; margin-top:-4px;">
                                                <input id="fuPopulationUploadText" type="text" readonly style="margin-left:26px; background-color:#fff; border-color:#000; border-width:1px; width:148px; position:absolute; margin-top:-17px;" />
                                            </div>
                                            <br />
                                            <asp:Label runat="server" ID="lblUploadResult"></asp:Label>
                                        </asp:Panel>
                                    </td>
                                    <td align="left">
                                        <asp:Label ID="lblDropOutOption" runat="server" Text="DNA Template Amount (pg):" CssClass="graynormalText"></asp:Label>
                                    </td>
                                    <td align="left" valign="middle">
                                        <asp:TextBox ID="txDropout" runat="server" CssClass="textboxBg" Width="100px"></asp:TextBox>
                                    </td>
                                    <td align="left" valign="top">&nbsp;
                                        
                                    </td>
                                    <td width="72" align="left" valign="top">
                                        <asp:ImageButton ID="btnCompare" runat="server" Text="Compare" ImageUrl="Images/Buttons/Compare1.png" OnClientClick="btnCompareClick(this)"
                                            OnClick="btnCompare_Click" />
                                    </td>
                                    <td align="left" valign="top">&nbsp;
                                        
                                    </td>
                                    <td width="58" align="left" valign="top">
                                        <asp:ImageButton ID="btnReset" runat="server" Text="Reset" ImageUrl="Images/Buttons/Reset1.png" OnClientClick="this.src='Images/Buttons/Reset2.png'"
                                            OnClick="btnReset_Click" />
                                    </td>
                                    <td align="left" valign="top">&nbsp;
                                        
                                    </td>
                                    <td align="left" valign="top">&nbsp;
                                        
                                    </td>
                                    <td width="61" align="left" valign="top">
                                        <asp:ImageButton ID="btnBack" runat="server" OnClick="btnBack_Click"
                                            Text="Back" ImageUrl="Images/Buttons/Back1.png" OnClientClick="this.src='Images/Buttons/Back2.png'" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <div style="display:none; ">
        <script type="text/javascript">
            var fuPopulationUploadButton = document.getElementById('<%=fuPopulationUpload.ClientID %>');
            // the line below is not compatible with IE9+ and other browsers. sorry, whoever has to work on this...
            if (fuPopulationUploadButton != null) fuPopulationUploadButton.attachEvent('onclick', function () { btnInput_MouseDownJs('ImgUp', '<%=fuPopulationUpload.ClientID %>'); });
            function btnInput_MouseDownJs(imgId, upload) {
                var img = document.getElementById(imgId);
                img.src = 'Images/Buttons/Browse2.png';
                window.setTimeout(function () {
                    img.src = 'Images/Buttons/Browse1.png';
                }, 250);
            }
            function btnCompareClick(img) {
                img.src = 'Images/Buttons/Compare2.png';
                window.setTimeout(
                    function () {
                        img.src = 'Images/Buttons/Compare1.png';
                    },
                250);

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
	            "Images/Buttons/Reset2.png",
                "Images/Buttons/Back2.png"
            );
        </script>
    </div>     
    <div id="divSpinningIndicator" style="border-width:1px; border-color:#000; border-style:solid; height:100px; width:320px; position:absolute; top: 280px; left:38%; background:#fff; visibility:visible; z-index:-1;">
        <div style="margin-top:44px; font-family:Verdana; font-size:12px;">
            <asp:TextBox ID="txSpinningIndicatorGuid" runat="server" style="visibility:hidden; float:right" Width="1" ></asp:TextBox>
            <div id="imgSpinningIndicator" style="float:left; margin-left:20px; margin-right:20px; margin-top:-13px; height: 40px; width:40px;">&nbsp;</div>
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

