using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections;

namespace BWA.Knowledgebase
{
    public partial class Unsubscribe : System.Web.UI.Page
    {
        protected ArrayList InstanceAdminEmails(Guid organizationId, Guid instanceId)
        {

            Micajah.Common.Dal.OrganizationDataSet.UserDataTable users = Micajah.Common.Bll.Providers.UserProvider.GetUsers(
                                        organizationId,
                                        instanceId,
                                        new string[] { "InstAdmin" });
            ArrayList recipientemails = new ArrayList();
            foreach (Micajah.Common.Dal.OrganizationDataSet.UserRow row in users)
                recipientemails.Add(row.Email);
            return recipientemails;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["token"]))
            {
                string decrypted = Utils.Decrypt(Request.QueryString["token"], "Dshd*&^*@dsdss", "237w&@2d", "SHA1", 2, "&s2hfyDjuf372*73", 256);
                string[] splitted = decrypted.Split('&');
                if (splitted.Length > 3)
                {
                    string email = splitted[0];
                    int type; int.TryParse(splitted[1], out type);
                    int commentId; int.TryParse(splitted[2], out commentId);
                    Guid instanceId = new Guid(splitted[3]);
                    Guid organizationId = new Guid(splitted[4]);

                    if (type == 1) // Unsubscribe from article
                    {
                        if (InstanceAdminEmails(organizationId, instanceId).Contains(email))
                        {
                            string encrypted = Utils.Encrypt(String.Format("{0}&{1}&{2}&{3}&{4}", email, 2, commentId.ToString(CultureInfo.InvariantCulture), instanceId.ToString(), organizationId.ToString()), "Dshd*&^*@dsdss", "237w&@2d", "SHA1", 2, "&s2hfyDjuf372*73", 256);

                            int indexUrl = Request.Url.AbsoluteUri.IndexOf(":" + Request.Url.Port.ToString());
                            if (indexUrl <= 0) indexUrl = Request.Url.AbsoluteUri.IndexOf(Request.Url.AbsolutePath);
                            string siteUrl = Request.Url.AbsoluteUri.Substring(0, indexUrl) + Request.ApplicationPath;
                            string url = siteUrl + String.Format("Unsubscribe.aspx?token={0}", encrypted);
                            Message.Text = String.Format("You cannot unsubscribe from this article, you are receiving emails because you are administrator of the KB system.<br><br><a href='{0}'>Click here to stop receiving all emails</a>", url);
                            CanCloseLiteral.Visible = false;
                        }
                        else
                        {
                            MainDataSetTableAdapters.CommentTableAdapter comments = new MainDataSetTableAdapters.CommentTableAdapter();
                            MainDataSet.CommentDataTable commentDataTable = comments.GetDataByCommentId(commentId);
                            if (commentDataTable != null && commentDataTable.Rows.Count > 0)
                            {
                                if (commentDataTable[0].ReceiveEmailUpdate)
                                {
                                    comments.Unsubscribe(commentId);
                                    Message.Text = "You have succussfuly unsubscribe from this article.";
                                }
                                else
                                    Message.Text = "You have been already unsubscribed from this article.";
                            }
                        }
                    }
                    else  // Unsubscribe from all emails
                    {
                        MainDataSetTableAdapters.DoNotEmailTableAdapter doNotEmail = new MainDataSetTableAdapters.DoNotEmailTableAdapter();
                        MainDataSet.DoNotEmailDataTable doNotEmailDataTable = doNotEmail.GetDataByEmail(instanceId, email);
                        if (doNotEmailDataTable == null || doNotEmailDataTable.Rows.Count == 0)
                        {
                            doNotEmail.Insert(Guid.NewGuid(), instanceId, email);
                            Message.Text = "You have succussfuly unsubscribe from all emails.";
                        }
                        else
                            Message.Text = "You have been already unsubscribed from all emails.";
                    }
                }
            }
        }
    }
}
