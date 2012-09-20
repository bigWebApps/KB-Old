using System;
using System.Web.UI.WebControls;
using Micajah.Common.Security;
using System.Data;
using Micajah.FileService.WebControls;
using System.Web;
using System.Globalization;
using Micajah.FileService.Client.Dal;
using System.Web.UI.HtmlControls;
using System.Text;
using Micajah.Common.Bll;
using Micajah.Common.Application;
using System.Collections;

namespace BWA.Knowledgebase
{
    public partial class ArticleViewAdmin : System.Web.UI.Page
    {
        #region Members
        private MainDataSet.ArticleRow m_articleRow = null;
        private MainDataSetTableAdapters.ArticleTableAdapter m_taArticle = null;
        private Guid articleGuid = Guid.Empty;
        #endregion

        #region Properties

        public MainDataSetTableAdapters.ArticleTableAdapter ArticleTableAdapter
        {
            get
            {
                if (m_taArticle == null)
                    m_taArticle = new MainDataSetTableAdapters.ArticleTableAdapter();
                return m_taArticle;
            }
        }

        public Guid ArticleGuid
        {
            get
            {
                Guid g = Guid.Empty;
                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    try { g = new Guid(Request.QueryString["id"]); }
                    catch { }
                }
                return g;
            }
        }

        public Guid ParntArticleGuid
        {
            get
            {
                Guid g = Guid.Empty;
                if (TreeViewParentArticle.SelectedNodes.Count > 0 &&
                    !string.IsNullOrEmpty(TreeViewParentArticle.SelectedNode.Value))
                {
                    try { g = new Guid(TreeViewParentArticle.SelectedNode.Value); }
                    catch { }
                }
                return g;
            }
        }

        public Guid NewArticleGuid
        {
            get
            {
                Guid g = Guid.Empty;
                if (TreeViewParentArticleToDelete.SelectedNodes.Count > 0 &&
                    !string.IsNullOrEmpty(TreeViewParentArticleToDelete.SelectedNode.Value))
                {
                    try { g = new Guid(TreeViewParentArticleToDelete.SelectedNode.Value); }
                    catch { }
                }
                return g;
            }
        }

        public string Mode
        {
            get
            {
                string g = string.Empty;
                if (!string.IsNullOrEmpty(Request.QueryString["mode"]))
                    g = Request.QueryString["mode"];
                return g;
            }
        }
        #endregion

