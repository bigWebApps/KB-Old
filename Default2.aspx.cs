using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.FileService.WebControls;
using Micajah.Common.Security;
using System.Web.UI.HtmlControls;

namespace BWA.Knowledgebase
{
    public partial class _Default2 : System.Web.UI.Page
    {
        #region Members
        private MainDataSetTableAdapters.ArticleTableAdapter m_taArticle = null;
        #endregion

        #region Properties

        protected MainDataSetTableAdapters.ArticleTableAdapter ArticleTableAdapter
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
                if (!string.IsNullOrEmpty(Request.QueryString["t"]))
                {
                    try { g = new Guid(Request.QueryString["t"]); }
                    catch { }
                }
                return g;
            }
        }

        public string AlternateId
        {
            get
            {
                string res = string.Empty;
                if (!string.IsNullOrEmpty(Request.QueryString["a"]))
                    res = HttpUtility.UrlDecode(Request.QueryString["a"]);
                return res;
            }
        }

        protected bool IsPopup
        {
            get
            {
                bool res = false;
                if (!string.IsNullOrEmpty(Request.QueryString["popup"]))
                    res = Boolean.TryParse(Request.QueryString["popup"], out res);
                return res;
            }
        }

        #endregion

        #region Handle Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (IsPopup)
                {
                    Master.VisibleBreadcrumbs = false;
                    Master.VisibleHeader = false;
                    Master.VisibleHelpLink = false;
                    Master.VisibleFooter = false;
                    //divBody.Visible = false;
                    PostCommentCtrl.VisibleLine = false;
                }
                Master.AutoGenerateBreadcrumbs = false;
                //Master.VisiblePageTitle = false;
                UserContext.Breadcrumbs.Clear();
                MainDataSet.ArticleDataTable dtArticle = null;
                if (UserContext.Current != null && UserContext.Current.SelectedInstance != null && this.ArticleGuid != Guid.Empty && !IsPopup)
                {
                    Response.Redirect("ArticleViewAdmin.aspx?id=" + this.ArticleGuid.ToString("N"), false);
                    return;
                }
                if (this.Master.InstanceGuid != Guid.Empty)
                {
                    string instName = (string)this.ArticleTableAdapter.GetInstanceName(this.Master.InstanceGuid);
                    if (!string.IsNullOrEmpty(instName))
                    {
                        //UploadControlSettings.DepartmentName = instName;
                        //UploadControlSettings.DepartmentId = this.Master.InstanceGuid;
                        //MainDataSetTableAdapters.Mc_InstanceTableAdapter instance = new MainDataSetTableAdapters.Mc_InstanceTableAdapter();
                        //UploadControlSettings.OrganizationId = instance.GetDataByInstanceId(this.Master.InstanceGuid)[0].OrganizationId;
                        if (!IsPopup)
                            UserContext.Breadcrumbs.Add((string)this.GetLocalResourceObject("Home"), "~/InstanceHome.aspx?i=" + this.Master.InstanceGuid.ToString("N"), string.Empty, false);
                        //SearchArticleCtrl.InstanceGuid = this.InstanceGuid;
                        if (this.ArticleGuid != Guid.Empty)
                        {
                            //if (UserContext.Current != null)
                            //{
                            //    Response.Redirect("ArticleViewAdmin.aspx?id=" + this.ArticleGuid.ToString("N"), true);
                            //    return;
                            //}
                            // view mode
                            dtArticle = this.ArticleTableAdapter.GetDataByArticleGuid(this.ArticleGuid);
                            if (dtArticle.Count > 0)
                            {
                                this.DoArticleView(dtArticle[0]);
                                AttachmentListCtrl.ArticleGuid = dtArticle[0].ArticleGuid;
                                //ImageListCtrl.ArticleGuid = dtArticle[0].ArticleGuid;
                                //FileListCtrl.ArticleGuid = dtArticle[0].ArticleGuid;
                                //VideoListCtrl.ArticleGuid = dtArticle[0].ArticleGuid;
                                return;
                            }
                        }
                        else if (this.AlternateId != string.Empty)
                        {
                            // view mode
                            string strAltId = this.AlternateId;
                            MainDataSetTableAdapters.AlternateIdTableAdapter taAlternateId = new MainDataSetTableAdapters.AlternateIdTableAdapter();
                            Guid? articleGuid = null;
                            articleGuid = taAlternateId.GetArticleGuid(strAltId, this.Master.InstanceGuid);
                            int idx = 0;
                            //if (!articleGuid.HasValue)
                            //{
                            //    idx = strAltId.LastIndexOf('?');
                            //    if (idx > 0)
                            //    {
                            //        strAltId = strAltId.Substring(0, idx);
                            //        articleGuid = taAlternateId.GetArticleGuid(strAltId, this.Master.InstanceGuid);
                            //    }
                            //}
                            if (!articleGuid.HasValue)
                            {
                                idx = strAltId.LastIndexOf('/');
                                if (idx > 0)
                                {
                                    strAltId = strAltId.Substring(0, idx + 1);
                                    articleGuid = taAlternateId.GetArticleGuid(strAltId, this.Master.InstanceGuid);
                                }
                            }
                            if (articleGuid.HasValue)
                            {
                                dtArticle = this.ArticleTableAdapter.GetDataByArticleGuid(articleGuid.Value);
                                if (dtArticle.Count > 0)
                                {
                                    AttachmentListCtrl.ArticleGuid = dtArticle[0].ArticleGuid;
                                    //ImageListCtrl.ArticleGuid = dtArticle[0].ArticleGuid;
                                    //FileListCtrl.ArticleGuid = dtArticle[0].ArticleGuid;
                                    //VideoListCtrl.ArticleGuid = dtArticle[0].ArticleGuid;
                                    this.DoArticleView(dtArticle[0]);
                                    return;
                                }
                            }
                            else
                            {
                                // add new article mode
                                if (!IsPopup)
                                {
                                    UserContext.Breadcrumbs.Add(this.AlternateId, string.Empty, string.Empty, false);
                                    divBody.InnerHtml = (string)this.GetLocalResourceObject("ArticleDoesNotExist");
                                }
                                PanelArticle.Visible = true;
                                Master.CustomName = (string)this.GetLocalResourceObject("NewArticle");
                                litSubject.Text = (string)this.GetLocalResourceObject("ArticleIsNotFoundTitle");
                                ArticlesListCtrl.Visible = false;
                                CommentsListCtrl.Visible = false;
                                //PostCommentCtrl.ReceiverName = Utils.GetInstanceUserName(this.Master.InstanceGuid);
                                PostCommentCtrl.InstanceGuid = this.Master.InstanceGuid;
                                PostCommentCtrl.ArticleGuid = Guid.Empty;
                                return;
                            }
                        }
                        else
                        {
                            Response.Redirect("InstanceHome.aspx?i=" + this.Master.InstanceGuid.ToString("N"), false);
                            return;
                        }
                    }
                }
                PanelArticle.Visible = false;
                Response.Redirect(Micajah.Common.Application.WebApplication.LoginProvider.GetLoginUrl(), false);
                //Master.ErrorMessage = (string)this.GetLocalResourceObject("ArticleIsNotFound");
            }
        }

        protected void PostCommentCtrl_CommentPosted(object sender, ArticleEventArgs e)
        {
            if (e.IsNew)
            {
                using (MainDataSetTableAdapters.AlternateIdTableAdapter taAlternateId = new MainDataSetTableAdapters.AlternateIdTableAdapter())
                {
                    taAlternateId.InsertAlt(this.Master.InstanceGuid, e.ArticleGuid, this.AlternateId);
                }
                PanelArticle.Visible = false;
                PanelSuccess.Visible = true;
                //LabelCommentPosted.Text = string.Format(LabelCommentPosted.Text, PostCommentCtrl.ReceiverName);
                //Response.Redirect(string.Format("~/?i={0}&t={1}#comments", this.Master.InstanceGuid.ToString("N"), e.ArticleGuid.ToString("N")), false);
            }
            else
            {
                CommentsListCtrl.DataBind();
                CommentsListCtrl.RegisterGoAnchor();
            }
        }
        #endregion

        #region Private Methods
        private void DoArticleView(MainDataSet.ArticleRow articleRow)
        {
            PanelArticle.Visible = true;
            if (articleRow != null)
            {
                if (!IsPopup)
                    UserContext.Breadcrumbs.AddRange(Utils.GenerateBreadCrumbs(articleRow.ArticleGuid, this.Master.InstanceGuid, false));
                //Master.CustomName = articleRow.Subject;
                Master.Title = articleRow.Subject;
                litSubject.Text = articleRow.Subject;
                HtmlMeta metaKeywords = new HtmlMeta();
                metaKeywords.Name = "keywords";
                metaKeywords.Content = Master.Title;
                Page.Header.Controls.Add(metaKeywords);
                HtmlMeta metaDescription = new HtmlMeta();
                metaDescription.Name = "Description";
                metaDescription.Content = articleRow.SearchDesc;
                Page.Header.Controls.Add(metaDescription);

                if (articleRow.Type == ArticleType.Request.ToString())
                {
                    divBody.InnerHtml = (string)this.GetLocalResourceObject("ArticleDoesNotExist");
                }
                else
                {
                    this.ArticleTableAdapter.IncReview(articleRow.ArticleGuid);
                    string body = this.ArticleTableAdapter.GetBody(articleRow.ArticleGuid);
                    if (!string.IsNullOrEmpty(body))
                        divBody.InnerHtml = body;
                }
                //divType.InnerText = articleRow.Type;
                Guid userId = Guid.Empty;
                if (!articleRow.IsCreatedByNull())
                    userId = articleRow.CreatedBy;
                if (!articleRow.IsUpdatedByNull())
                    userId = articleRow.UpdatedBy;
                //if (userId == Guid.Empty)
                //    PostCommentCtrl.ReceiverName = Utils.GetInstanceUserName(this.Master.InstanceGuid);
                //else
                //{
                //    using (MainDataSetTableAdapters.Mc_UserTableAdapter userAdapter = new MainDataSetTableAdapters.Mc_UserTableAdapter())
                //    {
                //        MainDataSet.Mc_UserDataTable dtUser = userAdapter.GetDataByUserId(userId);
                //        if (dtUser != null && dtUser.Count > 0)
                //            PostCommentCtrl.ReceiverName = dtUser[0].FirstName + " " + dtUser[0].LastName;
                //    }
                //}
                ArticlesListCtrl.ArticleGuid = articleRow.ArticleGuid;
                PostCommentCtrl.InstanceGuid = this.Master.InstanceGuid;
                PostCommentCtrl.ArticleGuid = articleRow.ArticleGuid;
                CommentsListCtrl.ArticleGuid = articleRow.ArticleGuid;
                CommentsListCtrl.DataBind();
            }
        }

        #endregion
    }
}
