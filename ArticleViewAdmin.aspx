<%@ Page Title="View Article" Language="C#" MasterPageFile="~/MasterPage.master"
    AutoEventWireup="true" CodeFile="ArticleViewAdmin.aspx.cs" Inherits="BWA.Knowledgebase.ArticleViewAdmin"
    ValidateRequest="false" meta:resourcekey="PageResource1" Async="true" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI.Editor" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/SearchArticleControl.ascx" TagName="SearchArticleControl"
    TagPrefix="kbc" %>
<%@ Register Src="~/Controls/PostCommentControl.ascx" TagName="PostCommentControl"
    TagPrefix="kbc" %>
<%@ Register Src="~/Controls/CommentsListControl.ascx" TagName="CommentsListControl"
    TagPrefix="kbc" %>
<%@ Register Src="~/Controls/ArticlesTreeControl.ascx" TagName="ArticlesTreeControl"
    TagPrefix="kbc" %>
<%@ Register Src="~/Controls/FileListControl.ascx" TagName="FileListControl" TagPrefix="kbc" %>
<%@ MasterType TypeName="BWA.Knowledgebase.MasterPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageBody" runat="Server">
    <asp:Panel ID="PanelView" runat="server" Visible="False" meta:resourcekey="PanelViewResource1">
        <div style="white-space: nowrap; margin-bottom: 7px; margin-top: 0px;">
            <span style="vertical-align: baseline;">
                <asp:LinkButton ID="LinkButtonEditArticle" runat="server" Font-Bold="false" OnClick="LinkButtonEditArticle_Click"
                    Text="Edit Article" ForeColor="Blue" Font-Size="10pt" meta:resourcekey="LinkButtonEditArticleResource1"></asp:LinkButton>
            </span><span>&nbsp;&nbsp;&nbsp;&nbsp;</span><span style="vertical-align: baseline;">
                <asp:HyperLink ID="HyperLinkPreview" runat="server" Font-Bold="false" Text="Preview"
                    ForeColor="Blue" Font-Size="9pt" NavigateUrl="#"></asp:HyperLink>
            </span><span>&nbsp;&nbsp;&nbsp;&nbsp;</span><span style="vertical-align: baseline;">
                <asp:LinkButton ID="ButtonInactive" runat="server" Font-Size="9pt" ForeColor="Blue"
                    Font-Bold="false" OnClick="ButtonInactive_Click" Text="Inactivate" meta:resourcekey="ButtonInactiveResource1"></asp:LinkButton>
            </span>
        </div>
        <div id="divTitle">
            <h1>
                <asp:Literal ID="litSubject" runat="server"></asp:Literal></h1>
        </div>
        <div id="divBody" runat="server">
        </div>
        <br />
        <kbc:FileListControl ID="ImageListCtrl" runat="server" EnableDeleting="false" AttachedFileType="Hidden" />
        <kbc:FileListControl ID="FileListCtrl" runat="server" EnableDeleting="false" AttachedFileType="Visible" />
        <kbc:ArticlesTreeControl runat="server" ID="ArticlesTreeCtrl" IsAdmin="True" />
        <kbc:CommentsListControl ID="CommentsListCtrl" runat="server" Width="90%" />
        <kbc:PostCommentControl ID="PostCommentCtrl" runat="server" OnCommentPosted="PostCommentCtrl_CommentPosted"
            Width="500px" />
    </asp:Panel>
    <asp:Panel ID="PanelArticleEdit" runat="server" Visible="False" meta:resourcekey="PanelArticleEditResource1">
        <table width="60%" cellpadding="2" cellspacing="2" border="0">
            <tr>
                <td valign="bottom">
                </td>
            </tr>
            <tr>
                <td valign="top" class="textbox" style="padding-left: 6px">
                    <mits:TextBox ID="TextBoxSubject" runat="server" MaxLength="255" Width="640px" ValidationGroup="ValidationGroupEdit"
                        Required="True" Mask="" TabIndex="0" meta:resourcekey="TextBoxSubjectResource1" />
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <asp:Label ID="LabelParentArticle" runat="server" CssClass="BoldText" Text="Parent Article"
                        AssociatedControlID="TreeViewParentArticle" meta:resourcekey="LabelParentArticleResource1"></asp:Label>
                    <mits:TreeView ID="TreeViewParentArticle" runat="server" ComboBoxMode="True" DataFieldID="ArticleGuid"
                        DataFieldParentID="ParentArticleGuid" DataTextField="Subject" DataValueField="ArticleGuid"
                        DataSourceID="ObjectDataSourceParentArticles" Width="640px" AppendDataBoundItems="True"
                        ValidationGroup="ValidationGroupEdit" meta:resourcekey="TreeViewParentArticleResource1">
                    </mits:TreeView>
                </td>
            </tr>
            <tr>
                <td valign="bottom">
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <telerik:RadEditor ID="HtmlEditorBody" runat="server" Required="True" ValidationGroup="ValidationGroupEdit"
                        DocumentsFilters="*.*" ImagesFilters="*.gif,*.xbm,*.xpm,*.png,*.ief,*.jpg,*.jpe,*.jpeg,*.tiff,*.tif,*.rgb,*.g3f,*.xwd,*.pict,*.ppm,*.pgm,*.pbm,*.pnm,*.bmp,*.ras,*.pcd,*.cgm,*.mil,*.cal,*.fif,*.dsf,*.cmx,*.wi,*.dwg,*.dxf,*.svf"
                        MediaFilters="*.asf,*.asx,*.wm,*.wmx,*.wmp,*.wma,*.wax,*.wmv,*.wvx,*.avi,*.wav,*.mpeg,*.mpg,*.mpe,*.mov,*.m1v,*.mp2,*.mpv2,*.mp2v,*.mpa,*.mp3,*.m3u,*.mid,*.midi,*.rm,*.rma,*.rmi,*.rmv,*.aif,*.aifc,*.aiff,*.au,*.snd,*.swf"
                        TemplateFilters="*.html,*.htm" NewLineBr="false" RegisterWithScriptManager="true" ConvertTagsToLower="True"
                        EnableContextMenus="false" PassSessionData="True" ToolsFile="ToolsFile.xml" Width="750px"
                        Height="600px" ExternalDialogsPath="~/Controls/" meta:resourcekey="HtmlEditorBodyResource1">
                    </telerik:RadEditor>
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <br />
                    <div>
                        <b><asp:Literal runat="server" ID="Label4" Text="SEO Search Meta Tag" /></b>
                        <span style="color: Gray">
                            <asp:Literal runat="server" ID="Literal1" Text="(Use a phrase, not keywords, this shows in Google search result)" /></span>
                    </div>
                    <div style="padding-left: 9px">                                            
                        <asp:TextBox ID="SearchDescriptionTextBox" runat="server" ToolTip="SEO Search Meta Tag"
                            TextMode="MultiLine" Rows="2" MaxLength="150" Width="640px">
                        </asp:TextBox>
                    </div>
                </td>
            </tr>
            <tr>
                <td valign="top">                   
                    <kbc:FileListControl ID="ImageAdminListCtrl" runat="server" AttachedFileType="Hidden"
                        EnableDeleting="true" />
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <kbc:FileListControl ID="FileAdminListCtrl" runat="server" AttachedFileType="Visible"
                        EnableDeleting="true" />
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <%--<div class="pageInfo">
                        <p class="SubHeader">
                            <asp:Label runat="server" ID="AttachLabel" Text="Attach the new files" meta:resourcekey="AttachLabelResource1" />
                        </p>
                    </div>--%>
                    <fs:SimpleUpload ID="UploadControl" UploadControlsUniqueId="ctl00$PageBody$ButtonUpload,ctl00$PageBody$ButtonUpdate"
                        LocalObjectType="Article" runat="server" ShowUploadedFiles="false" />
                    <br />
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <div>
                        <b><asp:Literal ID="LabelAlternateId" runat="server" Text="Alternate IDs"></asp:Literal></b>
                        <span style="color: Gray">
                            <asp:Literal runat="server" ID="Literal2" Text="(One alternate id per line)" /></span>
                    </div>
                    <div style="padding-left: 9px">
                        <asp:TextBox ID="TextBoxAlternateIds" runat="server" Width="640px" ValidationGroup="ValidationGroupEdit"
                            TextMode="MultiLine" Rows="4" Mask="" TabIndex="0" meta:resourcekey="TextBoxAlternateIdsResource1" />
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                    <asp:Button ID="ButtonUpload" runat="server" OnClick="ButtonUpload_Click" Text="Save Article & Refresh"
                        Font-Size="10pt" ToolTip="Images upload" ValidationGroup="ValidationGroupEdit" />
                    &nbsp;
                    <asp:Button ID="ButtonUpdate" runat="server" OnClick="ButtonUpdate_Click" Text="Save Article & Close"
                        Font-Size="10pt" ToolTip="Article update" ValidationGroup="ValidationGroupEdit"
                        meta:resourcekey="ButtonUpdateResource1" />
                    &nbsp;
                    <asp:LinkButton ID="LinkButtonCancelEdit" runat="server" Font-Size="10pt" OnClick="LinkButtonCancelEdit_Click"
                        meta:resourcekey="LinkButtonCancelEditResource1" Text="Cancel"></asp:LinkButton>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="PanelRequest" runat="server" Visible="False" meta:resourcekey="PanelRequestResource1">
        <br />
        <mits:MagicForm ID="MagicFormRequest" runat="server" DataKeyNames="ArticleGuid" DataSourceID="ObjectDataSourceRequest"
            ColorScheme="TanGray" Width="60%" AutoGenerateRows="False" ObjectName="Request"
            meta:resourcekey="MagicFormRequestResource1" Caption="View&amp;nbsp;Request"
            CellPadding="0" GridLines="None" CssClass="Mf_T">
            <AlternatingRowStyle CssClass="Mf_R" />
            <EditRowStyle CssClass="Mf_R" />
            <EmptyDataRowStyle CssClass="Mf_R" />
            <Fields>
                <mits:TextBoxField DataField="ArticleID" HeaderText="#" meta:resourceKey="TextBoxFieldResource1">
                    <ItemStyle Font-Bold="True" />
                </mits:TextBoxField>
                <mits:TextBoxField DataField="UserName" HeaderText="Name" meta:resourceKey="TextBoxFieldResource2">
                </mits:TextBoxField>
                <mits:HyperLinkField DataNavigateUrlFields="UserEmail" DataNavigateUrlFormatString="mailto:{0}"
                    DataTextField="UserEmail" HeaderText="Email" meta:resourceKey="HyperLinkFieldResource1">
                </mits:HyperLinkField>
                <mits:TextBoxField DataField="CreatedTime" HeaderText="Date" meta:resourceKey="TextBoxFieldResource3">
                </mits:TextBoxField>
                <mits:TextBoxField DataField="AlternateId" HeaderText="Alternate #" meta:resourceKey="TextBoxFieldResource5">
                </mits:TextBoxField>
                <mits:TextBoxField DataField="Body" HeaderText="Comment" meta:resourceKey="TextBoxFieldResource6"
                    TextMode="MultiLine">
                </mits:TextBoxField>
            </Fields>
            <FooterStyle CssClass="Mf_F" />
            <HeaderStyle CssClass="Mf_H" />
            <RowStyle CssClass="Mf_R" />
        </mits:MagicForm>
        <br />
        <asp:Button ID="ButtonCreateArticle" runat="server" OnClick="ButtonCreateArticle_Click"
            Text="Create a new article from this request" meta:resourcekey="ButtonCreateArticleResource1" />&nbsp;&nbsp;
        <asp:Button ID="ButtonCancelRequest" runat="server" OnClick="ButtonCancelRequest_Click"
            OnClientClick="javascript:return confirm('Do you sure to cancel this request?');"
            Text="Cancel this request" meta:resourcekey="ButtonCancelRequestResource1" />
        &nbsp;or
        <asp:LinkButton runat="server" Text="Cancel" ID="CancelButton" OnClick="CancelButton_Click" />
        <asp:ObjectDataSource ID="ObjectDataSourceRequest" runat="server" OldValuesParameterFormatString="original_{0}"
            SelectMethod="GetDataByArticleGuid" TypeName="MainDataSetTableAdapters.RequestListTableAdapter"
            OnSelecting="ObjectDataSourceRequest_Selecting">
            <SelectParameters>
                <asp:Parameter DbType="Guid" Name="ArticleGuid" />
                <asp:Parameter Name="Type" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <asp:ObjectDataSource ID="ObjectDataSourceParentArticles" runat="server" OldValuesParameterFormatString="original_{0}"
            OnSelecting="ObjectDataSourceParentArticles_Selecting" SelectMethod="GetDataWithoutArticle"
            TypeName="MainDataSetTableAdapters.ArticleTableAdapter">
            <SelectParameters>
                <asp:Parameter DbType="Guid" Name="DepartmentGuid" />
                <asp:Parameter DbType="Guid" Name="ArticleGuid" />
                <asp:Parameter DbType="String" Name="Type" DefaultValue="Article" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </asp:Panel>
    <asp:Panel ID="PanelArticleDelete" runat="server" Visible="False" meta:resourcekey="PanelArticleDeleteResource1">
        <table width="100%" cellpadding="2" cellspacing="2" border="0" id="tableInactive"
            runat="server">
            <tr runat="server">
                <td valign="bottom" runat="server">
                </td>
            </tr>
            <tr runat="server">
                <td valign="top" class="textbox" runat="server">
                    <div class="pageTitle">
                        <h1>
                            <asp:Literal ID="litSubjectDelete" runat="server"></asp:Literal>
                        </h1>
                    </div>
                    <hr />
                </td>
            </tr>
            <tr runat="server">
                <td valign="bottom" runat="server">
                    &nbsp;<asp:Label ID="Label1" runat="server" CssClass="BoldText" Text="Select the article in which you would like to transfer the all the related articles"
                        AssociatedControlID="TreeViewParentArticle"></asp:Label>
                </td>
            </tr>
            <tr runat="server">
                <td valign="top" class="textbox" runat="server">
                    <table cellpadding="0" cellspacing="0" border="0">
                        <tr>
                            <td style="width: 3px; background-color: Maroon; white-space: nowrap;">
                                &nbsp;
                            </td>
                            <td>
                                <mits:TreeView ID="TreeViewParentArticleToDelete" runat="server" ComboBoxMode="True"
                                    DataFieldID="ArticleGuid" DataFieldParentID="ParentArticleGuid" DataTextField="Subject"
                                    DataValueField="ArticleGuid" DataSourceID="ObjectDataSourceParentArticles" Width="800px"
                                    AppendDataBoundItems="True" ValidationGroup="ValidationGroupInactive">
                                </mits:TreeView>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td style="padding-left: 2px;">
                                <asp:CustomValidator ID="LtvValidator" runat="server" ClientValidationFunction="StartActionValidate"
                                    Display="Dynamic" ErrorMessage="You must select a article" ValidationGroup="ValidationGroupInactive" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr runat="server">
                <td valign="bottom" runat="server">
                    <br />
                    <br />
                </td>
            </tr>
            <tr runat="server">
                <td valign="top" align="center" runat="server">
                    <asp:Label ID="LabelConfirmDelete" runat="server" Font-Bold="True" Font-Size="14pt"
                        Text="Are you sure you want to inactive this article?"></asp:Label>
                    <p>
                        <asp:Label ID="Label2" runat="server" ForeColor="#333333" Text="This article will be marked as inactive can be re-activated by selecting the 'Activate this article' link."></asp:Label><br />
                        <asp:Label ID="Label3" runat="server" ForeColor="#333333" Text="All related articles will be reassigned to the new article."></asp:Label></p>
                </td>
            </tr>
            <tr runat="server">
                <td valign="top" align="center" style="padding-right: 60px;" runat="server">
                    <table border="0" cellpadding="0" width="60%">
                        <tr>
                            <td align="center">
                                <asp:Button ID="ButtonInactiveArticle" runat="server" Text="Inactive" OnClick="ButtonInactiveArticle_Click"
                                    ValidationGroup="ValidationGroupInactive" />
                            </td>
                            <td>
                                <asp:Button ID="ButtonCancelInactive" runat="server" Text="Cancel" OnClick="ButtonCancelInactive_Click"
                                    CausesValidation="False" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <div style="display: none">
        <telerik:RadEditor ID="EditorHiodden" runat="server" Width="0" Height="0">
        </telerik:RadEditor>
    </div>

    <script type="text/javascript">
        //<![CDATA[
        MyFilter = function() {
            MyFilter.initializeBase(this);
            this.set_isDom(false);
            this.set_enabled(true);
            this.set_name("RadEditor filter");
            this.set_description("RadEditor filter description");
        }
        MyFilter.prototype = {
            getHtmlContent: function(content) {
                var newContent = content.replace(/<p>&nbsp;<\/p>/gi, '<p></p>');
                return newContent;
            },
            getDesignContent: function(content) {
                var newContent = content.replace(/<p>&nbsp;<\/p>/gi, '<p></p>');
                return newContent;
            }
        }
        MyFilter.registerClass('MyFilter', Telerik.Web.UI.Editor.Filter);
        function EditorLoad(editor, args) {
            editor.get_filtersManager().add(new MyFilter());
        }
        //]]>
    </script>

</asp:Content>
