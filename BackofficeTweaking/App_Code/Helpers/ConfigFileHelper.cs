using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;

using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Core;
using Umbraco.Core.Strings;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Logging;

using BackofficeTweaking.Models;
using BackofficeTweaking.Extensions;

namespace BackofficeTweaking.Helpers
{
    public enum RuleType
    {
        HideTabs = 0,
        HideProperties = 1,
        HideButtons = 2,
        HidePanels = 3,
        HideLabels = 4,
        RunScripts = 5
    }

    public class ConfigFileHelper
    {
        private const string _CacheIdRules = "BackofficeTweaking.CacheId.CachedRules";
        private const string _CacheIdScripts = "BackofficeTweaking.CacheId.CachedScripts";
        private const string _ConfigFile = "~/Config/BackofficeTweaking.config";

        public static IEnumerable<Rule> getRulesForUser(IUser user)
        {

            IEnumerable<Rule> result = new List<Rule>();

            // If no user is logged in then omit all rules
            if (user == null)
            {
                return result;
            }

            // Current user name and user type
            var currentUsername = user.Username.ToLower();
            var currentUsertype = user.UserType.Alias.ToLower();

            // If the current user is admin then omit all rules
            if (currentUsertype.Equals("admin", StringComparison.InvariantCultureIgnoreCase))
            {
                return result;
            }

            // Get rules from the cache
            result = HttpContext.Current.Cache.Get(_CacheIdRules) as IEnumerable<Rule>;
            if (result == null)
            {
                // If no rules are cached then get them from the config file
                LoadAndCacheConfig();
                result = HttpContext.Current.Cache.Get(_CacheIdRules) as IEnumerable<Rule>;
            }

            // Filter rules to apply for the current user
            result = result.Where(x =>
                (string.IsNullOrWhiteSpace(x.Users) && string.IsNullOrWhiteSpace(x.UserTypes)) ||
                (!string.IsNullOrWhiteSpace(x.Users) && string.IsNullOrWhiteSpace(x.UserTypes) && x.Users.ToLower().ToDelimitedList().Contains(currentUsername)) ||
                (string.IsNullOrWhiteSpace(x.UserTypes) && !string.IsNullOrWhiteSpace(x.UserTypes) && x.UserTypes.ToLower().ToDelimitedList().Contains(currentUsertype)) ||
                (x.Users.ToLower().ToDelimitedList().Contains(currentUsername) || x.UserTypes.ToLower().ToDelimitedList().Contains(currentUsertype))
            );

            return result;
        }


        public static IEnumerable<Script> getScripts()
        {
            IEnumerable<Script> result = new List<Script>();

            // Get scripts from the cache
            result = HttpContext.Current.Cache.Get(_CacheIdScripts) as IEnumerable<Script>;
            if (result == null)
            {
                // If no scripts are cached then get them from the config file
                LoadAndCacheConfig();
                result = HttpContext.Current.Cache.Get(_CacheIdScripts) as IEnumerable<Script>;
            }

            return result;
        }