        #region Private Methods
        private MainDataSet.ArticleRow GetCurrentArticle()
        {
            if (!this.ArticleGuid.Equals(Guid.Empty) && m_articleRow == null)
            {
                MainDataSet.ArticleDataTable dtArticle = this.ArticleTableAdapter.GetDataByArticleGuid(this.ArticleGuid);
                if (dtArticle.Count > 0)
                    m_articleRow = dtArticle[0];
            }
            return m_articleRow;
        }
        private void DoAricleView(MainDataSet.ArticleRow articleRow)
        {
            if (articleRow != null)
            {
                this.ArticleTableAdapter.IncReview(articleRow.ArticleGuid);
                ImageListCtrl.ArticleGuid = articleRow.ArticleGuid;
                FileListCtrl.ArticleGuid = articleRow.ArticleGuid;
                ShowArticleView();

                HyperLinkPreview.Attributes["onclick"] = string.Format(CultureInfo.InvariantCulture, "javascript:window.open('default.aspx?i={0}&t={1}&popup=true', '_blank', 'location=0,menubar=0,resizable=1,scrollbars=1,status=0,titlebar=0,toolbar=0,top=' + Mp_GetPopupPositionY(event) + ',left=' + Mp_GetPopupPositionX({2})  + ',width={2},height={3}');",
                    UserContext.Current.SelectedInstance.InstanceId.ToString("N"),
                    articleRow.ArticleGuid.ToString("N"),
                    Micajah.Common.Configuration.FrameworkConfiguration.Current.WebApplication.MasterPage.HelpLink.WindowWidth,
                    Micajah.Common.Configuration.FrameworkConfiguration.Current.WebApplication.MasterPage.HelpLink.WindowHeight);
                if (articleRow.Deleted)
                {
                    ButtonInactive.Text = (string)this.GetLocalResourceObject("ActivateArticle");
                    ButtonInactive.OnClientClick = (string)this.GetLocalResourceObject("ConfirmActivate");
                }
                else
                {
                    ButtonInactive.Text = (string)this.GetLocalResourceObject("InactivateArticle");
                    ButtonInactive.OnClientClick = string.Empty;
                }
                Master.CustomName = articleRow.Subject;
                //Master.Title = string.Format("{0} {1} KB - {2}", this.ArticleTableAdapter.GetOrganizationName(this.Master.InstanceGuid), this.ArticleTableAdapter.GetInstanceName(this.Master.InstanceGuid), articleRow.Subject);
                Master.Title = articleRow.Subject;

                //Master.VisiblePageTitle = false;
                litSubject.Text = articleRow.Subject;
                string body = this.ArticleTableAdapter.GetBody(articleRow.ArticleGuid);
                if (!string.IsNullOrEmpty(body))
                {
                    body = body.Replace("<h2>", "<h2><span>");
                    body = body.Replace("<H2>", "<H2><span>");
                    body = body.Replace("</h2>", "</span></h2>");
                    body = body.Replace("</H2>", "</span></H2>");
                    divBody.InnerHtml = body;//HttpUtility.HtmlDecode(body);
                }
                HtmlMeta metaDescription = new HtmlMeta();
                metaDescription.Name = "Description";
                metaDescription.Content = articleRow.SearchDesc;
                Page.Header.Controls.Add(metaDescription);
                Guid userId = Guid.Empty;
                if (!articleRow.IsCreatedByNull())
                    userId = articleRow.CreatedBy;
                if (!articleRow.IsUpdatedByNull())
                    userId = articleRow.UpdatedBy;
                //if (userId == Guid.Empty)
                //    PostCommentCtrl.ReceiverName = Utils.GetInstanceUserName(UserContext.Current.SelectedInstance.InstanceId);
                //else
                //{
                //    Micajah.Common.Dal.OrganizationDataSet.UserRow userRow = Micajah.Common.Bll.Providers.UserProvider.GetUserRow(userId);
                //    if (userRow != null)
                //        PostCommentCtrl.ReceiverName = userRow.FirstName + " " + userRow.LastName;
                //}
                PostCommentCtrl.InstanceGuid = UserContext.Current.SelectedInstance.InstanceId;
                PostCommentCtrl.ArticleGuid = articleRow.ArticleGuid;
                CommentsListCtrl.ArticleGuid = articleRow.ArticleGuid;
                CommentsListCtrl.DataBind();
            }
        }
        private void CustomLinksDataBind(Guid instanceGuid)
        {
            HtmlEditorBody.Links.Clear();

            MainDataSet.ArticleDataTable dtLinks = this.ArticleTableAdapter.GetRecursiveByDepartmentGuid(instanceGuid);
            foreach (MainDataSet.ArticleRow row in dtLinks.Where(p => p.IsParentArticleGuidNull()))
            {
                Telerik.Web.UI.EditorLink link = new Telerik.Web.UI.EditorLink(row.Subject, string.Format("{0}/Default.aspx?i={1}&t={2}", Request.ApplicationPath, instanceGuid.ToString("N"), row.ArticleGuid.ToString("N")));
                SetCustomLink(ref link, dtLinks, row.ArticleGuid);
                HtmlEditorBody.Links.Add(link);
            }
        }

