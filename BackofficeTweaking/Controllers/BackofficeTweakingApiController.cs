using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Dynamic;
using System.ComponentModel;
using System.Text;
using System.IO;

using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

using System.Web.Http;
using System.Net.Http;
using System.Web.Hosting;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
using Umbraco.Core.Logging;

using BackofficeTweaking.Helpers;

namespace BackofficeTweaking.Controllers
{
    [PluginController("BackofficeTweaking")]
    [IsBackOffice]
    public class BackofficeTweakingApiController : UmbracoAuthorizedJsonController
    {
        [System.Web.Http.HttpGet]
        public string GetRules()
        {
            string result = string.Empty;

            // Load
            XDocument xDocument = ConfigFileHelper.LoadConfig();

            try
            {
                // Convert document attributes into elements
                foreach (XElement xElement in xDocument.Descendants())
                {
                    foreach (var attribute in xElement.Attributes())
                    {
                        xElement.SetElementValue(attribute.Name, attribute.Value);
                    }
                    xElement.RemoveAttributes();
                }
                // Serialize
                result = JsonConvert.SerializeXNode(xDocument.XPathSelectElement("//Rules"));

            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(BackofficeTweakingApiController), "Error serializing rules.", ex);
            }

            return result;
        }

        [System.Web.Http.HttpPost]
        public string SaveRules([FromBody] string paramValues)
        {
            string result = "Unexpected error.";

            // Check whether parameters have a value
            if (paramValues == null)
            {
                result = "Parameters are null.";
                return result;
            }

            // Load
            XDocument xDocument = ConfigFileHelper.LoadConfig();

            // Get the values 
            try
            {

                // Deserialize Xml
                var xElement = XDocument.Parse(JsonConvert.DeserializeXmlNode(paramValues, "Rules").OuterXml);

                // Convert document elements into attributes
                IEnumerable<XElement> rules = from element in xElement.Descendants("Rule")
                                              select element;
                foreach (XElement rule in rules)
                {
                    foreach (XElement element in rule.Descendants())
                    {
                        rule.SetAttributeValue(element.Name, element.Value);
                    }
                    rule.RemoveNodes();
                }

                xDocument.Element("Config").Element("Rules").ReplaceAll(rules);

                // Save 
                result = ConfigFileHelper.SaveConfig(xDocument);
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(BackofficeTweakingApiController), "Error deserializing rules.", ex);
                result = string.Format("Error deserializing rules: {0}", ex.Message);
                return result;
            }

            return result;
        }


        [System.Web.Http.HttpGet]
        public string GetScripts()
        {
            string result = string.Empty;

            // Load
            XDocument xDocument = ConfigFileHelper.LoadConfig();

            try
            {
                // Convert document attributes into elements
                foreach (XElement xElement in xDocument.Descendants())
                {
                    foreach (var attribute in xElement.Attributes())
                    {
                        xElement.SetElementValue(attribute.Name, attribute.Value);
                    }
                    xElement.RemoveAttributes();
                }

                // Serialize
                var scripts = xDocument.Element("Config").Element("Scripts").Elements("Script").AsEnumerable();
                result = JsonConvert.SerializeObject(scripts);

            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(BackofficeTweakingApiController), "Error serializing scripts.", ex);
            }

            return result;
        }

    }
}




