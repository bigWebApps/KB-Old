using System;
using System.Globalization;
using System.Web.UI.WebControls;
using Micajah.Common.Security;
using System.Text;
using System.Data;
using Micajah.Common.Bll;

namespace BWA.Knowledgebase
{
    public partial class SearchArticleControl : System.Web.UI.UserControl
    {
        #region Members

        private Instance m_CurrentInstance = null;

        #endregion

        #region Properties

        protected Instance CurrentInstance
        {
            get
            {
                if (m_CurrentInstance == null && this.InstanceGuid != Guid.Empty)
                    m_CurrentInstance = Micajah.Common.Bll.Providers.InstanceProvider.GetInstance(this.InstanceGuid, Guid.Empty);
                return m_CurrentInstance;
            }
        }

        protected string Host
        {
            get
            {
                DataView customUrls = Micajah.Common.Bll.Providers.CustomUrlProvider.GetCustomUrls(CurrentInstance.OrganizationId);
                string host = string.Empty;

                if (customUrls != null && customUrls.Count > 0)
                    foreach (DataRowView drv in customUrls)
                    {
                        if (drv["InstanceId"] != null && drv["InstanceId"].ToString().Length > 0 && new Guid(drv["InstanceId"].ToString()) == this.InstanceGuid)
                        {
                            if (drv["FullCustomUrl"].ToString().Length > 0)
                                host = drv["FullCustomUrl"].ToString();
                            else
                                host = drv["PartialCustomUrl"].ToString();
                        }
                    }
                return host;
            }
        }

        #endregion

        public Unit Width
        {
            get
            {
                Unit w = Unit.Empty;
                w = Unit.Parse(tableSearch.Width, CultureInfo.CurrentCulture);
                return w;
            }
            set
            {
                if (value.IsEmpty)
                    tableSearch.Width = "500px";
                else
                    tableSearch.Width = value.ToString(CultureInfo.CurrentCulture);
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

        public string SearchText
        {
            get { return TextBoxSearch.Text; }
            set { TextBoxSearch.Text = value; }
        }

        protected void ButtonSearch_Click(object sender, EventArgs e)
        {
            if (UserContext.Current != null)
                Response.Redirect(string.Format("~/SearchResultAdmin.aspx?search={0}", this.SearchText), false);
            else
                Response.Redirect(string.Format("~/SearchResult.aspx?i={0}&search={1}", this.InstanceGuid.ToString("N"), this.SearchText), false);
        }

        protected void GoogleSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("http://www.google.com/#q={0}&sitesearch={1}", SearchText, Host), true);
        }

        protected void BingSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("http://www.bing.com/search?cp=1251&FORM=FREESS&q={0}&q1=site%3A{1}", SearchText, Host), true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            TextBoxSearch.Attributes.Add("onkeydown", "javascript:preventEnter(event);");
            TextBoxSearch.Focus();
        }
    }
}