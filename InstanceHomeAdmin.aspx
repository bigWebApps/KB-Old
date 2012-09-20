<%@ Page Title="Instance Home" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="InstanceHomeAdmin.aspx.cs" Inherits="BWA.Knowledgebase.InstanceHomeAdmin"
    ValidateRequest="false" meta:resourcekey="PageResource1" %>
<%@ Register Src="Controls/SearchArticleControl.ascx" TagName="SearchArticleControl"
    TagPrefix="kbc" %>
<%@ Register Src="Controls/ArticlesTreeControl.ascx" TagName="ArticlesTreeControl"
    TagPrefix="kbc" %>
<%@ Register Src="~/Controls/PostCommentControl.ascx" TagName="PostCommentControl"
    TagPrefix="kbc" %>
<%@ MasterType TypeName="BWA.Knowledgebase.MasterPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageBody" runat="Server">
<%--    <asp:Label runat="server" ID="InstanseName" Font-Bold="True" Font-Size="Medium" 
        meta:resourcekey="InstanseNameResource1" />
    <br />
    <br />
    <kbc:SearchArticleControl ID="SearchArticleCtrl" runat="server" />--%>
    <br />
    <kbc:ArticlesTreeControl runat="server" ID="ArticlesTreeCtrl" IsAdmin="true" />
    <br />
    <%--<kbc:PostCommentControl ID="PostCommentCtrl" runat="server" OnCommentPosted="PostCommentCtrl_CommentPosted"
        Width="500px" />--%>
    <br />
</asp:Content>