        private void SetCustomLink(ref Telerik.Web.UI.EditorLink link, MainDataSet.ArticleDataTable articles, Guid articleGuid)
        {
            foreach (MainDataSet.ArticleRow row in articles.Where(p => !p.IsParentArticleGuidNull() && p.ParentArticleGuid == articleGuid))
            {
                Telerik.Web.UI.EditorLink newlink = new Telerik.Web.UI.EditorLink(row.Subject, string.Format("{0}/Default.aspx?i={1}&t={2}", Request.ApplicationPath, row.DepartmentGuid.ToString("N"), row.ArticleGuid.ToString("N")));
                SetCustomLink(ref newlink, articles, row.ArticleGuid);
                link.ChildLinks.Add(newlink);
            }
        }
        private void DoArticleEdit(MainDataSet.ArticleRow articleRow)
        {
            ShowArticleEdit();
            TreeViewParentArticle.Nodes.Clear();
            TreeViewParentArticle.Nodes.Add(new Telerik.Web.UI.RadTreeNode("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", string.Empty));
            TreeViewParentArticle.DataBind();

            CustomLinksDataBind(UserContext.Current.SelectedInstance.InstanceId);

            if (articleRow != null)
            {
                HtmlEditorBody.ImageManager.ViewPaths = new string[] { articleRow.ArticleGuid.ToString("N") };
                HtmlEditorBody.ImageManager.ContentProviderTypeName = typeof(Micajah.FileService.Providers.ImageDBContentProvider).AssemblyQualifiedName;
                HtmlEditorBody.MediaManager.ViewPaths = new string[] { articleRow.ArticleGuid.ToString("N") };
                HtmlEditorBody.MediaManager.ContentProviderTypeName = typeof(Micajah.FileService.Providers.VideoDBContentProvider).AssemblyQualifiedName;
                HtmlEditorBody.FlashManager.ViewPaths = new string[] { articleRow.ArticleGuid.ToString("N") };
                HtmlEditorBody.FlashManager.ContentProviderTypeName = typeof(Micajah.FileService.Providers.FlashDBContentProvider).AssemblyQualifiedName;
                HtmlEditorBody.DocumentManager.ViewPaths = new string[] { articleRow.ArticleGuid.ToString("N") };
                HtmlEditorBody.DocumentManager.ContentProviderTypeName = typeof(Micajah.FileService.Providers.FileDBContentProvider).AssemblyQualifiedName;
                HtmlEditorBody.Content = this.ArticleTableAdapter.GetBody(articleRow.ArticleGuid);

                ImageAdminListCtrl.ArticleGuid = articleRow.ArticleGuid;
                FileAdminListCtrl.ArticleGuid = articleRow.ArticleGuid;
                ImageAdminListCtrl.DataBind();
                FileAdminListCtrl.DataBind();

                Master.CustomName = articleRow.Subject;
                TextBoxSubject.Text = articleRow.Subject;
                SearchDescriptionTextBox.Text = articleRow.SearchDesc;
                if (!articleRow.IsParentArticleGuidNull())
                {
                    Telerik.Web.UI.RadTreeNode node = TreeViewParentArticle.FindNodeByValue(articleRow.ParentArticleGuid.ToString());
                    if (node != null)
                    {
                        node.Selected = true;
                        node.ExpandParentNodes();
                    }
                }
                if (articleRow.Type == ArticleType.Request.ToString())
                    TextBoxAlternateIds.Text = articleRow.Subject;
                else
                {
                    using (MainDataSetTableAdapters.AlternateIdTableAdapter altAdapter = new MainDataSetTableAdapters.AlternateIdTableAdapter())
                    {
                        TextBoxAlternateIds.Text = string.Empty;
                        MainDataSet.AlternateIdDataTable dtAlt = altAdapter.GetDataByArticleGuid(articleRow.ArticleGuid);
                        foreach (MainDataSet.AlternateIdRow row in dtAlt)
                            TextBoxAlternateIds.Text += row.AlternateId + Environment.NewLine;
                    }
                }
            }
            else
            {
                Master.CustomName = (string)this.GetLocalResourceObject("AddNewArticle");
            }
        }

        private void DoUploadFile(Guid articleGuid)
        {
            UploadControl.LocalObjectId = articleGuid.ToString("N");
            UploadControl.AcceptChanges();
            ImageAdminListCtrl.ArticleGuid = articleGuid;
            ImageAdminListCtrl.DataBind();
            FileAdminListCtrl.ArticleGuid = articleGuid;
            FileAdminListCtrl.DataBind();
        }

