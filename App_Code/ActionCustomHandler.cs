using System;
using System.Globalization;
using Micajah.Common.Bll;
using Micajah.Common.Security;

namespace BWA.Knowledgebase
{
    public class ActionCustomHandler : INameProvider<Micajah.Common.Bll.Action>
    {
        private MainDataSetTableAdapters.ArticleTableAdapter taArticle = null;
        private MainDataSetTableAdapters.CommentTableAdapter taComment = null;

        public ActionCustomHandler()
        {
            taArticle = new MainDataSetTableAdapters.ArticleTableAdapter();
            taComment = new MainDataSetTableAdapters.CommentTableAdapter();
        }

        public string GetName(Micajah.Common.Bll.Action obj)
        {
            if (obj != null && UserContext.Current.SelectedInstance != null)
            {
                int? count = new int?();
                switch (obj.NavigateUrl)
                {
                    case "/RequestList.aspx":
                        count = taArticle.GetTotalCountByType(UserContext.Current.SelectedInstance.InstanceId, ArticleType.Request.ToString());
                        break;
                    case "/UnreadComments.aspx":
                        count = (int)taComment.GetTotalUnreadCommentCount(UserContext.Current.SelectedInstance.InstanceId);
                        break;
                }
                if (count.HasValue)
                    if (count.Value > 0)
                        return string.Format("{0} ({1})", obj.Name, count.Value);
                return obj.Name;

            }
            return string.Empty;
        }
    }
}