using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.Security;
using Micajah.Common.Bll;
using System.Text;
using System.Globalization;
using System.Data;

namespace BWA.Knowledgebase
{
    public partial class InstanceHome : System.Web.UI.Page
    {
        #region Members
        private MainDataSetTableAdapters.ArticleTableAdapter m_taArticle = null;
        private Instance m_CurrentInstance = null;
        #endregion

        #region Properties

        public Guid InstanceGuid
        {
            get
            {
                Guid g = Guid.Empty;
                if (!string.IsNullOrEmpty(Request.QueryString["i"]))
                {
                    try { g = new Guid(Request.QueryString["i"]); }
                    catch { }
                }
                return g;
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

        protected MainDataSetTableAdapters.ArticleTableAdapter ArticleTableAdapter
        {
            get
            {
                if (m_taArticle == null)
                    m_taArticle = new MainDataSetTableAdapters.ArticleTableAdapter();
                return m_taArticle;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (UserContext.Current != null && UserContext.Current.SelectedInstance != null)
                {
                    Response.Redirect("InstanceHomeAdmin.aspx", false);
                }
                else if (this.InstanceGuid != Guid.Empty)
                {
                    string loginUrl = Micajah.Common.Application.WebApplication.LoginProvider.GetLoginUrl();
                    string orgInstName = string.Format(CultureInfo.CurrentCulture, "{0} {1}", this.CurrentInstance.Organization.Name, this.CurrentInstance.Name);
                    #region Generate the Page
                    StringBuilder textHeader = new StringBuilder((string)this.GetLocalResourceObject("Header_Inst"));
                    StringBuilder textFooter = new StringBuilder((string)this.GetLocalResourceObject("Footer_Inst"));
                    textHeader = textHeader.Replace("{title}", orgInstName);
                    textHeader = textHeader.Replace("{metadescription}", orgInstName);
                    textHeader = textHeader.Replace("{searchaction}", "SearchResult.aspx?i=" + this.InstanceGuid.ToString("N"));
                    textHeader = textHeader.Replace("{orgInstName}", orgInstName);
                    textHeader = textHeader.Replace("{orgUrl}", this.CurrentInstance.Organization.WebsiteUrl);
                    Response.Write(textHeader.ToString());
                    Response.Write("<br>");
                    //Response.Write("<ul>");
                    string listTreeView = string.Empty;
                    MainDataSet.ArticleDataTable articleTable = this.ArticleTableAdapter.GetRecursiveByDepartmentGuid(this.InstanceGuid);
                    this.SortRecursiveTable(ref listTreeView, ref articleTable, null);
                    Response.Write(listTreeView);
                    //Response.Write("</ul>");
                    textFooter = textFooter.Replace("{micajahurl}", loginUrl);
                    textFooter = textFooter.Replace("{currentyear}", DateTime.Now.Year.ToString(CultureInfo.InvariantCulture));
                    textFooter = textFooter.Replace("{trackingcode}", string.Empty);
                    //Setting trackCode = this.CurrentInstance.Settings.FindByShortName("TrackingCode");
                    //if (trackCode != null && !string.IsNullOrEmpty(trackCode.Value))
                    //    textFooter = textFooter.Replace("{trackingcode}", trackCode.Value);
                    //else
                    //    textFooter = textFooter.Replace("{trackingcode}", (string)this.GetLocalResourceObject("GlobalTrackingCode"));

                    Response.Write(textFooter);

                    #endregion
                }
            }
        }

        private string LICnt(int cnt)
        {
            string result = "<li>";
            return result;
        }

        private void SortRecursiveTable(ref string output, ref MainDataSet.ArticleDataTable input, Guid? parentArticleId)
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
                output += string.Format(CultureInfo.CurrentCulture, "<li><a style=\"vertical-align:top\" href=\"Default.aspx?i={0}&t={1}\">{2}</a>", this.InstanceGuid.ToString("N"), row.ArticleGuid.ToString("N"), row.Subject);
                SortRecursiveTable(ref output, ref input, row.ArticleGuid);
            }
            if (cnt > 0)
                output += "</ul>";
        }

        #region old code
        /*
            Master.AutoGenerateBreadcrumbs = false;
            Master.VisibleBreadcrumbs = false;
            //Master.VisiblePageTitle = false;
            if (!IsPostBack)
            {
                if (UserContext.Current != null && UserContext.Current.SelectedInstance != null)
                {
                    Response.Redirect("InstanceHomeAdmin.aspx", false);
                }
                else if (this.Master.InstanceGuid != Guid.Empty)
                {
                    //SearchArticleCtrl.InstanceGuid = this.Master.InstanceGuid;
                    //InstanseName.Text = Utils.GetInstanceName(this.Master.InstanceGuid);
                    ArticlesTreeCtrl.InstanceGuid = this.Master.InstanceGuid;
                    //PostCommentCtrl.InstanceGuid = this.Master.InstanceGuid;
                    //Master.Title = "Instance Home";
                }
            }
             * */
        #endregion
    }
}
