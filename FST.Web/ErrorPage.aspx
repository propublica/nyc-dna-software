<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" Inherits="ErrorPage" Codebehind="ErrorPage.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentHolder" Runat="Server">
    <div style="font-size:larger; width=700px; margin-top:20px; margin-bottom:20px; text-align:center" class="bluetextBold">
        FST Has Encountered an Error
    </div>
    <div class="graynormalText" style="width:700px; margin-left:auto; margin-right:auto">
        You are seeing this page because FST has encountered an error. If this is not your first time encountering this error, please report steps on how to reproduce it to the application administrator. 
        Otherwise, you can click one of the menu options above to return to the main page.
    </div>
    <div class="graynormalText" style="width:70px; margin-left:auto; margin-right:auto; margin-top:20px;">
        <a href="frmDefault.aspx">Home</a>
    </div>
</asp:Content>

