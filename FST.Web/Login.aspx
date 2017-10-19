<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="FST.Web.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Forensic Statistical Tool</title>
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link type="text/css" href="Style/ui-lightness/jquery-ui-1.7.2.custom.css" rel="stylesheet" />
    <link href="Style/ui-lightness/style_uvis.css" rel="stylesheet" type="text/css" />
    <link href="Style/style.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="<%=ImagePath%>Scripts/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" src="<%=ImagePath%>Scripts/jquery-ui-1.7.2.custom.min.js"></script>
    <style type="text/css">
        div.imageSub
        {
            position: relative;
        }
        
        div.imageSub img
        {
            z-index: 1;
        }
        
        div.imageSub div
        {
            position: absolute;
            left: 15%;
            right: 15%;
            bottom: 0;
            padding: 4px;
            height: 16px;
            line-height: 16px;
            text-align: right;
            overflow: hidden;
        }
        
        div.imageSub div.label
        {
            z-index: 3;
            color: black;
            top: 28px;
            right: 8px;
        }
        .lang
        {
            font-family: Arial, Helvetica, Sans-Serif;
            font-size: 13.3px;
            line-height:16px;
        }
        .footerText{
	        font-family: Arial, Verdana, Helvetica;
	        font-size: 8pt;
	        color: #666666;
	        font-weight: normal;
	        text-decoration: none;
        }
        .textBox
        {
            width:169px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <table width="996px" border="0" align="center" cellpadding="0" cellspacing="0" bgcolor="#f5f5f5">
        <tr>
            <td>
                <div class="imageSub" style="width: 996px;">
                    <img alt="FST-Banner" src="<%=ImagePath%>Images/banner.gif" width="996px" height="96px" />
                    <div class="label" ondblclick="nn()">
                        v<%=FST_VERSION %></div>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <img src="<%=ImagePath%>Images/spacer.gif" alt="Spacer" width="996px" height="50px" />
            </td>
        </tr>
        <tr>
            <td bgcolor="#f5f5f5">
                <table class="lang">
                    <tr>
                        <td rowspan="4" style="width:350px;"></td>
                        <td>
                            <label>Username:</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txbUserName" class="textBox" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label>Password:</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txbPassWord" runat="server" TextMode="Password" class="textBox"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                            <asp:Button runat="server" Text="Login" ID="btnLogin" OnClick="btnLogin_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lblConfirmation" runat="server" Visible="false" ForeColor="Maroon"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td bgcolor="#eaeaea">
                <table>
                    <tr><td colspan="2" style="width:100px"></td>
                        <td style="width:800px">
		                    <div class="lang">
                                The Forensic Statistical Tool has been validated to be used on the following sample situations:
                            </div>
	                        <ul class="lang">
		                        <li>Samples amplified with more than 100pg of total DNA with Identifiler 28 cycles</li>
		                        <li>Samples amplified with less than 100pg of total DNA with Identifiler 31 cycles</li>
		                        <li>Single source samples, two-person mixtures in which one or more components were not deconvoluted, or three-person mixtures in which two or more components were not deconvoluted</li>
		                        <li>1 - 3 replicates of different amplifications</li>
	                        </ul>
                            <div  class="lang">
		                        Calculations can be done on a sample that is deemed to be "deducible" or "non-deducible" and can be calculated with up to two assumed contributors (or "knowns"). The number of contributors must be the same for the numerator and denominator.  
                            </div>
	                        <br />
                            <div  class="lang">
		                        Any other set of scenarios was <strong>not validated</strong> and therefore <strong>will not give reliable results</strong>. Please contact a supervisor or Technical Leader before proceeding with this analysis if you have any questions. By logging in, you acknowledge these specific parameters.
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <%--****************** Footer Start**************--%>
        <tr>
            <td bgcolor="#eaeaea">
                <img src="<%=ImagePath%>Images/spacer.gif" alt="Spacer" width="1" height="3" />
            </td>
        </tr>
        <tr>
            <td align="center" valign="middle" bgcolor="#eaeaea" class="footerText">
                © <script type="text/javascript">document.write(new Date().getFullYear())</script> Office Of Chief Medical Examiner. The City Of New York. All Rights Reserved.
            </td>
        </tr>
        <tr>
            <td align="center" valign="middle" bgcolor="#eaeaea">
                <img src="<%=ImagePath%>Images/spacer.gif" alt="Spacer" width="1" height="3" />
            </td>
        </tr>
        <%--****************** Footer End**************--%>
    </table>
    </form>
</body>
</html>
