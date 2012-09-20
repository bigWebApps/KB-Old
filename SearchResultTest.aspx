<%@ Page Title="Search Result" Language="C#" MasterPageFile="~/MasterPage.master"
    AutoEventWireup="true" CodeFile="SearchResultTest.aspx.cs" Inherits="BWA.Knowledgebase.SearchResult"
    meta:resourcekey="PageResource1" %>

<%@ Register Src="Controls/SearchResultControl.ascx" TagName="SearchResultControl"
    TagPrefix="uc1" %>
<%@ MasterType TypeName="BWA.Knowledgebase.MasterPage" %>
<asp:Content ID="Content2" ContentPlaceHolderID="PageBody" runat="Server">
    <input type="hidden" name="cp" value="1251" />
    <input type="hidden" name="FORM" value="FREESS" />
    <table bgcolor="#FFFFFF">
        <tr>
            <td>
                <input type="text" name="q" size="30" />
                <input type="submit" value="Search Site" />
                <asp:Literal runat="server" ID="siteDomain" />                                
            </td>
        </tr>
    </table>
</asp:Content>
