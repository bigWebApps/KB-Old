using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Configuration;

namespace BWA.Knowledgebase.Modules
{
    public class RewriteModule : IHttpModule
    {
        public RewriteModule()
        {

        }

        protected void RewriteModule_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            if ((app.Context.CurrentHandler is Page) && app.Context.CurrentHandler != null)
            {
                Page pg = (Page)app.Context.CurrentHandler;
                pg.PreInit += Page_PreInit;
            }
        }
        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (HttpContext.Current.Items.Contains("OriginalUrl"))
            {
                string path = (string)HttpContext.Current.Items["OriginalUrl"];

                // save query string parameters to context
                RewriteContext con = new RewriteContext(HttpContext.Current.Request.QueryString, path);

                HttpContext.Current.Items["RewriteContextInfo"] = con;

                if (path.IndexOf("?") == -1)
                    path += "?";
                HttpContext.Current.RewritePath(path);
            }
        }

        protected void RewriteModule_BeginRequest(object sender, EventArgs e)
        {

            RewriteModuleSectionHandler cfg = (RewriteModuleSectionHandler)ConfigurationManager.GetSection("modulesSection/rewriteModule");

            if (!cfg.RewriteOn) return;

            string path = HttpContext.Current.Request.Path;

            if (path.Length == 0) return;

            XmlNode rules = cfg.XmlSection.SelectSingleNode("rewriteRules");
            foreach (XmlNode xml in rules.SelectNodes("rule"))
            {
                try
                {
                    Regex re = new Regex(cfg.RewriteBase + xml.Attributes["source"].InnerText, RegexOptions.IgnoreCase);
                    Match match = re.Match(path);
                    if (match.Success)
                    {
                        path = re.Replace(path, xml.Attributes["destination"].InnerText);
                        if (path.Length != 0)
                        {
                            if (HttpContext.Current.Request.QueryString.Count != 0)
                            {
                                string sign = (path.IndexOf('?') == -1) ? "?" : "&";
                                path = path + sign + HttpContext.Current.Request.QueryString;
                            }
                            string rew = cfg.RewriteBase + path;
                            HttpContext.Current.Items.Add(
                                "OriginalUrl",
                                HttpContext.Current.Request.RawUrl);
                            HttpContext.Current.RewritePath(rew);
                        }
                        return;
                    }
                }
                catch (Exception ex)
                {
                    throw (new Exception("Incorrect rule.", ex));
                }
            }
            return;
        }


        #region IHttpModule Members

        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += RewriteModule_BeginRequest;
            context.PreRequestHandlerExecute += RewriteModule_PreRequestHandlerExecute;
        }

        #endregion
    }
}