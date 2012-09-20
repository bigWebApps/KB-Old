using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.Security;

namespace BWA.Knowledgebase
{
    public partial class SearchResultAdmin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (UserContext.Current != null)
                    SearchResultControl1.InstanceGuid = UserContext.Current.SelectedInstance.InstanceId;
            }
        }
    }
}