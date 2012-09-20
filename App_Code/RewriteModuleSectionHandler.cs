using System;
using System.Configuration;
using System.Web;
using System.Xml;

namespace BWA.Knowledgebase.Modules
{
    public class RewriteModuleSectionHandler : IConfigurationSectionHandler
    {

        public XmlNode XmlSection { get; set; }
        public string RewriteBase { get; set; }
        public bool RewriteOn { get; set; }


        public object Create(object parent, object configContext, XmlNode section)
        {
            if (!HttpContext.Current.Request.ApplicationPath.EndsWith("/"))
            {
                RewriteBase = HttpContext.Current.Request.ApplicationPath + "/";
            }

            try
            {
                XmlSection = section;
                RewriteOn = Convert.ToBoolean(section.SelectSingleNode("rewriteOn").InnerText);
            }
            catch (Exception ex)
            {
                throw (new Exception("Error while processing RewriteModule configuration section.", ex));
            }
            return this;
        }
    }
}