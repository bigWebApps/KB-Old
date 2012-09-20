using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.Security;

namespace BWA.Knowledgebase
{
    public partial class CommentsListControl : System.Web.UI.UserControl
    {
        public Unit Width
        {
            get { return DataListComments.Width; }
            set { DataListComments.Width = value; }
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

        public bool IsPublic
        {
            get
            {
                return (UserContext.Current != null);
            }
        }

        //public override bool Visible
        //{
        //    get
        //    {
        //        return base.Visible;
        //    }
        //    set
        //    {
        //        base.Visible = value;
        //    }
        //}

        public override void DataBind()
        {
            DataListComments.DataBind();
        }

        public void RegisterGoAnchor()
        {
            this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "CommentLoad", "window.location.href='#comments';", true);
        }

        protected void ObjectDataSourceComments_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["ArticleGuid"] = this.ArticleGuid;
        }

        protected void ObjectDataSourceComments_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            int count = ((MainDataSet.CommentDataTable)e.ReturnValue).Count;
            if (count > 0)
            {
                this.Visible = true;
                LabelCommentCount.Text = string.Format((string)this.GetLocalResourceObject("LabelCommentCountResource1.Text"), count);
            }
            else this.Visible = false;
        }

        protected void DataListComments_DeleteCommand(object source, DataListCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                int commentId = 0;
                int.TryParse(e.CommandArgument.ToString(), out commentId);
                if (commentId > 0)
                {
                    using (MainDataSetTableAdapters.CommentTableAdapter taComment = new MainDataSetTableAdapters.CommentTableAdapter())
                    {
                        taComment.Inactive(commentId);
                    }
                    this.DataBind();
                    RegisterGoAnchor();
                }
            }
        }
    }
}