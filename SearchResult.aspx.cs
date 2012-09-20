using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.Security;
using Micajah.Common.Bll;
using System.Globalization;

namespace BWA.Knowledgebase
{
    public partial class SearchResult : System.Web.UI.Page
    {
        #region Members

        private Instance m_CurrentInstance = null;

        #endregion

        public Instance CurrentInstance
        {
            get
            {
                if (m_CurrentInstance == null && this.InstanceGuid != Guid.Empty)
                    m_CurrentInstance = Micajah.Common.Bll.Providers.InstanceProvider.GetInstance(this.InstanceGuid, Guid.Empty);
                return m_CurrentInstance;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            string pageUrl = "./";

            string orgInstName = string.Format(CultureInfo.CurrentCulture, "{0} {1}", this.CurrentInstance.Organization.Name, this.CurrentInstance.Name);
            string instanceUrl = pageUrl + (UserContext.SelectedInstanceId == Guid.Empty ? "?i=" + this.InstanceGuid.ToString("N") : string.Empty);
            string instUrl = (UserContext.SelectedInstanceId == Guid.Empty ? "?i=" + this.Master.InstanceGuid.ToString("N") : string.Empty);
            string breadcrumbs = string.Format("<a href=\"{1}\">{0}</a> > {2}", (string)this.GetLocalResourceObject("Home"), instanceUrl, (string)this.GetLocalResourceObject("PageResource1.Title"), "~/SearchResult.aspx" + instUrl);

            SearchResultControl1.InstanceGuid = this.Master.InstanceGuid;
            SearchResultControl1.DataBind();
            Master.AutoGenerateBreadcrumbs = false;
            Master.CustomName = (string)this.GetLocalResourceObject("PageResource1.Title");
            Master.CustomNavigateUrl = "~/SearchResult.aspx" + instUrl;
            Master.VisibleHeader = false;
            Master.VisibleBreadcrumbs = false;
            TitleLink.NavigateUrl = instanceUrl;
            TitleLink.Text = orgInstName;
            this.Form.Action = "./SearchResult.aspx" + instUrl;
            breadcrumbsLiteral.Text = breadcrumbs;
        }
    }
}