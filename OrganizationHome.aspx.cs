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
using Micajah.Common.Dal;

namespace BWA.Knowledgebase
{
    public partial class OrganizationHome : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (UserContext.SelectedOrganizationId != Guid.Empty)
            {
                OrganizationDataSet.InstanceDataTable instanceDataTable = (OrganizationDataSet.InstanceDataTable)Micajah.Common.Bll.Providers.InstanceProvider.GetInstances(UserContext.SelectedOrganizationId);
                if (instanceDataTable != null && instanceDataTable.Rows.Count > 0)
                {
                    DataView customUrls = Micajah.Common.Bll.Providers.CustomUrlProvider.GetCustomUrls(UserContext.SelectedOrganizationId);
                    foreach (OrganizationDataSet.InstanceRow instanceRow in instanceDataTable.Rows)
                    {
                        DataRow[] datarows = ((CommonDataSet.CustomUrlDataTable)customUrls.Table).Select(string.Format("InstanceId = '{0}'", instanceRow.InstanceId.ToString()));
                        if (datarows.Length > 0)
                            instanceRow.Description = ((CommonDataSet.CustomUrlRow)datarows[0]).FullCustomUrl.Length > 0 ? ((CommonDataSet.CustomUrlRow)datarows[0]).FullCustomUrl : ((CommonDataSet.CustomUrlRow)datarows[0]).PartialCustomUrl.Length > 0 ? ((CommonDataSet.CustomUrlRow)datarows[0]).PartialCustomUrl : "litekb.com/?i=" + instanceRow.InstanceId.ToString();
                        else
                            instanceRow.Description = "litekb.com/?i=" + instanceRow.InstanceId.ToString();
                    }
                }
                InstanceRepeater.DataSource = instanceDataTable.DefaultView;
                InstanceRepeater.DataBind();
            }
        }
    }
}
