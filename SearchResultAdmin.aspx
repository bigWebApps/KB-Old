<%@ Page Title="Search Result" Language="C#" MasterPageFile="~/MasterPage.master"
    AutoEventWireup="true" CodeFile="SearchResultAdmin.aspx.cs" Inherits="BWA.Knowledgebase.SearchResultAdmin"
    meta:resourcekey="PageResource1" %>

<%@ Register Src="Controls/SearchResultControl.ascx" TagName="SearchResultControl"
    TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="PageBody" runat="Server">
    <uc1:SearchResultControl ID="SearchResultControl1" runat="server" />
</asp:Content>
