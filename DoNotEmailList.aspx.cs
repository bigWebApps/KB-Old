using System;
using System.Web.UI.WebControls;
using Micajah.Common.Security;

namespace BWA.Knowledgebase
{
    public partial class DoNotEmailList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GridDoNotEmailList.ColorScheme = Micajah.Common.Configuration.FrameworkConfiguration.Current.WebApplication.DefaultColorScheme;
        }
        protected void ObjectDataSourceDoNotEmailList_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (UserContext.Current != null)
            {
                e.InputParameters["InstanceId"] = UserContext.Current.SelectedInstance.InstanceId;
            }
        }
    }
}