        private void ShowArticleView()
        {
            PanelRequest.Visible = false;
            PanelView.Visible = true;
            PanelArticleEdit.Visible = false;
            PanelArticleDelete.Visible = false;
        }
        private void ShowArticleEdit()
        {
            PanelRequest.Visible = false;
            PanelView.Visible = false;
            PanelArticleEdit.Visible = true;
            PanelArticleDelete.Visible = false;
        }
        private void ShowRequestView()
        {
            PanelRequest.Visible = true;
            PanelView.Visible = false;
            PanelArticleEdit.Visible = false;
            PanelArticleDelete.Visible = false;
        }
        private void ShowArticleInactive()
        {
            PanelRequest.Visible = false;
            PanelView.Visible = false;
            PanelArticleEdit.Visible = false;
            PanelArticleDelete.Visible = true;
        }
        #endregion

        #region Handle Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            MagicFormRequest.ColorScheme = Micajah.Common.Configuration.FrameworkConfiguration.Current.WebApplication.DefaultColorScheme;
            if (!this.ClientScript.IsClientScriptBlockRegistered("TreeViewValidator"))
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "TreeViewValidator", string.Format(@"function StartActionValidate(source, arguments) {{ var _treeView = $find('{0}'); if (_treeView) arguments.IsValid = (_treeView.get_selectedNode() != null); }}", TreeViewParentArticleToDelete.ClientID), true);

            UploadControl.OrganizationId = UserContext.Current.SelectedOrganization.OrganizationId;
            UploadControl.OrganizationName = UserContext.Current.SelectedOrganization.Name;
            UploadControl.DepartmentName = UserContext.Current.SelectedInstance.Name;
            UploadControl.DepartmentId = UserContext.Current.SelectedInstance.InstanceId;

            if (!IsPostBack)
            {
                //                FileAdminListCtrl.UploadControlsUniqueId = ButtonUpdate.UniqueID;
                Master.AutoGenerateBreadcrumbs = false;
                UserContext.Breadcrumbs.Add((string)this.GetLocalResourceObject("Home"), UserContext.Current.StartPageUrl, string.Empty, false);
                //SearchArticleCtrl.InstanceGuid = UserContext.Current.SelectedInstance.InstanceId;

                if (!this.ArticleGuid.Equals(Guid.Empty))
                {
                    MainDataSet.ArticleRow articleRow = this.GetCurrentArticle();
                    if (articleRow != null)
                    {
                        using (MainDataSetTableAdapters.CommentTableAdapter taComment = new MainDataSetTableAdapters.CommentTableAdapter())
                        {
                            taComment.MarkAsRead(articleRow.ArticleGuid);
                        }
                        //Master.AutoGenerateBreadcrumbs = false;
                        ArticlesTreeCtrl.ArticleGuid = articleRow.ArticleGuid;

                        if (this.Mode == "edit")
                        {
                            DoArticleEdit(articleRow);
                            return;
                        }
                        if (articleRow.Type == ArticleType.Request.ToString())
                        {
                            UserContext.Breadcrumbs.Add((string)this.GetLocalResourceObject("TheRequestsList"), "RequestList.aspx", string.Empty, false);
                            UserContext.Breadcrumbs.AddRange(Utils.GenerateBreadCrumbs(articleRow.ArticleGuid, UserContext.Current.SelectedInstance.InstanceId, true));
                            ShowRequestView();
                            MagicFormRequest.DataBind();
                            ImageListCtrl.Visible = FileListCtrl.Visible = false;
                            return;
                        }
                        else if (articleRow.Type == ArticleType.Article.ToString())
                        {
                            if (Request.QueryString["UC"] != null)
                                UserContext.Breadcrumbs.Add((string)this.GetLocalResourceObject("TheUnreadCommentsList"), "UnreadComments.aspx", string.Empty, false);
                            else
                                UserContext.Breadcrumbs.Add((string)this.GetLocalResourceObject("TheInstanceHome"), "InstanceHomeAdmin.aspx", string.Empty, false);
                            UserContext.Breadcrumbs.AddRange(Utils.GenerateBreadCrumbs(articleRow.ArticleGuid, UserContext.Current.SelectedInstance.InstanceId, true));
                            DoAricleView(articleRow);

                            return;
                        }
                    }
                    Master.AutoGenerateBreadcrumbs = false;
                    Master.ErrorMessage = (string)this.GetLocalResourceObject("ArticleNotFound");
                }
                else
                {
                    UserContext.Breadcrumbs.Add((string)this.GetLocalResourceObject("TheInstanceHome"), "InstanceHomeAdmin.aspx", string.Empty, false);
                    UserContext.Breadcrumbs.Add((string)this.GetLocalResourceObject("AddNewArticle"), string.Empty, string.Empty, false);
                    DoArticleEdit(null);
                }
            }
        }

        protected void PostCommentCtrl_CommentPosted(object sender, ArticleEventArgs e)
        {
            CommentsListCtrl.DataBind();
            CommentsListCtrl.RegisterGoAnchor();
        }

        protected void ObjectDataSourceRequest_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["ArticleGuid"] = this.ArticleGuid;
            e.InputParameters["Type"] = ArticleType.Request.ToString();
        }

        protected void ButtonCreateArticle_Click(object sender, EventArgs e)
        {
            this.DoArticleEdit(this.GetCurrentArticle());

            LinkButtonCancelEdit.Visible = false;
            ButtonUpdate.Text = (string)this.GetLocalResourceObject("ButtonUpdateResource1.Text");
        }

        protected void ButtonCancelRequest_Click(object sender, EventArgs e)
        {
            if (this.ArticleTableAdapter.Delete(this.ArticleGuid) > 0)
                Response.Redirect("~/RequestList.aspx", false);

        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/RequestList.aspx", false);
        }

        protected void ObjectDataSourceParentArticles_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["DepartmentGuid"] = UserContext.Current.SelectedInstance.InstanceId;
            e.InputParameters["ArticleGuid"] = this.ArticleGuid;
            e.InputParameters["Type"] = ArticleType.Article.ToString();
        }

        protected void ButtonUpdate_Click(object sender, EventArgs e)
        {
            ButtonUpload_Click(sender, e);
            if (!articleGuid.Equals(Guid.Empty))
                Response.Redirect("~/ArticleViewAdmin.aspx?id=" + articleGuid.ToString("N"), false);
        }

        protected void ButtonUpload_Click(object sender, EventArgs e)
        {
            using (MainDataSetTableAdapters.AlternateIdTableAdapter altAdapter = new MainDataSetTableAdapters.AlternateIdTableAdapter())
            {
                articleGuid = this.ArticleGuid;
                foreach (string str in TextBoxAlternateIds.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if ((int)altAdapter.CheckAlternateId(UserContext.Current.SelectedInstance.InstanceId, articleGuid, str) == 1)
                    {
                        Master.ErrorMessage = string.Format((string)this.GetLocalResourceObject("ErrorMsg_AlternateId"), str);
                        return;
                    }
                }
                //string htmlBody = HtmlEditorBody.Content;
                if (!HtmlEditorBody.Content.EndsWith("<p></p>\r\n") &&
                    !HtmlEditorBody.Content.EndsWith("<p>nbsp;</p>\r\n") &&
                    !HtmlEditorBody.Content.EndsWith("<p> </p>\r\n") &&
                    !HtmlEditorBody.Content.EndsWith("<p></p>") &&
                    !HtmlEditorBody.Content.EndsWith("<p> </p>") &&
                    !HtmlEditorBody.Content.EndsWith("<p>nbsp;</p>"))
                    HtmlEditorBody.Content += "<p></p>\r\n";
                if (articleGuid.Equals(Guid.Empty))
                {
                    articleGuid = Guid.NewGuid();
                    this.ArticleTableAdapter.Insert(
                        articleGuid,
                        UserContext.Current.SelectedInstance.InstanceId,
                        ((this.ParntArticleGuid != Guid.Empty) ? this.ParntArticleGuid : new Guid?()),
                        ArticleType.Article.ToString(),
                        TextBoxSubject.Text,
                        HtmlEditorBody.Content,
                        SearchDescriptionTextBox.Text,
                        0,
                        0,
                        0,
                        false,
                        new DateTime?(DateTime.Now),
                        new DateTime?(),
                        new DateTime?(),
                        new Guid?(UserContext.Current.UserId),
                        new Guid?(),
                        new Guid?());

                    if (!articleGuid.Equals(Guid.Empty))
                    {
                        DoUploadFile(articleGuid);
                        if (this.ArticleGuid.Equals(Guid.Empty))
                            Response.Redirect("~/ArticleViewAdmin.aspx?mode=edit&id=" + articleGuid.ToString("N"), false);
                    }
                }
                else
                {
                    MainDataSet.ArticleRow articleRow = this.GetCurrentArticle();
                    string oldType = articleRow.Type;
                    articleRow.Type = ArticleType.Article.ToString();
                    articleRow.Body = HtmlEditorBody.Content;//HttpUtility.HtmlEncode(htmlBody);
                    articleRow.Subject = TextBoxSubject.Text;
                    articleRow.SearchDesc = SearchDescriptionTextBox.Text;
                    if (this.ParntArticleGuid != Guid.Empty) articleRow.ParentArticleGuid = this.ParntArticleGuid;
                    else articleRow.SetParentArticleGuidNull();
                    articleRow.UpdatedBy = UserContext.Current.UserId;
                    articleRow.UpdatedTime = DateTime.Now;
                    this.ArticleTableAdapter.Update(articleRow);

                    if (oldType == ArticleType.Request.ToString())
                    {
                        string siteUrl = Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.IndexOf(Request.Url.AbsolutePath)) + Request.ApplicationPath + "/";

                        Micajah.Common.Dal.OrganizationDataSet.UserDataTable users = Micajah.Common.Bll.Providers.UserProvider.GetUsers(
                            UserContext.Current.SelectedOrganization.OrganizationId,
                            UserContext.Current.SelectedInstance.InstanceId,
                            new string[] { "InstAdmin" });

                        ArrayList admins = new ArrayList();
                        ArrayList SendTo = new ArrayList();

                        foreach (Micajah.Common.Dal.OrganizationDataSet.UserRow row in users)
                            admins.Add(row.Email);

                        SendTo.AddRange(admins);

                        MainDataSetTableAdapters.EmailsTableAdapter emailsTableAdapter = new MainDataSetTableAdapters.EmailsTableAdapter();
                        foreach (DataRow row in emailsTableAdapter.GetArticleEmails(ArticleGuid).Rows)
                        {
                            if (!SendTo.Contains(row["UserEmail"].ToString()))
                                SendTo.Add(row["UserEmail"].ToString());
                        }

                        foreach (DataRow row in emailsTableAdapter.GetUnsubscribedEmails(articleRow.ArticleGuid, UserContext.Current.SelectedInstance.InstanceId).Rows)
                            SendTo.Remove(row["UserEmail"].ToString());

                        string SendToList = string.Empty;
                        bool separated = false;
                        for (int i = 0; i < SendTo.Count; i++)
                        {
                            if (!separated && !admins.Contains(SendTo[i].ToString()))
                            {
                                SendToList += "<br><br>Commentors:<br>" + SendTo[i].ToString();
                                separated = true;
                            }
                            else
                                SendToList += ", " + SendTo[i].ToString();
                        }
                        SendToList = SendToList.Remove(0,1);

                        if (SendTo.Count > 0)
                        {
                            for (int i = 0; i < SendTo.Count; i++)
                            {
                                StringBuilder body = new StringBuilder((string)this.GetLocalResourceObject("EmailBody_ArticleCreated"));
                                body.Replace("{OrgName}", UserContext.Current.SelectedOrganization.Name);
                                body.Replace("{InstName}", UserContext.Current.SelectedInstance.Name);
                                body.Replace("{ArticleName}", articleRow.Subject);
                                body.Replace("{ArticleUrl}", siteUrl + string.Format(CultureInfo.CurrentCulture, "?i={0}&t={1}", articleRow.DepartmentGuid.ToString("N"), articleRow.ArticleGuid.ToString("N")));
                                body.Replace("{ArticleText}", HtmlEditorBody.Content);
                                body.Replace("{AuthorName}", UserContext.Current.FirstName + " " + UserContext.Current.LastName);
                                body.Replace("{AuthorEmail}", string.IsNullOrEmpty(UserContext.Current.Email) ? string.Empty : string.Format(CultureInfo.CurrentCulture, "<a href=\"mailto:{0}\" target=\"_blank\">{0}</a>", UserContext.Current.Email));
                                body.Replace("{ImageUrl}", siteUrl + Micajah.Common.Configuration.FrameworkConfiguration.Current.WebApplication.Copyright.CompanyLogoImageUrl);
                                if (admins.Contains(SendTo[i]))
                                    body.Replace("{SendToList}", "This message was also sent to:<br>" + SendToList);
                                else
                                    body.Replace("{SendToList}", string.Empty);
                                Utils.SendEmail("noreply@litekb.com", SendTo[i].ToString(), new string[] { }, string.Format((string)this.GetLocalResourceObject("EmailSubjectNewArticle"), articleRow.Subject), body.ToString(), true, Micajah.Common.Configuration.FrameworkConfiguration.Current.WebApplication.Email.SmtpServer, true);
                            }
                        }
                    }
                    altAdapter.DeleteByArticleGuid(articleGuid);
                    foreach (string str in TextBoxAlternateIds.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                        altAdapter.InsertAlt(UserContext.Current.SelectedInstance.InstanceId, articleGuid, str);
                    if (!articleGuid.Equals(Guid.Empty))
                    {
                        DoUploadFile(articleGuid);
                        if (this.ArticleGuid.Equals(Guid.Empty))
                            Response.Redirect("~/ArticleViewAdmin.aspx?mode=edit&id=" + articleGuid.ToString("N"), false);
                    }
                }
            }
        }

        protected void LinkButtonCancelEdit_Click(object sender, EventArgs e)
        {
            if (this.ArticleGuid != Guid.Empty)
                ShowArticleView();
            else Response.Redirect(UserContext.Current.StartPageUrl, true);
        }

        protected void LinkButtonEditArticle_Click(object sender, EventArgs e)
        {
            HtmlEditorBody.Modules.Clear();
            MainDataSet.ArticleRow articleRow = this.GetCurrentArticle();
            if (articleRow != null)
            {
                this.DoArticleEdit(this.GetCurrentArticle());
                HtmlEditorBody.Content = this.ArticleTableAdapter.GetBody(articleRow.ArticleGuid);
                LinkButtonCancelEdit.Visible = true;
            }
        }

        protected void ButtonInactive_Click(object sender, EventArgs e)
        {
            MainDataSet.ArticleRow row = this.GetCurrentArticle();
            if (row != null)
            {
                if (ButtonInactive.Text == (string)this.GetLocalResourceObject("ActivateArticle"))
                {
                    this.ArticleTableAdapter.Activate(this.ArticleGuid);
                    ShowArticleView();
                    ButtonInactive.Text = (string)this.GetLocalResourceObject("InactivateArticle");
                    ButtonInactive.OnClientClick = string.Empty;
                }
                else
                {
                    ShowArticleInactive();
                    litSubjectDelete.Text = row.Subject;
                    int? qty = this.ArticleTableAdapter.GetRelatedArticleCount(row.ArticleGuid);
                    tableInactive.Rows[2].Visible = tableInactive.Rows[3].Visible = tableInactive.Rows[4].Visible = (qty.HasValue && qty.Value > 0);
                }
            }
        }

        protected void ButtonInactiveArticle_Click(object sender, EventArgs e)
        {
            if (this.ArticleGuid != Guid.Empty)
            {
                Guid newartId = this.NewArticleGuid;
                this.ArticleTableAdapter.Inactive(newartId == Guid.Empty ? new Guid?() : new Guid?(newartId), this.ArticleGuid, DateTime.Now, UserContext.Current.UserId);
            }
            Response.Redirect("~/InstanceHomeAdmin.aspx", false);
        }

        protected void ButtonCancelInactive_Click(object sender, EventArgs e)
        {
            ShowArticleView();
        }

        #endregion
    }
}