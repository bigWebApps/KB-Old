using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BWA.Knowledgebase
{
    public partial class ArticleListControl : System.Web.UI.UserControl
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

        public int BodyLength
        {
            get
            {
                if (ViewState["BodyLength"] != null)
                    return (int)ViewState["BodyLength"];
                else return 100;
            }
            set
            {
                ViewState["BodyLength"] = value;
            }
        }
        
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.DataBind();
            }
        }

        public override void DataBind()
        {
            if (this.InstanceGuid != Guid.Empty || ArticleGuid != Guid.Empty)
            {
                DataListArticles.DataSource = this.ArticleAdapter.GetChildArticles(this.ArticleGuid == Guid.Empty ? new Guid?() : new Guid?(this.ArticleGuid), this.BodyLength).OrderBy(x => x.Subject);
                DataListArticles.DataBind();
            }
        }
    }
}