        public static void LoadAndCacheConfig()
        {
            string configFilePath = System.Web.Hosting.HostingEnvironment.MapPath(_ConfigFile);

            try
            {

                // Get rules and scripts from cache
                if (HttpContext.Current.Cache.Get(_CacheIdRules) as IEnumerable<Rule> == null || HttpContext.Current.Cache.Get(_CacheIdScripts) as IEnumerable<Script> == null)
                {

                    // Check whether the config file exists
                    if (!File.Exists(configFilePath) || string.IsNullOrWhiteSpace(System.IO.File.ReadAllText(configFilePath)))
                    {
                        // Create a new config file with default values
                        XDocument xDocument = new XDocument(
                            new XElement("Config",

                                new XElement("Rules",
                                    new XElement("Rule",
                                        new XAttribute("Type", "HideProperties"),
                                        new XAttribute("Enabled", "false"),
                                        new XAttribute("Names", "property1Alias"),
                                        new XAttribute("Users", ""),
                                        new XAttribute("UserTypes", ""),
                                        new XAttribute("ContentIds", ""),
                                        new XAttribute("ContentTypes", ""),
                                        new XAttribute("Description", "Example")
                                    )
                                ),
                                new XElement("Scripts",
                                    new XElement("Script",
                                        new XAttribute("Name", "example"),
                                        new XAttribute("Content", "console.log('Hello world');")
                                    )
                                )
                            )
                        );
                        xDocument.Save(configFilePath);
                    }

                    // Load config
                    XElement xelement = XElement.Load(configFilePath);

                    // Check whether the <Config> section exists and create it if it doesn't
                    if (!xelement.Name.LocalName.Equals("Config"))
                    {
                        XDocument xDocument = new XDocument(
                            new XElement("Config",
                                xelement,
                                new XElement("Scripts",
                                    new XElement("Script",
                                        new XAttribute("Name", "example"),
                                        new XAttribute("Content", "console.log('Hello world');")
                                    )
                                )
                            )
                        );
                        xDocument.Save(configFilePath);
                        xelement = XElement.Load(configFilePath);
                    }

                    // Check whether the "Enabled" attribute is a valid boolean value. 
                    bool requireSaving = false;
                    foreach (XElement element in xelement.Element("Rules").Descendants())
                    {
                        bool enabledAttribute;
                        if (!bool.TryParse(element.Attribute("Enabled").Value, out enabledAttribute))
                        {
                            element.SetAttributeValue("Enabled", "false");
                            requireSaving = true;
                        }
                    }
                    if (requireSaving)
                    {
                        xelement.Save(configFilePath);
                    }

                    var Rules = from rule in xelement.Element("Rules").Elements("Rule")
                                select new Rule()
                                {
                                    Type = rule.Attribute("Type").Value,
                                    Enabled = Convert.ToBoolean(rule.Attribute("Enabled").Value),
                                    Names = rule.Attribute("Names").Value,
                                    UserTypes = rule.Attribute("UserTypes").Value,
                                    Users = rule.Attribute("Users").Value,
                                    ContentIds = rule.Attributes().Any(a => a.Name.ToString().InvariantEquals("ContentIds")) ? rule.Attribute("ContentIds").Value : string.Empty,
                                    ContentTypes = rule.Attribute("ContentTypes").Value,
                                    Description = rule.Attribute("Description").Value
                                };
                    // Cache the result for a year but with a dependency on the config file
                    HttpContext.Current.Cache.Add(_CacheIdRules, Rules, new CacheDependency(configFilePath), DateTime.Now.AddYears(1), Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, null);

                    var Scripts = from script in xelement.Element("Scripts").Elements("Script")
                                  select new Script()
                                  {
                                      Name = script.Attribute("Name").Value,
                                      Content = script.Attribute("Content").Value
                                  };
                    // Cache the result for a year but with a dependency on the config file
                    HttpContext.Current.Cache.Add(_CacheIdScripts, Scripts, new CacheDependency(configFilePath), DateTime.Now.AddYears(1), Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, null);

                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(ConfigFileHelper), string.Format("Error loading rules from the config file : {0}", configFilePath), ex);
            }

        }


        public static XDocument LoadConfig()
        {
            var result = new XDocument();
            string configFilePath = System.Web.Hosting.HostingEnvironment.MapPath(_ConfigFile);
            LoadAndCacheConfig(); // Creates a new config file if it doesn't exist
            try
            {
                result = XDocument.Load(configFilePath);
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(ConfigFileHelper), string.Format("Error loading rules/scripts from the config file : {0}", configFilePath), ex);
            }
            return result;
        }


        public static string SaveConfig(XDocument config)
        {
            string result = "Unexpected error.";
            string configFilePath = System.Web.Hosting.HostingEnvironment.MapPath(_ConfigFile);
            try
            {
                config.Save(configFilePath);
                result = string.Empty; // No error
            }
            catch (Exception ex)
            {
                result = string.Format("Error saving the config file {0}: {1}", configFilePath, ex.Message);
                LogHelper.Error(typeof(ConfigFileHelper), string.Format("Error saving the config file : {0}", configFilePath), ex);
            }
            return result;
        }

    }

}