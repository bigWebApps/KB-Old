using System;
using System.Web;
using System.Collections.Specialized;

namespace BWA.Knowledgebase.Modules
{
    public class RewriteContext
    {
        public static RewriteContext Current
        {
            get
            {
                if (HttpContext.Current.Items.Contains("RewriteContextInfo"))
                    return (RewriteContext)HttpContext.Current.Items["RewriteContextInfo"];

                return new RewriteContext();
            }
        }

        public RewriteContext()
        {
            Params = new NameValueCollection();
            InitialUrl = String.Empty;
        }

        public RewriteContext(NameValueCollection param, string url)
        {
            InitialUrl = url;
            Params = new NameValueCollection(param);

        }

        public NameValueCollection Params { get; set; }

        public string InitialUrl { get; set; }
    }
}