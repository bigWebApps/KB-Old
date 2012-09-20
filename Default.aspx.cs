using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Linq;
using Micajah.Common.Bll;
using Micajah.Common.Security;
using Micajah.FileService.Client;
using Micajah.Common.Application;
using System.Data;
using Micajah.Common.Bll.Providers;
using System.Collections;

namespace BWA.Knowledgebase
{
    public partial class _Default : System.Web.UI.Page
    {
        #region Members
        private MainDataSetTableAdapters.ArticleTableAdapter m_taArticle = null;
        private Instance m_CurrentInstance = null;
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

        public Instance CurrentInstance
        {
            get
            {
                if (m_CurrentInstance == null && this.InstanceGuid != Guid.Empty)
                    m_CurrentInstance = Micajah.Common.Bll.Providers.InstanceProvider.GetInstance(this.InstanceGuid, Guid.Empty);
                return m_CurrentInstance;
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

        public Guid InstanceGuid
        {
            get
            {
                Guid g = Guid.Empty;
                if (UserContext.SelectedInstanceId != Guid.Empty)
                    g = UserContext.SelectedInstanceId;
                else if (!string.IsNullOrEmpty(Request.QueryString["i"]))
                {
                    try { g = new Guid(Request.QueryString["i"]); }
                    catch { }
                }
                return g;
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

        protected string[] InstanceAdminEmails
        {
            get
            {
                Micajah.Common.Dal.OrganizationDataSet.UserDataTable users = Micajah.Common.Bll.Providers.UserProvider.GetUsers(
                                            this.CurrentInstance.OrganizationId,
                                            this.InstanceGuid,
                                            new string[] { "InstAdmin" });
                string recipientemails = string.Empty;
                foreach (Micajah.Common.Dal.OrganizationDataSet.UserRow row in users)
                    recipientemails += string.Format(CultureInfo.CurrentCulture, "{0},", row.Email);
                if (!string.IsNullOrEmpty(recipientemails))
                {
                    recipientemails = recipientemails.TrimEnd(new char[] { ',' });
                }
                return recipientemails.Split(',');
            }
        }

        protected string Canonical
        {
            get
            {
                string query = string.Empty;
                if (!String.IsNullOrEmpty(Request.QueryString["t"]))
                    query = "?t=" + Request.QueryString["t"];

                DataView customUrls = Micajah.Common.Bll.Providers.CustomUrlProvider.GetCustomUrls(CurrentInstance.OrganizationId);

                if (customUrls != null && customUrls.Count > 0)
                    foreach (DataRowView drv in customUrls)
                    {
                        if (drv["InstanceId"] != null && drv["InstanceId"].ToString().Length > 0 && new Guid(drv["InstanceId"].ToString()) == this.InstanceGuid)
                        {
                            if (drv["FullCustomUrl"].ToString().Length > 0)
                                return String.Format("http://{0}/{1}", drv["FullCustomUrl"].ToString(), query);
                            else
                                return String.Format("http://{0}/{1}", drv["PartialCustomUrl"].ToString(), query);
                        }
                    }

                return string.Empty;
            }
        }

        public string AlternateId
        {
            get
            {
                return this.GetValueFromUrl("a");
            }
        }

        public string InputUserName
        {
            get
            {
                return this.GetValueFromUrl("un");
            }
        }

        public string InputUserEmail
        {
            get
            {
                return this.GetValueFromUrl("ue");
            }
        }

        public string[] InputListAdmin
        {
            get
            {
                string[] list = new string[] { };
                string values = this.GetValueFromUrl("la");
                if (values.Length > 0)
                    list = values.Split(new char[] { ';', ',' });
                return list;
            }
        }
        #endregion

        #region Handle Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (UserContext.Current != null && UserContext.Current.SelectedInstance != null && this.ArticleGuid != Guid.Empty && !IsPopup)
                {
                    Response.Redirect("ArticleViewAdmin.aspx?id=" + this.ArticleGuid.ToString("N"), false);
                    return;
                }
                else if (UserContext.Current != null && UserContext.Current.SelectedInstance != null && !IsPopup)
                {
                    Response.Redirect("InstanceHomeAdmin.aspx", false);
                    return;
                }

                //// todo: popup case
                //if (IsPopup)
                //{
                //}
                string loginUrl = Micajah.Common.Application.WebApplication.LoginProvider.GetLoginUrl();
                string pageUrl = Request.Url.AbsolutePath.ToLowerInvariant().Replace("default.aspx", string.Empty);
                int indexUrl = Request.Url.AbsoluteUri.IndexOf(":" + Request.Url.Port.ToString());
                if (indexUrl <= 0) indexUrl = Request.Url.AbsoluteUri.IndexOf(Request.Url.AbsolutePath);
                string siteUrl = Request.Url.AbsoluteUri.Substring(0, indexUrl) + Request.ApplicationPath;
                if (this.CurrentInstance != null)
                {
                    string orgInstName = string.Format(CultureInfo.CurrentCulture, "{0} {1}", this.CurrentInstance.Organization.Name, this.CurrentInstance.Name);
                    string instanceUrl = pageUrl + (UserContext.SelectedInstanceId == Guid.Empty ? "?i=" + this.InstanceGuid.ToString("N") : string.Empty);
                    string breadcrumbs = string.Format("<a href=\"{1}\">{0}</a>", (string)this.GetLocalResourceObject("Home"), instanceUrl);

                    #region Load Article
                    MainDataSet.ArticleDataTable dtArticle = null;
                    MainDataSet.ArticleRow articleRow = null;
                    StringBuilder textHeader = new StringBuilder((string)this.GetLocalResourceObject("Header"));
                    StringBuilder textFooter = new StringBuilder((string)this.GetLocalResourceObject("Footer"));
                    if (this.ArticleGuid != Guid.Empty)
                    {
                        dtArticle = this.ArticleTableAdapter.GetDataByArticleGuid(this.ArticleGuid);
                        if (dtArticle.Count > 0)
                            articleRow = dtArticle[0];

                        if (articleRow == null || articleRow.Deleted)
                            Response.Redirect(instanceUrl, false);
                    }
                    else if (this.AlternateId != string.Empty)
                    {
                        string strAltId = this.AlternateId;
                        MainDataSetTableAdapters.AlternateIdTableAdapter taAlternateId = new MainDataSetTableAdapters.AlternateIdTableAdapter();
                        Guid? articleGuid = null;
                        articleGuid = taAlternateId.GetArticleGuid(strAltId, this.InstanceGuid);
                        int idx = 0;
                        if (!articleGuid.HasValue)
                        {
                            idx = strAltId.LastIndexOf('/');
                            if (idx > 0)
                            {
                                strAltId = strAltId.Substring(0, idx + 1);
                                articleGuid = taAlternateId.GetArticleGuid(strAltId, this.InstanceGuid);
                            }
                        }
                        if (articleGuid.HasValue)
                        {
                            dtArticle = this.ArticleTableAdapter.GetDataByArticleGuid(articleGuid.Value);
                            if (dtArticle.Count > 0)
                                articleRow = dtArticle[0];

                            if (articleRow == null || articleRow.Deleted)
                                Response.Redirect(instanceUrl, false);
                        }
                    }
                    //else
                    //{
                    //Response.Redirect(instanceUrl, false);
                    //return;
                    //}
                    #endregion

                    #region Post Comment
                    // before write a comment we has to chek a new posted comment
                    if (!string.IsNullOrEmpty(Request.Form["Name"]) && !string.IsNullOrEmpty(Request.Form["Comment"]))
                    {
                        using (MainDataSetTableAdapters.CommentTableAdapter taComment = new MainDataSetTableAdapters.CommentTableAdapter())
                        {
                            string postName = Request.Form["Name"];
                            string postEmail = Request.Form["Email"];
                            if (postEmail == null) postEmail = string.Empty;
                            string postComment = Request.Form["Comment"];
                            string honeyPot = Request.Form["hpot"];
                            DateTime dtNow = DateTime.Now;

                            if (honeyPot == "micajah@@kb") // honeypot protection
                            {
                                if (articleRow == null)
                                {
                                    Guid newId = Guid.NewGuid();
                                    // create a request
                                    if (this.ArticleTableAdapter.Insert(newId,
                                        this.InstanceGuid,
                                        new Guid?(),
                                        ArticleType.Request.ToString(),
                                        (this.AlternateId != string.Empty) ? this.AlternateId : HttpUtility.HtmlEncode(Utils.ShortCommentText(postComment, 50)),
                                        (string)this.GetLocalResourceObject("BodyNotArticle"),
                                        string.Empty,
                                        0,
                                        0,
                                        0,
                                        false,
                                        new DateTime?(),
                                        new DateTime?(),
                                        new DateTime?(),
                                        new Guid?(),
                                        new Guid?(),
                                        new Guid?()) > 0)
                                    {
                                        dtArticle = this.ArticleTableAdapter.GetDataByArticleGuid(newId);
                                        if (dtArticle.Count > 0)
                                            articleRow = dtArticle[0];
                                        MainDataSet.CommentDataTable commentDataTable = taComment.InsertComment(newId,
                                            postName,
                                            postEmail,
                                            string.Format("{0} ({1})", Request.UserHostName, Request.UserHostAddress),
                                            HttpUtility.HtmlEncode(Utils.ShortCommentText(postComment, 50)),
                                            HttpUtility.HtmlEncode(postComment),
                                            false,
                                            true,
                                            DateTime.Now, true);

                                        if (commentDataTable != null && commentDataTable.Rows.Count > 0)
                                        {
                                            string subj;
                                            subj = (this.AlternateId != string.Empty) ? this.AlternateId : HttpUtility.HtmlEncode(Utils.ShortCommentText(postComment, 50));

                                            ArrayList SendTo = new ArrayList();
                                            SendTo.AddRange(this.InputListAdmin);
                                            SendTo.AddRange(InstanceAdminEmails);

                                            MainDataSetTableAdapters.EmailsTableAdapter emailsTableAdapter = new MainDataSetTableAdapters.EmailsTableAdapter();
                                            foreach (DataRow row in emailsTableAdapter.GetArticleEmails(ArticleGuid).Rows)
                                            {
                                                if (!SendTo.Contains(row["UserEmail"].ToString()))
                                                    SendTo.Add(row["UserEmail"].ToString());
                                            }

                                            foreach (DataRow row in emailsTableAdapter.GetUnsubscribedEmails(ArticleGuid, this.InstanceGuid).Rows)
                                                SendTo.Remove(row["UserEmail"].ToString());

                                            string SendToList = string.Empty;
                                            bool separated = false;
                                            for (int i = 0; i < SendTo.Count; i++)
                                            {
                                                if (!separated && !InputListAdmin.Contains(SendTo[i].ToString()) && !InstanceAdminEmails.Contains(SendTo[i].ToString()))
                                                {
                                                    SendToList += "<br><br>Commentors:<br>" + SendTo[i].ToString();
                                                    separated = true;
                                                }
                                                else
                                                    SendToList += ", " + SendTo[i].ToString();
                                            }
                                            SendToList = SendToList.Remove(0, 1);

                                            if (SendTo.Count > 0)
                                            {
                                                for (int i = 0; i < SendTo.Count; i++)
                                                {
                                                    StringBuilder body = new StringBuilder((string)this.GetLocalResourceObject("EmailBody_Request"));
                                                    body.Replace("{OrgName}", this.CurrentInstance.Organization.Name);
                                                    body.Replace("{InstName}", this.CurrentInstance.Name);
                                                    body.Replace("{ArticleName}", subj);
                                                    body.Replace("{ArticleUrl}", siteUrl + (UserContext.SelectedInstanceId != Guid.Empty ? "?" : string.Format(CultureInfo.CurrentCulture, "?i={0}&", this.InstanceGuid.ToString("N"))) + string.Format(CultureInfo.CurrentCulture, "t={0}", newId.ToString("N")));
                                                    body.Replace("{ArticleText}", HttpUtility.HtmlEncode(postComment));
                                                    body.Replace("{AuthorName}", postName);
                                                    body.Replace("{AuthorEmail}", string.IsNullOrEmpty(postEmail) ? string.Empty : string.Format(CultureInfo.CurrentCulture, "<a href=\"mailto:{0}\" target=\"_blank\">{0}</a>", postEmail));
                                                    body.Replace("{ImageUrl}", siteUrl + Micajah.Common.Configuration.FrameworkConfiguration.Current.WebApplication.Copyright.CompanyLogoImageUrl);
                                                    if (InputListAdmin.Contains(SendTo[i]) || InstanceAdminEmails.Contains(SendTo[i]))
                                                        body.Replace("{SendToList}", "This message was also sent to:<br>" + SendToList);
                                                    else
                                                        body.Replace("{SendToList}", string.Empty);

                                                    if (body.Length > 0)
                                                    {
                                                        string encrypted = Utils.Encrypt(String.Format("{0}&{1}&{2}&{3}&{4}", SendTo[i].ToString(), 1, commentDataTable[0].CommentId.ToString(CultureInfo.InvariantCulture), InstanceGuid.ToString(), UserContext.SelectedOrganizationId.ToString()), "Dshd*&^*@dsdss", "237w&@2d", "SHA1", 2, "&s2hfyDjuf372*73", 256);
                                                        string url = siteUrl + String.Format("Unsubscribe.aspx?token={0}", encrypted);
                                                        body.Replace("{UnsubscribeFromArticleUrl}", url);
                                                        encrypted = Utils.Encrypt(String.Format("{0}&{1}&{2}&{3}&{4}", SendTo[i].ToString(), 2, commentDataTable[0].CommentId.ToString(CultureInfo.InvariantCulture), InstanceGuid.ToString(), UserContext.SelectedOrganizationId.ToString()), "Dshd*&^*@dsdss", "237w&@2d", "SHA1", 2, "&s2hfyDjuf372*73", 256);
                                                        url = siteUrl + String.Format("Unsubscribe.aspx?token={0}", encrypted);
                                                        body.Replace("{UnsubscribeFromAllUrl}", url);
                                                        Utils.SendEmail("noreply@litekb.com", SendTo[i].ToString(), string.Format((string)this.GetLocalResourceObject("EmailSubjectRequest"), subj), body.ToString(), true, Micajah.Common.Configuration.FrameworkConfiguration.Current.WebApplication.Email.SmtpServer, true);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // post to article
                                    MainDataSet.CommentDataTable commentDataTable = taComment.InsertComment(this.ArticleGuid,
                                        postName,
                                        postEmail,
                                        string.Format("{0} ({1})", Request.UserHostName, Request.UserHostAddress),
                                        HttpUtility.HtmlEncode(Utils.ShortCommentText(postComment, 50)),
                                        HttpUtility.HtmlEncode(postComment),
                                        false,
                                        true,
                                        dtNow, true);

                                    if (commentDataTable != null && commentDataTable.Rows.Count > 0)
                                    {
                                        ArrayList SendTo = new ArrayList();
                                        SendTo.AddRange(this.InputListAdmin);
                                        SendTo.AddRange(InstanceAdminEmails);

                                        MainDataSetTableAdapters.EmailsTableAdapter emailsTableAdapter = new MainDataSetTableAdapters.EmailsTableAdapter();
                                        foreach (DataRow row in emailsTableAdapter.GetArticleEmails(ArticleGuid).Rows)
                                        {
                                            if (!SendTo.Contains(row["UserEmail"].ToString()))
                                                SendTo.Add(row["UserEmail"].ToString());
                                        }

                                        foreach (DataRow row in emailsTableAdapter.GetUnsubscribedEmails(ArticleGuid, this.InstanceGuid).Rows)
                                            SendTo.Remove(row["UserEmail"].ToString());

                                        string SendToList = string.Empty;
                                        bool separated = false;
                                        for (int i = 0; i < SendTo.Count; i++)
                                        {
                                            if (!separated && !InputListAdmin.Contains(SendTo[i].ToString()) && !InstanceAdminEmails.Contains(SendTo[i].ToString()))
                                            {
                                                SendToList += "<br><br>Commentors:<br>" + SendTo[i].ToString();
                                                separated = true;
                                            }
                                            else
                                                SendToList += ", " + SendTo[i].ToString();
                                        }
                                        SendToList = SendToList.Remove(0, 1);

                                        if (SendTo.Count > 0)
                                        {
                                            if (!articleRow.IsUpdatedByNull())
                                            {
                                                string subj = String.Empty;
                                                subj = string.Format((string)this.GetLocalResourceObject("EmailSubjectComment"), articleRow.Subject);

                                                for (int i = 0; i < SendTo.Count; i++)
                                                {
                                                    StringBuilder body = new StringBuilder((string)this.GetLocalResourceObject("EmailBody_PostToArticle"));
                                                    body.Replace("{OrgName}", this.CurrentInstance.Organization.Name);
                                                    body.Replace("{InstName}", this.CurrentInstance.Name);
                                                    body.Replace("{ArticleName}", articleRow.Subject);
                                                    body.Replace("{ArticleUrl}", siteUrl + (UserContext.SelectedInstanceId != Guid.Empty ? "?" : string.Format(CultureInfo.CurrentCulture, "?i={0}&", this.InstanceGuid.ToString("N"))) + string.Format(CultureInfo.CurrentCulture, "t={0}", articleRow.ArticleGuid.ToString("N")));
                                                    body.Replace("{ArticleText}", HttpUtility.HtmlEncode(postComment));
                                                    body.Replace("{AuthorName}", postName);
                                                    body.Replace("{AuthorEmail}", string.IsNullOrEmpty(postEmail) ? string.Empty : string.Format(CultureInfo.CurrentCulture, "<a href=\"mailto:{0}\" target=\"_blank\">{0}</a>", postEmail));
                                                    body.Replace("{ImageUrl}", siteUrl + Micajah.Common.Configuration.FrameworkConfiguration.Current.WebApplication.Copyright.CompanyLogoImageUrl);
                                                    if (InputListAdmin.Contains(SendTo[i]) || InstanceAdminEmails.Contains(SendTo[i]))
                                                        body.Replace("{SendToList}", "This message was also sent to:<br>" + SendToList);
                                                    else
                                                        body.Replace("{SendToList}", string.Empty);

                                                    if (body.Length > 0)
                                                    {
                                                        string encrypted = Utils.Encrypt(String.Format("{0}&{1}&{2}&{3}&{4}", SendTo[i].ToString(), 1, commentDataTable[0].CommentId.ToString(CultureInfo.InvariantCulture), InstanceGuid.ToString(), UserContext.SelectedOrganizationId.ToString()), "Dshd*&^*@dsdss", "237w&@2d", "SHA1", 2, "&s2hfyDjuf372*73", 256);
                                                        string url = siteUrl + String.Format("Unsubscribe.aspx?token={0}", encrypted);
                                                        body.Replace("{UnsubscribeFromArticleUrl}", url);
                                                        encrypted = Utils.Encrypt(String.Format("{0}&{1}&{2}&{3}&{4}", SendTo[i].ToString(), 2, commentDataTable[0].CommentId.ToString(CultureInfo.InvariantCulture), InstanceGuid.ToString(), UserContext.SelectedOrganizationId.ToString()), "Dshd*&^*@dsdss", "237w&@2d", "SHA1", 2, "&s2hfyDjuf372*73", 256);
                                                        url = siteUrl + String.Format("Unsubscribe.aspx?token={0}", encrypted);
                                                        body.Replace("{UnsubscribeFromAllUrl}", url);
                                                        Utils.SendEmail("noreply@litekb.com", SendTo[i].ToString(), subj, body.ToString(), true, Micajah.Common.Configuration.FrameworkConfiguration.Current.WebApplication.Email.SmtpServer, true);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region Generate an Article
                    if (articleRow != null)
                    {
                        if (!articleRow.Deleted)
                        {
                            //Response.Redirect(instanceUrl, false);
                            //return;
                            textHeader = textHeader.Replace("{title}", string.Format(CultureInfo.CurrentCulture, "{2}{0} {1}", this.CurrentInstance.Organization.Name, this.CurrentInstance.Name, string.IsNullOrEmpty(articleRow.Subject) ? string.Empty : articleRow.Subject + " - "));
                            //textHeader = textHeader.Replace("{metakeywords}", articleRow.SearchDesc);
                            textHeader = textHeader.Replace("{metadescription}", articleRow.SearchDesc);
                            textHeader = textHeader.Replace("{canonical}", Canonical);
                            Response.Write(textHeader.ToString());
                            if (!this.IsPopup)
                            {
                                Response.Write("    <div id=\"header\"><div id=\"breadcrumbs\"><span>" + Environment.NewLine);
                                MainDataSet.ArticleDataTable dtBreadcrumbs = this.ArticleTableAdapter.GetRecursiveAllByArticleGuid(articleRow.ArticleGuid);
                                for (int i = dtBreadcrumbs.Count - 1; i >= 0; i--)
                                {
                                    if (dtBreadcrumbs[i].ArticleGuid == articleRow.ArticleGuid)
                                        breadcrumbs += string.Format("&nbsp; > &nbsp;{0}", dtBreadcrumbs[i].Subject);
                                    else
                                        breadcrumbs += string.Format("&nbsp; > &nbsp;<a href=\"{1}\">{0}</a>", dtBreadcrumbs[i].Subject, pageUrl + (UserContext.SelectedInstanceId != Guid.Empty ? "?" : "?i=" + this.InstanceGuid.ToString("N") + "&") + "t=" + dtBreadcrumbs[i].ArticleGuid.ToString("N"));
                                }
                                Response.Write(breadcrumbs);
                                Response.Write("</span></div><div id=\"search\">" + Environment.NewLine);
                                Response.Write(string.Format("<form action=\"{0}\" method=\"post\"><input type=\"text\" name=\"search\" size=\"40\" />{1}<input type=\"submit\" value=\"Search\" /></form>{1}", "SearchResult.aspx" + (UserContext.SelectedInstanceId == Guid.Empty ? "?i=" + this.InstanceGuid.ToString("N") : string.Empty), Environment.NewLine));
                                Response.Write("</div></div>");
                            }
                            // write a header
                            // write a body
                            Response.Write("    <div id=\"title\">" + Environment.NewLine);
                            Response.Write(string.Format("        <h1>{0}</h1>{1}", articleRow.Subject, Environment.NewLine));
                            Response.Write("    </div>" + Environment.NewLine);
                            if (articleRow.Type == ArticleType.Request.ToString())
                            {
                                //Response.Write("    <hr />" + Environment.NewLine);
                                Response.Write("    <div id=\"article\">" + Environment.NewLine);
                                Response.Write((string)this.GetLocalResourceObject("ArticleDoesNotExist"));
                                Response.Write("    </div>" + Environment.NewLine);
                            }
                            else
                            {
                                this.ArticleTableAdapter.IncReview(articleRow.ArticleGuid);
                                string body = this.ArticleTableAdapter.GetBody(articleRow.ArticleGuid);
                                if (body.Replace("<p>", string.Empty).Replace("</p>", string.Empty).Trim().Length > 0)
                                {
                                    body = body.Replace("<h2>", "<h2><span>");
                                    body = body.Replace("<H2>", "<H2><span>");
                                    body = body.Replace("</h2>", "</span></h2>");
                                    body = body.Replace("</H2>", "</span></H2>");
                                    //Response.Write("    <hr />" + Environment.NewLine);
                                    Response.Write("    <div id=\"article\">" + Environment.NewLine);
                                    Response.Write(body);
                                    Response.Write("    </div>" + Environment.NewLine);
                                }
                            }
                            // write a attachments
                            using (Micajah.FileService.Client.Dal.MetaDataSetTableAdapters.FileTableAdapter taFile = new Micajah.FileService.Client.Dal.MetaDataSetTableAdapters.FileTableAdapter())
                            {
                                Micajah.FileService.Client.Dal.MetaDataSet.FileDataTable dtFiles = taFile.GetFiles(this.CurrentInstance.OrganizationId, this.CurrentInstance.InstanceId, "Article", articleRow.ArticleGuid.ToString("N"), false);
                                StringBuilder stringAttachments = new StringBuilder();
                                foreach (Micajah.FileService.Client.Dal.MetaDataSet.FileRow frow in dtFiles)
                                {
                                    string ext = Path.GetExtension(frow.Name);
                                    string mimeType = Micajah.FileService.Client.MimeType.GetMimeType(ext);
                                    if (!Micajah.FileService.Client.MimeType.IsImageType(mimeType) &&
                                        !Micajah.FileService.Client.MimeType.IsFlash(mimeType))
                                        stringAttachments = stringAttachments.AppendFormat("        <li><a href=\"{1}\">{0}</a></li>", frow.Name, Access.GetFileUrl(frow.FileUniqueId, frow.OrganizationId, frow.DepartmentId));
                                }
                                if (stringAttachments.Length > 0)
                                {
                                    stringAttachments = stringAttachments.Insert(0, "    <div id=\"attachments\">" + Environment.NewLine + "     <ol>" + Environment.NewLine);
                                    stringAttachments = stringAttachments.AppendLine("     </ol>" + Environment.NewLine + "    </div>");
                                    //Response.Write("    <hr />" + Environment.NewLine);
                                    Response.Write(stringAttachments.ToString());
                                }
                            }
                            // write a child article list
                            MainDataSet.ArticleDataTable dtList = this.ArticleTableAdapter.GetChildArticles(new Guid?(articleRow.ArticleGuid), 1024);
                            //if (dtList.Count > 0)
                            Response.Write("    <div id=\"related\">" + Environment.NewLine + "     <ol>" + Environment.NewLine);
                            foreach (MainDataSet.ArticleRow arow in dtList.OrderBy(x => x.Subject))
                            {
                                Response.Write(this.GetRelatedString(arow, pageUrl));
                            }
                            if (articleRow.IsParentArticleGuidNull())
                            {
                                //this.CurrentInstance.Name
                                Response.Write(string.Format("        <li><a style='color:gray' href=\"{0}\">{1}</a>{2}          <span style=\"display:block;\">{3}</span>{2}        </li>",
                                    pageUrl + (UserContext.SelectedInstanceId == Guid.Empty ? "?i=" + this.InstanceGuid.ToString("N") : string.Empty),
                                    "< " + (string)this.GetLocalResourceObject("BackTo") + " " + this.CurrentInstance.Name + " " + (string)this.GetLocalResourceObject("Home"),
                                    Environment.NewLine,
                                    string.Empty));
                                //this.CurrentInstance.Description));
                            }
                            else
                            {
                                dtArticle = this.ArticleTableAdapter.GetDataByArticleGuid(articleRow.ParentArticleGuid);
                                if (dtArticle.Count > 0)
                                {
                                    MainDataSet.ArticleRow row = dtArticle[0];
                                    row.Subject = "< " + (string)this.GetLocalResourceObject("BackTo") + " " + row.Subject;
                                    Response.Write(this.GetRelatedString(row, pageUrl, true));
                                }
                            }
                            //if (dtList.Count > 0)
                            Response.Write("     </ol>" + Environment.NewLine + "    </div>" + Environment.NewLine);

                            // write a comments
                            using (MainDataSetTableAdapters.CommentTableAdapter taComments = new MainDataSetTableAdapters.CommentTableAdapter())
                            {
                                MainDataSet.CommentDataTable dtComments = taComments.GetDataByArticleGuid(articleRow.ArticleGuid);
                                if (dtComments.Count > 0)
                                {
                                    //Response.Write("    <hr />" + Environment.NewLine);
                                    Response.Write("    <div id=\"comments\">" + Environment.NewLine + "     <ol>" + Environment.NewLine);
                                    foreach (MainDataSet.CommentRow crow in dtComments)
                                    {
                                        Response.Write(string.Format("     <li><a name=\"{0}\">{1}</a>{2}           <span style=\"display:block;\">{3}</span></li>",
                                            crow.CommentId.ToString(),
                                            crow.UserName,
                                            Environment.NewLine,
                                            crow.Body.Replace(Environment.NewLine, "<br>")));
                                    }
                                    Response.Write("     </ol>" + Environment.NewLine + "    </div>" + Environment.NewLine);
                                }
                            }
                            // write a footer
                            string commentaction = "default.aspx?";
                            if (UserContext.SelectedInstanceId == Guid.Empty)
                                commentaction += "i=" + this.InstanceGuid.ToString("N") + "&";
                            commentaction += "t=" + articleRow.ArticleGuid.ToString("N");
                            if (this.InputUserName.Length > 0)
                                commentaction += "&un=" + this.InputUserName;
                            if (this.InputUserEmail.Length > 0)
                                commentaction += "&ue=" + this.InputUserEmail;
                            if (this.InputListAdmin.Length > 0)
                                commentaction += "&la=" + string.Join(";", this.InputListAdmin);

                            textFooter = textFooter.Replace("{username}", this.InputUserName);
                            textFooter = textFooter.Replace("{useremail}", this.InputUserEmail);
                            textFooter = textFooter.Replace("{homeurl}", instanceUrl);
                            if (articleRow != null)
                                textFooter = textFooter.Replace("{editurl}", string.Format("<a href=\"ArticleViewAdmin.aspx?id={0}&mode=edit\">Edit</a>", articleRow.ArticleGuid.ToString()));
                            else
                                textFooter = textFooter.Replace("{editurl}", string.Empty);
                            textFooter = textFooter.Replace("{micajahurl}", loginUrl);
                            textFooter = textFooter.Replace("{currentyear}", DateTime.Now.Year.ToString(CultureInfo.InvariantCulture));
                            textFooter = textFooter.Replace("{commentaction}", commentaction);
                            //textFooter = textFooter.Replace("{trackingcode}", string.Empty);

                            string trackingcode = string.Empty;
                            Setting trackCode = this.CurrentInstance.Settings.FindByShortName("TrackingCode");
                            if (trackCode != null && !string.IsNullOrEmpty(trackCode.Value))
                                trackingcode = trackCode.Value;
                            textFooter = textFooter.Replace("{trackingcode}", trackingcode + (string)this.GetLocalResourceObject("GlobalTrackingCode"));

                            Response.Write(textFooter);
                            return;
                        }
                    }
                    else if (this.AlternateId != string.Empty)
                    {
                        // add new article mode
                        textHeader = textHeader.Replace("{title}", (string)this.GetLocalResourceObject("NewArticle"));
                        textHeader = textHeader.Replace("{metadescription}", string.Empty);
                        // write a header
                        Response.Write(textHeader.ToString());
                        if (!this.IsPopup)
                        {
                            Response.Write("    <div id=\"header\"><div id=\"breadcrumbs\"><span>" + Environment.NewLine);
                            breadcrumbs += "&nbsp; > &nbsp;" + this.AlternateId;
                            //breadcrumbs += "&nbsp; > &nbsp;" + (string)this.GetLocalResourceObject("NewArticle");
                            Response.Write(breadcrumbs);
                            Response.Write("</span></div><div id=\"search\">" + Environment.NewLine);
                            Response.Write(string.Format("<form action=\"{0}\" method=\"post\"><input type=\"text\" name=\"search\" size=\"40\" /><input type=\"submit\" value=\"Search\" /></form>{1}", "SearchResult.aspx" + (UserContext.SelectedInstanceId == Guid.Empty ? "?i=" + this.InstanceGuid.ToString("N") : string.Empty), Environment.NewLine));
                            Response.Write("</div></div>");
                        }
                        // write a body
                        Response.Write("    <div id=\"title\">" + Environment.NewLine);
                        Response.Write(string.Format("        <H1>{0}</H1>{1}", (string)this.GetLocalResourceObject("ArticleIsNotFoundTitle"), Environment.NewLine));
                        Response.Write("    </div>" + Environment.NewLine);
                        //Response.Write("    <hr />" + Environment.NewLine);
                        Response.Write("    <div id=\"article\">" + Environment.NewLine);
                        Response.Write(string.Format("        <P>{0}</P>{1}", (string)this.GetLocalResourceObject("ArticleDoesNotExist"), Environment.NewLine));
                        Response.Write("    </div>" + Environment.NewLine);
                        //  = (string)this.GetLocalResourceObject("ArticleDoesNotExist");
                        // write a footer
                        string commentaction = "default.aspx?";
                        if (UserContext.SelectedInstanceId == Guid.Empty)
                            commentaction += "i=" + this.InstanceGuid.ToString("N") + "&";
                        commentaction += "a=" + this.AlternateId;
                        if (this.InputUserName.Length > 0)
                            commentaction += "&un=" + this.InputUserName;
                        if (this.InputUserEmail.Length > 0)
                            commentaction += "&ue=" + this.InputUserEmail;
                        if (this.InputListAdmin.Length > 0)
                            commentaction += "&la=" + string.Join(";", this.InputListAdmin);

                        textFooter = textFooter.Replace("{username}", this.InputUserName);
                        textFooter = textFooter.Replace("{useremail}", this.InputUserEmail);
                        textFooter = textFooter.Replace("{homeurl}", instanceUrl);
                        if (articleRow != null)
                            textFooter = textFooter.Replace("{editurl}", string.Format("<a href=\"ArticleViewAdmin.aspx?id={0}&mode=edit\">Edit</a>", articleRow.ArticleGuid.ToString()));
                        else
                            textFooter = textFooter.Replace("{editurl}", string.Empty);
                        textFooter = textFooter.Replace("{micajahurl}", loginUrl);
                        textFooter = textFooter.Replace("{currentyear}", DateTime.Now.Year.ToString(CultureInfo.InvariantCulture));
                        textFooter = textFooter.Replace("{commentaction}", commentaction);
                        //textFooter = textFooter.Replace("{trackingcode}", string.Empty);

                        string trackingcode = string.Empty;
                        Setting trackCode = this.CurrentInstance.Settings.FindByShortName("TrackingCode");
                        if (trackCode != null && !string.IsNullOrEmpty(trackCode.Value))
                            trackingcode = trackCode.Value;
                        textFooter = textFooter.Replace("{trackingcode}", trackingcode + (string)this.GetLocalResourceObject("GlobalTrackingCode"));

                        Response.Write(textFooter);
                        return;
                    }
                    #endregion

                    #region Generate the Instance Home page

                    StringBuilder textHeaderInst = new StringBuilder((string)this.GetLocalResourceObject("Header_Inst"));
                    StringBuilder textFooterInst = new StringBuilder((string)this.GetLocalResourceObject("Footer_Inst"));
                    textHeaderInst = textHeaderInst.Replace("{title}", orgInstName + " - Home");
                    textHeaderInst = textHeaderInst.Replace("{metadescription}", "KB Home Page");
                    textHeaderInst = textHeaderInst.Replace("{searchaction}", "SearchResult.aspx" + (UserContext.SelectedInstanceId == Guid.Empty ? "?i=" + this.InstanceGuid.ToString("N") : string.Empty));
                    textHeaderInst = textHeaderInst.Replace("{orgInstName}", orgInstName);
                    textHeaderInst = textHeaderInst.Replace("{orgUrl}", instanceUrl);
                    textHeaderInst = textHeaderInst.Replace("{canonical}", Canonical);

                    //textHeaderInst = textHeaderInst.Replace("{orgUrl}", this.CurrentInstance.Organization.WebsiteUrl);
                    Response.Write(textHeaderInst.ToString());
                    string listTreeView = string.Empty;
                    MainDataSet.ArticleDataTable articleTable = this.ArticleTableAdapter.GetRecursiveByDepartmentGuid(this.InstanceGuid);
                    this.SortRecursiveTable(ref listTreeView, ref articleTable, null, pageUrl);
                    Response.Write(listTreeView);
                    textFooterInst = textFooterInst.Replace("{micajahurl}", loginUrl);
                    textFooterInst = textFooterInst.Replace("{currentyear}", DateTime.Now.Year.ToString(CultureInfo.InvariantCulture));
                    //textFooterInst = textFooterInst.Replace("{trackingcode}", string.Empty);

                    string trackingcode2 = string.Empty;
                    Setting trackCode2 = this.CurrentInstance.Settings.FindByShortName("TrackingCode");
                    if (trackCode2 != null && !string.IsNullOrEmpty(trackCode2.Value))
                        trackingcode2 = trackCode2.Value;
                    textFooterInst = textFooterInst.Replace("{trackingcode}", trackingcode2 + (string)this.GetLocalResourceObject("GlobalTrackingCode"));

                    Response.Write(textFooterInst);
                    #endregion
                }
                else if (UserContext.SelectedOrganizationId != Guid.Empty && UserContext.SelectedInstanceId == Guid.Empty)
                {
                    Response.Redirect("OrganizationHome.aspx", false);
                    return;
                }
                else
                    Response.Redirect(loginUrl, false);

            }
        }
        #endregion

        #region Private Methods
        private string GetValueFromUrl(string paramenter)
        {
            string res = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString[paramenter]))
                res = HttpUtility.UrlDecode(Request.QueryString[paramenter]);
            return res;
        }

        private string GetRelatedString(MainDataSet.ArticleRow arow, string pageUrl)
        {
            return GetRelatedString(arow, pageUrl, false);
        }

        private string GetRelatedString(MainDataSet.ArticleRow arow, string pageUrl, bool isBack)
        {
            //string body = arow.Body;
            //if (!string.IsNullOrEmpty(body))
            //{
            //    HtmlAgilityPack.HtmlNode node = HtmlAgilityPack.HtmlNode.CreateNode(body);
            //    if (node != null)
            //        body = node.InnerText;
            //}
            return string.Format("        <li><a {3} href=\"{0}\">{1}</a>{2}          </li>",
                pageUrl + (UserContext.SelectedInstanceId != Guid.Empty ? "?" : "?i=" + this.InstanceGuid.ToString("N") + "&") + "t=" + arow.ArticleGuid.ToString("N"),
                arow.Subject,
                Environment.NewLine,
                isBack ? "style='color:gray'" : string.Empty);
            // body); "<span style=\"display:block;\">{3}</span>"
        }

        private string LICnt(int cnt)
        {
            string result = "<li>";
            return result;
        }

        private void SortRecursiveTable(ref string output, ref MainDataSet.ArticleDataTable input, Guid? parentArticleId, string pageUrl)
        {
            OrderedEnumerableRowCollection<MainDataSet.ArticleRow> query = null;
            if (parentArticleId.HasValue)
                query = input.Where(w => w.IsParentArticleGuidNull() ? false : w.ParentArticleGuid.Equals(parentArticleId.Value)).OrderBy(o => o.Subject);
            else
                query = input.Where(w => w.IsParentArticleGuidNull()).OrderBy(o => o.Subject);
            int cnt = query.Count();
            if (cnt > 0)
                output += "<ul>";
            foreach (MainDataSet.ArticleRow row in query)
            {
                output += string.Format(CultureInfo.CurrentCulture, "<li {4}><a style=\"vertical-align:top{5}\" href=\"{3}?{0}t={1}\">{2}</a>", UserContext.SelectedInstanceId == Guid.Empty ? "i=" + this.InstanceGuid.ToString("N") + "&" : string.Empty, row.ArticleGuid.ToString("N"), row.Subject, pageUrl, row.IsParentArticleGuidNull() ? "style=\"padding-top:10px;\"" : string.Empty, row.IsParentArticleGuidNull() ? ";font-size:14pt\"" : string.Empty);
                SortRecursiveTable(ref output, ref input, row.ArticleGuid, pageUrl);
            }
            if (cnt > 0)
                output += "</ul>";
        }
        #endregion
    }
}