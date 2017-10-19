<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" Inherits="Admin_frmJobsStatus" Title="FST-JobsQueue" Codebehind="frmJobsStatus.aspx.cs" %>

<%@ Register assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.WebControls" tagprefix="asp" %>

<asp:Content ID="CntMain" ContentPlaceHolderID="MainContentHolder" runat="Server">
    <table width="996px">
        <tr>
            <td style="width: 400px">
                <asp:RadioButtonList ID="rbtnlStatus" runat="server" CellSpacing="2" RepeatDirection="Horizontal"
                    Width="452px" AutoPostBack="True" 
                    OnSelectedIndexChanged="rbtnlStatus_SelectedIndexChanged">
                    <asp:ListItem Selected="True">To Be Processed</asp:ListItem>
                    <asp:ListItem>Processing</asp:ListItem>
                    <asp:ListItem>Processed</asp:ListItem>
                    <asp:ListItem>Removed</asp:ListItem>
                </asp:RadioButtonList>          
            </td>
            <td>
                <asp:ImageButton ID="btnRefresh" runat="server" Text= "Refresh" onclick="imgbtnRefresh_Click" 
                    ToolTip="Refresh the list" ImageUrl="~/Images/Buttons/Refresh1.png" OnClientClick="this.src='../Images/Buttons/Refresh2.png';" />
            </td>
        </tr>
        <tr>
            <td style="width: 996px" colspan="2">
                <asp:Panel ID="pnlJobStatus" runat="server" ScrollBars="Both" Width="990px">
                     <asp:GridView ID="gvJobStatus" runat="server" BorderStyle="Solid" 
                         HorizontalAlign="Center" CellSpacing="1" Width="100%" 
                         onpageindexchanging="gvJobStatus_PageIndexChanging" 
                         AllowPaging="True" style="margin-bottom: 27px" Font-Size="Large" 
                         onrowdatabound="gvJobStatus_RowDataBound" PageSize="20" >
                        <Columns>
                            <asp:TemplateField ShowHeader ="false">
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnRemove" runat="server" OnClick="btnRemove_Click" CommandName="DeleteRow" CommandArgument='<%#Eval("RecordID") %>' ImageUrl="~/Admin/Images/Symbols-Delete-icon32.png" Text="Remove" Visible='<%#Eval("Processed").ToString()=="N" ? true:false%>' ToolTip="Stop the job to be processed." />  
                                    <asp:ImageButton ID="btnRestartJob" runat="server" OnClick="btnRestartJob_Click" CommandName="RefreshRow" CommandArgument='<%#Eval("RecordID") %>' ImageUrl="~/Admin/Images/refresh.png" Text="Fresh" Visible='<%#Eval("Processed").ToString()=="D" ? true:false%>' ToolTip="Restart the job to be processed" />
                                </ItemTemplate>
                            </asp:TemplateField>                                                     
                        </Columns>
                        <PagerSettings Mode="NextPreviousFirstLast" 
                             FirstPageImageUrl="~/Admin/Images/First.gif" 
                             LastPageImageUrl="~/Admin/Images/Last.gif" 
                             NextPageImageUrl="~/Admin/Images/Next.gif" Position="TopAndBottom" 
                             PreviousPageImageUrl="~/Admin/Images/Prev.gif" />
                        <RowStyle Wrap="False" CssClass="datagridRow1" BorderStyle="Solid" HorizontalAlign="Left" />                        
                        <EmptyDataRowStyle Wrap="False" />                       
                        <FooterStyle Wrap="False" />
                        <PagerStyle Wrap="False" HorizontalAlign="Justify" />
                        <SelectedRowStyle Wrap="False" />
                        <HeaderStyle CssClass="datagridRowHead" Wrap="False" />
                        <EditRowStyle Wrap="False" CssClass="comboBg" />
                        <AlternatingRowStyle CssClass="datagridRow2" Wrap="False" />    
                        
                    </asp:GridView>
                </asp:Panel>
            </td>
        </tr>
    </table>
    <script type="text/javascript">
        var images = new Array();
        function preload() {
            for (i = 0; i < preload.arguments.length; i++) {
                images[i] = new Image();
                images[i].src = preload.arguments[i];
            }
        }
        preload(
	        "../Images/Buttons/Refresh2.png"
        );
    </script>
</asp:Content>
