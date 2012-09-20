using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.Security;

namespace BWA.Knowledgebase
{
    public partial class SearchResult : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Master.VisibleHeader = false;
                this.Form.Action = "http://www.bing.com/search";
                this.Form.Method = "get";
                this.Form.Target = "_blank";
                siteDomain.Text = "<input type='hidden'name='q1' value='site:"+ Page.Request.Url.Host + "' />";
            }
        }
    }
}