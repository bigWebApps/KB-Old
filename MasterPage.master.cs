using System;
using Micajah.Common.Security;
using System.Web.UI.WebControls;
using Micajah.Common.Bll;
using System.Globalization;

namespace BWA.Knowledgebase
{
    public partial class MasterPage : Micajah.Common.Pages.MasterPage
    {
        public string Title
        {
            get
            {
                return (string)ViewState["Title"];
            }
            set
            {
                ViewState["Title"] = value;
            }
        }

        public Guid InstanceGuid
        {
            get
            {
                Guid g = Guid.Empty;
                if (UserContext.SelectedInstanceId != Guid.Empty)
                    g = UserContext.SelectedInstanceId;
                else if (!string.IsNullOrEmpty(Request.QueryString["i"]))
                {
                    try { g = new Guid(Request.QueryString["i"]); }
                    catch { }
                }
                return g;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            this.SearchButtonClick += new CommandEventHandler(MasterPage_SearchButtonClick);
            base.OnInit(e);
        }

        protected void MasterPage_SearchButtonClick(object sender, CommandEventArgs e)
        {
            if (UserContext.Current != null)
                Response.Redirect(string.Format("~/SearchResultAdmin.aspx?search={0}", e.CommandArgument.ToString()), false);
            else if (!this.InstanceGuid.Equals(Guid.Empty))
                Response.Redirect(string.Format("~/SearchResult.aspx?{0}search={1}", (UserContext.SelectedInstanceId != Guid.Empty ? string.Empty : "i=" + this.InstanceGuid.ToString("N") + "&"), e.CommandArgument.ToString()), false);
        }

        protected void Page_Load(object sender, EventArgs args)
        {            
            if (this.InstanceGuid != Guid.Empty)
            {
                Instance inst = Micajah.Common.Bll.Providers.InstanceProvider.GetInstance(this.InstanceGuid, Guid.Empty);
                if (inst != null && inst.Organization != null)
                {
                    Organization org = Micajah.Common.Bll.Providers.InstanceProvider.GetInstance(this.InstanceGuid, Guid.Empty).Organization;
                    //this.HeaderLogoText = string.Format(CultureInfo.CurrentCulture, "{0} {1}", org.Name, inst.Name);
                    //this.HeaderLogoNavigateUrl = org.WebsiteUrl;
                    //this.HeaderLogoImageUrl = org.LogoImageUrl;
                    this.Title = string.Format(CultureInfo.CurrentCulture, "{2}{0} {1}", org.Name, inst.Name, string.IsNullOrEmpty(this.Title) ? string.Empty : this.Title + " - ");
                    Setting trackCode = inst.Settings.FindByShortName("TrackingCode");
                    if (trackCode != null && !string.IsNullOrEmpty(trackCode.Value))
                        LiteralTrackingCode.Text = trackCode.Value;
                }
            }
            if (this.ActiveAction != null)
                base.VisibleBreadcrumbs = (this.ActiveAction.OrderNumber >= 0);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (!string.IsNullOrEmpty(this.Title))
                title1.Text = this.Title;
        }

    }
}