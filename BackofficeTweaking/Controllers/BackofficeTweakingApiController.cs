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
using System.Web.Caching;

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
        private const string _CacheIdApiControllerRules = "BackofficeTweaking.CacheId.ApiControllerRules";
        private const string _CacheIdApiControllerScripts = "BackofficeTweaking.CacheId.ApiControllerScripts";

        [System.Web.Http.HttpGet]
        public string GetRules()
        {
            string result = string.Empty;
            var cachedResult = HttpContext.Current.Cache.Get(_CacheIdApiControllerRules);
            if (cachedResult != null && !string.IsNullOrWhiteSpace(cachedResult.ToString()))
            {
                return cachedResult.ToString();
            }
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
                // Cache the result for a year but with a dependency on the config file
                HttpContext.Current.Cache.Add(_CacheIdApiControllerRules, result, new CacheDependency(ConfigFileHelper.getConfigFilePath()), DateTime.Now.AddYears(1), Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, null);
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
                // Save the rules
                // Deserialize Xml Rules
                var xElementRules = XDocument.Parse(JsonConvert.DeserializeXmlNode(paramValues, "Rules").OuterXml);

                // Convert document elements into attributes
                IEnumerable<XElement> rules = from element in xElementRules.Descendants("Rule")
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

                //// Save the scripts 
                //// Deserialize Xml Scripts
                //var xElementScripts = XDocument.Parse(JsonConvert.DeserializeXmlNode(paramValues, "Scripts").OuterXml);
                //// Convert document elements into attributes
                //IEnumerable<XElement> scripts = from element in xElementScripts.Descendants("Script")
                //                                select element;
                //foreach (XElement script in scripts)
                //{
                //    foreach (XElement element in script.Descendants())
                //    {
                //        if (!element.Name.Equals("Content"))
                //        {
                //            script.SetAttributeValue(element.Name, element.Value);
                //        }
                //        else
                //        {
                //            // The "Content" vale is not saved as an attribute 
                //            script.Value = element.Value;
                //        }
                //    }
                //    script.RemoveNodes();
                //}
                //xDocument.Element("Config").Element("Scripts").ReplaceAll(scripts);

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

            var cachedResult = HttpContext.Current.Cache.Get(_CacheIdApiControllerScripts);
            if (cachedResult != null && !string.IsNullOrWhiteSpace(cachedResult.ToString()))
            {
                return cachedResult.ToString();
            }
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
                // Cache the result for a year but with a dependency on the config file
                HttpContext.Current.Cache.Add(_CacheIdApiControllerScripts, result, new CacheDependency(ConfigFileHelper.getConfigFilePath()), DateTime.Now.AddYears(1), Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, null);
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(BackofficeTweakingApiController), "Error serializing scripts.", ex);
            }

            return result;
        }

    }
}




