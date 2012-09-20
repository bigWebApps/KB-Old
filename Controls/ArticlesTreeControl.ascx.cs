using System;
using System.Data;
using Micajah.Common.Security;

namespace BWA.Knowledgebase
{
    public partial class ArticlesTreeControl : System.Web.UI.UserControl
    {
        #region Members
        private MainDataSetTableAdapters.ArticleTableAdapter m_ArticleAdapter = null;
        #endregion

        #region Public Properties
        public MainDataSetTableAdapters.ArticleTableAdapter ArticleAdapter
        {
            get
            {
                if (m_ArticleAdapter == null)
                    m_ArticleAdapter = new MainDataSetTableAdapters.ArticleTableAdapter();
                return m_ArticleAdapter;
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

        public bool IsAdmin
        {
            get
            {
                if (ViewState["IsAdmin"] != null)
                    return (bool)ViewState["IsAdmin"];
                else return false;
            }
            set
            {
                ViewState["IsAdmin"] = value;
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

        public int ArticleCount
        {
            get
            {
                return DataListArticles.Items.Count;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LinkButtonShowInactive.Visible = this.IsAdmin && (this.InstanceGuid != Guid.Empty);
                if (this.InstanceGuid != Guid.Empty || ArticleGuid != Guid.Empty)
                {
                    MainDataSet.ArticleDataTable articleTable;
                    MainDataSet.ArticleDataTable sortedTable = new MainDataSet.ArticleDataTable();
                    if (ArticleGuid != Guid.Empty)
                    {
                        articleTable = this.ArticleAdapter.GetRecursiveByArticleGuid(ArticleGuid);
                        this.SortRecursiveTable(ref sortedTable, ref articleTable, ArticleGuid);
                    }
                    else
                    {
                        articleTable = this.ArticleAdapter.GetRecursiveByDepartmentGuid(InstanceGuid);
                        this.SortRecursiveTable(ref sortedTable, ref articleTable, null);
                    }
                    if (sortedTable != null && sortedTable.Rows.Count > 0)
                    {
                        this.Visible = true;

                        DataListArticles.DataSource = sortedTable;
                        DataListArticles.DataBind();
                    }
                    else
                        this.Visible = false;
                }
            }
        }
        protected void LinkButtonShowInactive_Click(object sender, EventArgs e)
        {
            if (this.InstanceGuid != Guid.Empty)
            {
                MainDataSet.ArticleDataTable articleTable;
                MainDataSet.ArticleDataTable sortedTable = new MainDataSet.ArticleDataTable();
                if (LinkButtonShowInactive.Text.Equals((string)this.GetLocalResourceObject("ShowInactive")))
                {
                    LinkButtonShowInactive.Text = (string)this.GetLocalResourceObject("ShowActive");
                    articleTable = this.ArticleAdapter.GetRecursiveAllByDepartmentGuid(InstanceGuid);
                    this.SortRecursiveTable(ref sortedTable, ref articleTable, null);
                }
                else
                {
                    LinkButtonShowInactive.Text = (string)this.GetLocalResourceObject("ShowInactive");
                    articleTable = this.ArticleAdapter.GetRecursiveByDepartmentGuid(InstanceGuid);
                    this.SortRecursiveTable(ref sortedTable, ref articleTable, null);
                }
                if (sortedTable != null && sortedTable.Rows.Count > 0)
                {
                    this.Visible = true;
                    DataListArticles.DataSource = sortedTable;
                    DataListArticles.DataBind();
                }
                else
                    this.Visible = false;
            }
        }
        private void SortRecursiveTable(ref MainDataSet.ArticleDataTable output, ref MainDataSet.ArticleDataTable input, Guid? parentArticleId)
        {
            OrderedEnumerableRowCollection<MainDataSet.ArticleRow> query = null;
            if (parentArticleId.HasValue)
                query = input.Where(w => w.IsParentArticleGuidNull() ? false : w.ParentArticleGuid.Equals(parentArticleId.Value)).OrderBy(o => o.Subject);
            else
                query = input.Where(w => w.IsParentArticleGuidNull()).OrderBy(o => o.Subject);
            foreach (MainDataSet.ArticleRow row in query)
            {
                output.ImportRow(row);
                SortRecursiveTable(ref output, ref input, row.ArticleGuid);
            }
        }
        //private void CopyRow(ref MainDataSet.ArticleRow output, MainDataSet.ArticleRow copy)
        //{
        //    output.ArticleGuid = copy.ArticleGuid;
        //    output.ArticleID = copy.ArticleID;
        //    output.Body = copy.Body;
        //    if (copy.IsCreatedByNull()) output.SetCreatedByNull(); else output.CreatedBy = copy.CreatedBy;
        //    if (copy.IsCreatedTimeNull()) output.SetCreatedTimeNull(); else output.CreatedTime = copy.CreatedTime;
        //    output.Deleted = copy.Deleted;
        //    if (copy.IsDeletedByNull()) output.SetDeletedByNull(); else output.DeletedBy = copy.DeletedBy;
        //    if (copy.IsDeletedTimeNull()) output.SetDeletedTimeNull(); else output.DeletedTime = copy.DeletedTime;
        //    output.DepartmentGuid = copy.DepartmentGuid;
        //    if (copy.IsLevelNull()) output.SetLevelNull(); else output.Level = copy.Level;
        //    if (copy.IsParentArticleGuidNull()) output.SetParentArticleGuidNull(); else output.ParentArticleGuid = copy.ParentArticleGuid;
        //    if (copy.IsPathNull()) output.SetPathNull(); else output.Path = copy.Path;
        //    output.RateCount = copy.RateCount;
        //    output.RateSum = copy.RateSum;
        //    output.ReviewCount = copy.ReviewCount;
        //    output.SearchDesc = copy.SearchDesc;
        //    output.Subject = copy.Subject;
        //    output.Type = copy.Type;
        //    if (copy.IsUpdatedByNull()) output.SetUpdatedByNull(); else output.UpdatedBy = copy.UpdatedBy;
        //    if (copy.IsUpdatedTimeNull()) output.SetUpdatedTimeNull(); else output.UpdatedTime = copy.UpdatedTime;
        //}
    }
}