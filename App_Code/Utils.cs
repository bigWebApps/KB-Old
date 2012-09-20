using System;
using Micajah.Common.Bll;
using System.Net.Mail;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Web;

namespace BWA.Knowledgebase
{
    [FlagsAttribute]
    public enum ArticleType
    {
        Request,
        Article,
        FAQ
    }

    [Serializable]
    
    public class ArticleEventArgs : EventArgs
    {
        public Guid ArticleGuid { get; set; }
        public bool IsNew { get; set; }
        public ArticleEventArgs(Guid articleGuid, bool isNew) { this.ArticleGuid = articleGuid; this.IsNew = isNew; }
    }

    public static class Utils
    {
        public static BreadcrumbCollection GenerateBreadCrumbs(Guid articleGuid, Guid instanceGuid, bool isAdmin)
        {
            BreadcrumbCollection breadCrumbs = new BreadcrumbCollection();
            using (MainDataSetTableAdapters.ArticleTableAdapter m_taArticle = new MainDataSetTableAdapters.ArticleTableAdapter())
            {
                if (articleGuid != Guid.Empty && instanceGuid != Guid.Empty)
                {
                    MainDataSet.ArticleDataTable dtArticle = m_taArticle.GetRecursiveAllByArticleGuid(articleGuid);
                    if (dtArticle.Count > 0)
                    {
                        for (int i = dtArticle.Count - 1; i >= 0; i--)
                        {
                            MainDataSet.ArticleRow row = dtArticle[i];
                            breadCrumbs.Add(
                                row.Subject,
                                isAdmin ? string.Format("~/ArticleViewAdmin.aspx?id={0}", row.ArticleGuid.ToString("N")) : string.Format("~/?i={0}&t={1}", instanceGuid.ToString("N"), row.ArticleGuid.ToString("N")), 
                                row.Subject, false);
                        }
                    }
                }
            }
            return breadCrumbs;
        }

        public static string GetInstanceUserName(Guid instanceGuid)
        {
            string userName = string.Empty;
            MainDataSetTableAdapters.Mc_InstanceTableAdapter taDepartment = new MainDataSetTableAdapters.Mc_InstanceTableAdapter();
            object obj = taDepartment.GetDepartmentAdministrator(instanceGuid);
            if (obj != null)
                userName = (string)obj;
            else
            {
                obj = taDepartment.GetOrganizationAdministrator(instanceGuid);
                if (obj != null)
                    userName = (string)obj;
            }
            return userName;
        }

        public static string GetInstanceName(Guid instanceGuid)
        {
            MainDataSetTableAdapters.Mc_InstanceTableAdapter taDepartment = new MainDataSetTableAdapters.Mc_InstanceTableAdapter();
            MainDataSet.Mc_InstanceDataTable departmentTable = taDepartment.GetDataByInstanceId(instanceGuid);
            if (departmentTable != null && departmentTable.Rows.Count > 0)
                return ((MainDataSet.Mc_InstanceRow)departmentTable.Rows[0]).Name;
            else
                return string.Empty;
        }

        public static string ShortCommentText(string input, int count)
        {
            if (input.Length <= count) return input;
            int index = input.IndexOf(' ', count);
            if (index < 0) return input;
            return input.Substring(0, index);;
        }

        public static bool SendEmail(string from, string to, string subject, string body, bool isBodyHtml, string smtpServer, bool async)
        {
            bool sent = false;
            MailMessage msg = null;
            try
            {
                msg = new MailMessage(from, to);
                msg.Subject = subject;
                msg.Body = body;
                msg.IsBodyHtml = isBodyHtml;

                SmtpClient client = new SmtpClient(smtpServer);
                if (async)
                    client.SendAsync(msg, to);
                else
                    client.Send(msg);
                sent = true;
            }
            catch (ArgumentNullException) { }
            catch (ArgumentException) { }
            catch (FormatException) { }
            catch (SmtpException) { }
            finally
            {
                if ((msg != null) && (!async)) msg.Dispose();
            }

            return sent;
        }

        public static bool SendEmail(string from, string to, string[] cc, string subject, string body, bool isBodyHtml, string smtpServer, bool async)
        {
            bool sent = false;
            MailMessage msg = null;
            try
            {
                msg = new MailMessage(from, to);
                msg.Subject = subject;
                msg.Body = body;
                msg.IsBodyHtml = isBodyHtml;
                if (cc.Length > 0)
                {
                    foreach (string email in cc)
                        msg.CC.Add(email);
                }

                SmtpClient client = new SmtpClient(smtpServer);
                if (async)
                    client.SendAsync(msg, to);
                else
                    client.Send(msg);
                sent = true;
            }
            catch (ArgumentNullException) { }
            catch (ArgumentException) { }
            catch (FormatException) { }
            catch (SmtpException) { }
            finally
            {
                if ((msg != null) && (!async)) msg.Dispose();
            }

            return sent;
        }

        public static string Encrypt(string plainText, string passPhrase, string saltValue, string hashAlgorithm, int passwordIterations, string initVector, int keySize)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes = password.GetBytes(keySize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            string cipherText = Convert.ToBase64String(cipherTextBytes);

            StringWriter writer = new StringWriter();
            HttpContext.Current.Server.UrlEncode(cipherText, writer);

            return writer.ToString();
        }

        public static string Decrypt(string cipherText, string passPhrase, string saltValue, string hashAlgorithm, int passwordIterations, string initVector, int keySize)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes = password.GetBytes(keySize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

            StringWriter writer = new StringWriter();
            HttpContext.Current.Server.UrlDecode(plainText, writer);

            return writer.ToString();
        }
    }
}