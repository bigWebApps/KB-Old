using System;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Micajah.Common.Bll;
using Micajah.Common.Application;
using System.Text;
using Micajah.Common.Security;
using System.Collections;
using System.Data;

namespace BWA.Knowledgebase
{
    public partial class PostCommentControl : System.Web.UI.UserControl
    {
        #region Members
        //private MainDataSet.ArticleRow m_articleRow = null;
        private MainDataSetTableAdapters.ArticleTableAdapter m_taArticle = null;
        #endregion

        #region Events
        public event EventHandler<ArticleEventArgs> CommentPosted;
        #endregion

        #region Properties
        //public string ReceiverName
        //{
        //    get
        //    {
        //        if (ViewState["ReceiverName"] != null)
        //            return (string)ViewState["ReceiverName"];
        //        else return (string)this.GetLocalResourceObject("Administrator");
        //    }
        //    set
        //    {
        //        ViewState["ReceiverName"] = value;
        //    }
        //}

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

        public Unit Width
        {
            get
            {
                Unit w = Unit.Empty;
                w = Unit.Parse(tablePostComment.Width, CultureInfo.CurrentCulture);
                return w;
            }
            set
            {
                if (value.IsEmpty)
                    tablePostComment.Width = "500px";
                else
                    tablePostComment.Width = value.ToString(CultureInfo.CurrentCulture);
            }
        }

        public Guid InstanceGuid
        {
            get
            {
                if (ViewState["InstanceGuid"] != null)
                    return (Guid)ViewState["InstanceGuid"];
                else return Guid.Empty;
            }
            set
            {
                ViewState["InstanceGuid"] = value;
            }
        }

        public Guid ArticleGuid
        {
            get
            {
                if (ViewState["ArticleGuid"] != null)
                    return (Guid)ViewState["ArticleGuid"];
                else return Guid.Empty;
            }
            set
            {
                ViewState["ArticleGuid"] = value;
            }
        }

        public bool VisibleLine
        {
            get
            {
                return (divMain.Attributes["class"] != null && divMain.Attributes["class"] == "pageInfo");
            }
            set
            {
                if (value) divMain.Attributes["class"] = "pageInfo";
                else divMain.Attributes.Remove("class");
            }
        }

        public string ArticleSubject
        {
            get
            {
                if (ViewState["ArticleSubject"] != null)
                    return (string)ViewState["ArticleSubject"];
                else return string.Empty;
            }
            set
            {
                ViewState["ArticleSubject"] = value;
            }
        }

