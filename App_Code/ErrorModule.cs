using System;
using System.IO;
using System.Web;

namespace BWA.Knowledgebase.Modules
{
    public class ErrorModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.Error += new EventHandler(Error);
        }

        protected void Error(object sender, EventArgs e)
        {
            try
            {
                Exception ex = HttpContext.Current.Server.GetLastError();
                File.WriteAllText(HttpContext.Current.Server.MapPath("~/Logs/" + DateTime.Now.Ticks.ToString() + ".txt"),
                String.Format("{0}\n{1}\nException: {2}\n{3}\n\n",
                    DateTime.Now.ToLongDateString(),
                    HttpContext.Current.Request.Url.AbsoluteUri,
                    ex.Message,
                    ex.ToString()));
            }
            catch (Exception) { }

        }

        public void Dispose()
        {

        }
    }
}