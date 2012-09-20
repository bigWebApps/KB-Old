using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.Security;

namespace BWA.Knowledgebase
{
    public partial class InstanceHomeAdmin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (UserContext.Current != null)
                {
                    //InstanseName.Text = Utils.GetInstanceName(UserContext.Current.SelectedInstance.InstanceId);
                    ArticlesTreeCtrl.InstanceGuid = UserContext.Current.SelectedInstance.InstanceId;
                    //PostCommentCtrl.InstanceGuid = UserContext.Current.SelectedInstance.InstanceId;
                    //SearchArticleCtrl.InstanceGuid = UserContext.Current.SelectedInstance.InstanceId;
                    //Master.Title = "Instance Home";
                }
            }
        }

        protected void PostCommentCtrl_CommentPosted(object sender, ArticleEventArgs e)
        {
            Response.Redirect("~/RequestList.aspx", false);
        }

    }
}