        public MainDataSetTableAdapters.ArticleTableAdapter ArticleTableAdapter
        {
            get
            {
                if (m_taArticle == null)
                    m_taArticle = new MainDataSetTableAdapters.ArticleTableAdapter();
                return m_taArticle;
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

        private MainDataSet.ArticleRow GetCurrentArticle()
        {
            if (!this.ArticleGuid.Equals(Guid.Empty))
            {
                MainDataSet.ArticleDataTable dtArticle = this.ArticleTableAdapter.GetDataByArticleGuid(this.ArticleGuid);
                if (dtArticle.Count > 0)
                    return dtArticle[0];
            }
            return null;
        }

        private void ResetData()
        {
            TextBoxName.Text = TextBoxEmail.Text = TextBoxPhone.Text = TextBoxComment.Text = string.Empty;
        }
        #endregion

        #region Handle Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //LabelHeader.Text = string.Format(LabelHeader.Text, this.ReceiverName);
                ResetData();
            }
        }
        protected void ButtonPostComment_Click(object sender, EventArgs e)
        {
            // check the Honeypot
            if (TextBoxPhone.Text == string.Empty && this.InstanceGuid != Guid.Empty)
            {
                int indexUrl = Request.Url.AbsoluteUri.IndexOf(":" + Request.Url.Port.ToString());
                if (indexUrl <= 0) indexUrl = Request.Url.AbsoluteUri.IndexOf(Request.Url.AbsolutePath);
                string siteUrl = Request.Url.AbsoluteUri.Substring(0, indexUrl) + Request.ApplicationPath + "/";
                using (MainDataSetTableAdapters.CommentTableAdapter taComment = new MainDataSetTableAdapters.CommentTableAdapter())
                {
                    Organization currOrganization = null;
                    MainDataSet.Mc_InstanceRow currInstance = null;
                    using (MainDataSetTableAdapters.Mc_InstanceTableAdapter taInstance = new MainDataSetTableAdapters.Mc_InstanceTableAdapter())
                    {
                        MainDataSet.Mc_InstanceDataTable instances = taInstance.GetDataByInstanceId(this.InstanceGuid);
                        if (instances.Count > 0)
                        {
                            currInstance = instances[0];
                            currOrganization = Micajah.Common.Bll.Providers.OrganizationProvider.GetOrganization(currInstance.OrganizationId);
                        }
                    }
                    DateTime dtNow = DateTime.Now;
                    if (this.ArticleGuid == Guid.Empty)
                    {
                        // create a reqest
                        Guid newId = Guid.NewGuid();
                        // create a request
                        if (this.ArticleTableAdapter.Insert(newId,
                            this.InstanceGuid,
                            new Guid?(),
                            ArticleType.Request.ToString(),
                            (this.AlternateId != string.Empty) ? this.AlternateId : HttpUtility.HtmlEncode(Utils.ShortCommentText(TextBoxComment.Text, 50)),
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
                            MainDataSet.CommentDataTable commentDataTable = taComment.InsertComment(newId,
                                TextBoxName.Text,
                                TextBoxEmail.Text,
                                string.Format("{0} ({1})", Request.UserHostName, Request.UserHostAddress),
                                HttpUtility.HtmlEncode(Utils.ShortCommentText(TextBoxComment.Text, 50)),
                                HttpUtility.HtmlEncode(TextBoxComment.Text),
                                false,
                                true,
                                DateTime.Now, true);
                            if (commentDataTable != null && commentDataTable.Rows.Count > 0)
                            {
                                if (CommentPosted != null) CommentPosted(this, new ArticleEventArgs(newId, true));
                                if (currOrganization != null && currInstance != null)
                                {
                                    string subj;
                                    Micajah.Common.Dal.OrganizationDataSet.UserDataTable users = Micajah.Common.Bll.Providers.UserProvider.GetUsers(
                                        currOrganization.OrganizationId,
                                        this.InstanceGuid,
                                        new string[] { "InstAdmin" });

                                    ArrayList admins = new ArrayList();
                                    ArrayList SendTo = new ArrayList();

                                    admins.AddRange(this.InputListAdmin);

                                    foreach (Micajah.Common.Dal.OrganizationDataSet.UserRow row in users)
                                        admins.Add(row.Email);

                                    SendTo.AddRange(admins);

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
                                        if (!separated && !admins.Contains(SendTo[i].ToString()))
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
                                            subj = (this.AlternateId != string.Empty) ? this.AlternateId : HttpUtility.HtmlEncode(Utils.ShortCommentText(TextBoxComment.Text, 50));
                                            StringBuilder body = new StringBuilder((string)this.GetLocalResourceObject("EmailBody_Request"));
                                            body.Replace("{OrgName}", currOrganization.Name);
                                            body.Replace("{InstName}", currInstance.Name);
                                            body.Replace("{ArticleName}", subj);
                                            body.Replace("{ArticleUrl}", siteUrl + string.Format(CultureInfo.CurrentCulture, "?i={0}&t={1}", this.InstanceGuid.ToString("N"), newId.ToString("N")));
                                            body.Replace("{ArticleText}", HttpUtility.HtmlEncode(TextBoxComment.Text));
                                            body.Replace("{AuthorName}", TextBoxName.Text);
                                            body.Replace("{AuthorEmail}", string.IsNullOrEmpty(TextBoxEmail.Text) ? string.Empty : string.Format(CultureInfo.CurrentCulture, "<a href=\"mailto:{0}\" target=\"_blank\">{0}</a>", TextBoxEmail.Text));
                                            body.Replace("{ImageUrl}", siteUrl + Micajah.Common.Configuration.FrameworkConfiguration.Current.WebApplication.Copyright.CompanyLogoImageUrl);
                                            if (body.Length > 0)
                                            {
                                                string encrypted = Utils.Encrypt(String.Format("{0}&{1}&{2}&{3}&{4}", SendTo[i].ToString(), 1, commentDataTable[0].CommentId.ToString(CultureInfo.InvariantCulture), InstanceGuid.ToString(), UserContext.SelectedOrganizationId.ToString()), "Dshd*&^*@dsdss", "237w&@2d", "SHA1", 2, "&s2hfyDjuf372*73", 256);
                                                string url = siteUrl + String.Format("Unsubscribe.aspx?token={0}", encrypted);
                                                body.Replace("{UnsubscribeFromArticleUrl}", url);
                                                encrypted = Utils.Encrypt(String.Format("{0}&{1}&{2}&{3}&{4}", SendTo[i].ToString(), 2, commentDataTable[0].CommentId.ToString(CultureInfo.InvariantCulture), InstanceGuid.ToString(), UserContext.SelectedOrganizationId.ToString()), "Dshd*&^*@dsdss", "237w&@2d", "SHA1", 2, "&s2hfyDjuf372*73", 256);
                                                url = siteUrl + String.Format("Unsubscribe.aspx?token={0}", encrypted);
                                                body.Replace("{UnsubscribeFromAllUrl}", url);
                                                if (admins.Contains(SendTo[i]))
                                                    body.Replace("{SendToList}", "This message was also sent to:<br>" + SendToList);
                                                else
                                                    body.Replace("{SendToList}", string.Empty);
                                                Utils.SendEmail("noreply@litekb.com", SendTo[i].ToString(), string.Format((string)this.GetLocalResourceObject("EmailSubjectRequest"), subj), body.ToString(), true, Micajah.Common.Configuration.FrameworkConfiguration.Current.WebApplication.Email.SmtpServer, true);
                                            }
                                        }
                                    }
                                }
                                ResetData();
                            }
                        }
                    }
                    else
                    {
                        // post to article
                        MainDataSet.CommentDataTable commentDataTable = taComment.InsertComment(this.ArticleGuid,
                            TextBoxName.Text,
                            TextBoxEmail.Text,
                            string.Format("{0} ({1})", Request.UserHostName, Request.UserHostAddress),
                            HttpUtility.HtmlEncode(Utils.ShortCommentText(TextBoxComment.Text, 50)),
                            HttpUtility.HtmlEncode(TextBoxComment.Text),
                            false,
                            true,
                            dtNow, true);
                        if (commentDataTable != null && commentDataTable.Rows.Count > 0)
                        {
                            if (CommentPosted != null) CommentPosted(this, new ArticleEventArgs(this.ArticleGuid, false));
                            MainDataSet.ArticleRow articleRow = this.GetCurrentArticle();
                            if (currOrganization != null && articleRow != null && !articleRow.IsUpdatedByNull() && currInstance != null)
                            {
                                string author = string.Empty, subj;
                                subj = string.Format((string)this.GetLocalResourceObject("EmailSubjectComment"), articleRow.Subject);

                                System.Data.DataRow mcuser = Micajah.Common.Bll.Providers.UserProvider.GetUserRow(articleRow.UpdatedBy, currOrganization.OrganizationId);
                                if (mcuser != null)
                                    author = (string)mcuser["Email"];

                                Micajah.Common.Dal.OrganizationDataSet.UserDataTable users = Micajah.Common.Bll.Providers.UserProvider.GetUsers(
                                        currOrganization.OrganizationId,
                                        this.InstanceGuid,
                                        new string[] { "InstAdmin" });

                                ArrayList admins = new ArrayList();
                                ArrayList SendTo = new ArrayList();

                                admins.AddRange(this.InputListAdmin);
                                foreach (Micajah.Common.Dal.OrganizationDataSet.UserRow row in users)
                                    admins.Add(row.Email);
                                SendTo.AddRange(admins);

                                if (!string.IsNullOrEmpty(author) && !SendTo.Contains(author))
                                    SendTo.Add(author);                                                              

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
                                    if (!separated && !admins.Contains(SendTo[i].ToString()))
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
                                        StringBuilder body = new StringBuilder((string)this.GetLocalResourceObject("EmailBody_PostToArticle"));
                                        body.Replace("{OrgName}", currOrganization.Name);
                                        body.Replace("{InstName}", currInstance.Name);
                                        body.Replace("{ArticleName}", articleRow.Subject);
                                        body.Replace("{ArticleUrl}", siteUrl + string.Format(CultureInfo.CurrentCulture, "?i={0}&t={1}", this.InstanceGuid.ToString("N"), this.ArticleGuid.ToString("N")));
                                        body.Replace("{ArticleText}", HttpUtility.HtmlEncode(TextBoxComment.Text));
                                        body.Replace("{AuthorName}", TextBoxName.Text);
                                        body.Replace("{AuthorEmail}", string.IsNullOrEmpty(TextBoxEmail.Text) ? string.Empty : string.Format(CultureInfo.CurrentCulture, "<a href=\"mailto:{0}\" target=\"_blank\">{0}</a>", TextBoxEmail.Text));
                                        body.Replace("{ImageUrl}", siteUrl + Micajah.Common.Configuration.FrameworkConfiguration.Current.WebApplication.Copyright.CompanyLogoImageUrl);
                                        if (admins.Contains(SendTo[i]))
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
                            ResetData();
                        }
                    }
                }
            }
        }
        #endregion
    }
}