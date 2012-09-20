using System;
using Micajah.FileService.WebControls;
using Micajah.Common.Security;

namespace BWA.Knowledgebase
{
    public class Global : Micajah.Common.Application.WebApplication
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            base.Application_Start(sender, e);
            Micajah.FileService.Client.ResourceVirtualPathProvider.Register();
        }

        protected override void Application_BeginRequest(object sender, EventArgs e)
        {
            base.Application_BeginRequest(sender, e);
        }
        protected override void Application_PostAcquireRequestState(object sender, EventArgs e)
        {
            base.Application_PostAcquireRequestState(sender, e);
        }
        protected override void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            base.Application_PostAuthenticateRequest(sender, e);
        }
    }
}
