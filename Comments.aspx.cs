using System;
using System.Web.UI.WebControls;
using Micajah.Common.Security;

namespace BWA.Knowledgebase
{
    public partial class Comments : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GridUnreadComments.ColorScheme = Micajah.Common.Configuration.FrameworkConfiguration.Current.WebApplication.DefaultColorScheme;
        }
        protected void ObjectDataSourceUnreadComments_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (UserContext.Current != null)
            {
                e.InputParameters["DepartmentGuid"] = UserContext.Current.SelectedInstance.InstanceId;
            }
        }
    }
}
