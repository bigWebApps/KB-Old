<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Default2.aspx.cs" Inherits="BWA.Knowledgebase._Default2" Async="true" %>

<%@ Register Src="~/Controls/SearchArticleControl.ascx" TagName="SearchArticleControl"
    TagPrefix="kbc" %>
<%@ Register Src="~/Controls/PostCommentControl.ascx" TagName="PostCommentControl"
    TagPrefix="kbc" %>
<%@ Register Src="~/Controls/CommentsListControl.ascx" TagName="CommentsListControl"
    TagPrefix="kbc" %>
<%@ Register Src="Controls/ArticlesTreeControl.ascx" TagName="ArticlesTreeControl"
    TagPrefix="kbc" %>
<%@ Register Src="Controls/ArticleList.ascx" TagName="ArticleListControl" TagPrefix="kbc" %>
<%@ Register Src="Controls/FileListControl.ascx" TagName="FileListControl" TagPrefix="kbc" %>
<%@ MasterType TypeName="BWA.Knowledgebase.MasterPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageBody" runat="Server">
    <asp:Panel ID="PanelArticle" runat="server" meta:resourcekey="PanelArticleResource1">
        <h1 class="bottomLine">
            <asp:Literal ID="litSubject" runat="server"></asp:Literal></h1>
        <div id="divBody" runat="server">
        </div>
        <kbc:FileListControl ID="AttachmentListCtrl" runat="server" EnableDeleting="false" />
        <%--<kbc:FileListControl ID="ImageListCtrl" runat="server" EnableDeleting="false" AttachedFileType="Image" />
        <kbc:FileListControl ID="FileListCtrl" runat="server" EnableDeleting="false" AttachedFileType="File" />
        <kbc:FileListControl ID="VideoListCtrl" runat="server" EnableDeleting="false" AttachedFileType="Video" />--%>
        <%--<kbc:ArticlesTreeControl runat="server" ID="ArticlesTreeCtrl" IsAdmin="false" />--%>
        <kbc:ArticleListControl runat="server" ID="ArticlesListCtrl" BodyLength="1024" />
        <kbc:CommentsListControl ID="CommentsListCtrl" runat="server" />
        <kbc:PostCommentControl ID="PostCommentCtrl" runat="server" OnCommentPosted="PostCommentCtrl_CommentPosted"
            Width="500px" />
    </asp:Panel>
    <asp:Panel ID="PanelSuccess" runat="server" meta:resourcekey="PanelSuccessResource1"
        Visible="False">
        <h2 align="center">
            <br />
            <br />
            <br />
            <asp:Literal ID="LabelCommentPosted" runat="server" Text="Your comment was posted successfully!"
                meta:resourcekey="LabelCommentPostedResource1"></asp:Literal>
        </h2>
    </asp:Panel>
</asp:Content>
