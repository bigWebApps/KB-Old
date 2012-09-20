using System;
using System.Web.UI.WebControls;
using Micajah.Common.Security;

namespace BWA.Knowledgebase
{
    public partial class SearchResultControl : System.Web.UI.UserControl
    {
        public string SearchText
        {
            get
            {
                string res = string.Empty;
                if (!string.IsNullOrEmpty(Request.QueryString["search"]) && !IsPostBack)
                    res = Request.QueryString["search"];
                else if (!string.IsNullOrEmpty(Request.Form["search"]))
                    res = Request.Form["search"];
                else
                    res = SearchArticleCtrl.SearchText;
                return res;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (UserContext.Current != null)
            GridSearchResult.ColorScheme = Micajah.Common.Configuration.FrameworkConfiguration.Current.WebApplication.DefaultColorScheme;
            //else
            //    Micajah.Common.Pages.MasterPage.AddGlobalStyleSheet(this.Page);
            if (!IsPostBack)
            {
                SearchArticleCtrl.InstanceGuid = this.InstanceGuid;                
                ((HyperLinkField)GridSearchResult.Columns[0]).DataNavigateUrlFormatString = (UserContext.Current == null) ? "~/" + (UserContext.SelectedInstanceId != Guid.Empty ? "?" : "?i=" + this.InstanceGuid.ToString("N") + "&") + "t={0:N}" : "~/ArticleViewAdmin.aspx?id={0:N}";
            }
            SearchArticleCtrl.SearchText = this.SearchText;
        }

        protected void ObjectDataSourceSearch_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["DepartmentGuid"] = this.InstanceGuid;
            string searchText = string.Empty;
            if (this.SearchText.Length > 0)
            {
                foreach (string str in this.SearchText.Split(new char[] { ' ' }))
                    searchText += string.Format("FORMSOF(INFLECTIONAL,\"{0}\") AND ", str);
                if (searchText.Length > 5) searchText = searchText.Substring(0, searchText.Length - 5);
            }
            else searchText = "\"\"";
            e.InputParameters["SearchingText"] = searchText;
        }
    }
}