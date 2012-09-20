<%@ Page Title="Search Result" Language="C#" MasterPageFile="~/MasterPage.master"
    AutoEventWireup="true" CodeFile="SearchResult.aspx.cs" Inherits="BWA.Knowledgebase.SearchResult"
    meta:resourcekey="PageResource1" %>

<%@ Register Src="Controls/SearchResultControl.ascx" TagName="SearchResultControl"
    TagPrefix="uc1" %>
<%@ MasterType TypeName="BWA.Knowledgebase.MasterPage" %>
<asp:Content ID="Content2" ContentPlaceHolderID="PageBody" runat="Server">
    <div id="nonFooter">
        <div id="header">
            <div id="headertitle">
                <asp:HyperLink runat="server" ID="TitleLink" Target="_blank" />
            </div>
            <div id="search" style="white-space: nowrap">
                <input type="text" name="search" size="40" />
                <input type="submit" value="Search" />
            </div>
        </div>
    </div>
    <div id="pageInfo">
        <br />
        <asp:Literal runat="server" ID="breadcrumbsLiteral" />
        <br />
        <br />
        <uc1:SearchResultControl ID="SearchResultControl1" runat="server" />
    </div>
</asp:Content>
