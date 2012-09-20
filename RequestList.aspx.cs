using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.Security;

namespace BWA.Knowledgebase
{
    public partial class RequestList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GridRequestList.ColorScheme = Micajah.Common.Configuration.FrameworkConfiguration.Current.WebApplication.DefaultColorScheme;
        }
        protected void ObjectDataSourceRequest_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (UserContext.Current != null)
            {
                e.InputParameters["DepartmentGuid"] = UserContext.Current.SelectedInstance.InstanceId;
                e.InputParameters["Type"] = ArticleType.Request.ToString();
            }
        }
    }
}