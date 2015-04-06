using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace BackofficeTweaking.Helpers
{
    public class DashboardHelper
    {
        // Partially taken from https://github.com/kipusoep/UrlTracker/blob/b2bc998adaae2f74779e1119fd7b1e97a3b71ed2/UI/Installer/UrlTrackerInstallerService.asmx.cs

        public static bool InstallDashboard(string section, string caption, string control)
        {
            bool result = false;
            try
            {
                string dashboardConfig;
                string dashboardConfigPath = HttpContext.Current.Server.MapPath("~/config/dashboard.config");
                using (StreamReader streamReader = File.OpenText(dashboardConfigPath))
                    dashboardConfig = streamReader.ReadToEnd();
                if (string.IsNullOrEmpty(dashboardConfig))
                    throw new Exception("Unable to add dashboard: Couldn't read current ~/config/dashboard.config, permissions issue?");
                XDocument dashboardDoc = XDocument.Parse(dashboardConfig, LoadOptions.PreserveWhitespace);
                if (dashboardDoc == null)
                    throw new Exception("Unable to add dashboard: Unable to parse current ~/config/dashboard.config file, invalid XML?");
                XElement dashBoardElement = dashboardDoc.Element("dashBoard");
                if (dashBoardElement == null)
                    throw new Exception("Unable to add dashboard: dashBoard element not found in ~/config/dashboard.config file");
                List<XElement> sectionElements = dashBoardElement.Elements("section").ToList();
                if (sectionElements == null || !sectionElements.Any())
                    throw new Exception("Unable to add dashboard: No section elements found in ~/config/dashboard.config file");
                XElement existingSectionElement = sectionElements.SingleOrDefault(x => x.Attribute("alias") != null && x.Attribute("alias").Value == section);
                if (existingSectionElement == null)
                    throw new Exception(string.Format("Unable to add dashboard: '{0}' section not found in ~/config/dashboard.config", section));

                List<XElement> tabs = existingSectionElement.Elements("tab").ToList();
                if (!tabs.Any())
                    throw new Exception(string.Format("Unable to add dashboard: No existing tabs found within the '{0}' section", section));

                List<XElement> existingTabs = tabs.Where(x => x.Attribute("caption").Value == caption).ToList();
                if (existingTabs.Any())
                {
                    foreach (XElement tab in existingTabs)
                    {
                        List<XElement> existingTabControls = tab.Elements("control").ToList();
                        if (existingTabControls.Any(x => x.Value == control))
                            return true;
                    }
                }

                XElement lastTab = tabs.Last();
                XElement newTab = new XElement("tab");
                newTab.Add(new XAttribute("caption", caption));
                XElement newControl = new XElement("control");
                newControl.Add(new XAttribute("addPanel", true));
                newControl.SetValue(control);
                newTab.Add(newControl);
                newControl.AddBeforeSelf(string.Concat(Environment.NewLine, "      "));
                newControl.AddAfterSelf(string.Concat(Environment.NewLine, "    "));
                lastTab.AddAfterSelf(newTab);
                lastTab.AddAfterSelf(string.Concat(Environment.NewLine, "    "));
                dashboardDoc.Save(dashboardConfigPath, SaveOptions.None);
                result = true;
            }
            catch (Exception ex)
            {
                ExceptionHelper.LogExceptionMessage(typeof(DashboardHelper), ex);
            }
            return result;
        }

        public static bool UninstallDashboard(string section, string caption, string control)
        {
            bool result = false;
            try
            {
                string dashboardConfig;
                string dashboardConfigPath = HttpContext.Current.Server.MapPath("~/config/dashboard.config");
                using (StreamReader streamReader = File.OpenText(dashboardConfigPath))
                    dashboardConfig = streamReader.ReadToEnd();
                if (string.IsNullOrEmpty(dashboardConfig))
                    throw new Exception("Unable to remove dashboard: Couldn't read current ~/config/dashboard.config, permissions issue?");
                XDocument dashboardDoc = XDocument.Parse(dashboardConfig, LoadOptions.PreserveWhitespace);
                if (dashboardDoc == null)
                    throw new Exception("Unable to remove dashboard: Unable to parse current ~/config/dashboard.config file, invalid XML?");
                XElement dashBoardElement = dashboardDoc.Element("dashBoard");
                if (dashBoardElement == null)
                    throw new Exception("Unable to remove dashboard: dashBoard element not found in ~/config/dashboard.config file");
                List<XElement> sectionElements = dashBoardElement.Elements("section").ToList();
                if (sectionElements == null || !sectionElements.Any())
                    return true;
                XElement existingSectionElement = sectionElements.SingleOrDefault(x => x.Attribute("alias") != null && x.Attribute("alias").Value == section);
                if (existingSectionElement == null)
                    return true;

                List<XElement> tabs = existingSectionElement.Elements("tab").ToList();
                if (!tabs.Any())
                    return true;

                List<XElement> existingTabs = tabs.Where(x => x.Attribute("caption").Value == caption).ToList();
                if (existingTabs.Any())
                {
                    foreach (XElement tab in existingTabs)
                    {
                        List<XElement> existingTabControls = tab.Elements("control").ToList();
                        if (existingTabControls.Any(x => x.Value == control))
                        {
                            tab.Remove();
                        }
                    }
                }
                dashboardDoc.Save(dashboardConfigPath, SaveOptions.None);
                result = true;
            }
            catch (Exception ex)
            {
                ExceptionHelper.LogExceptionMessage(typeof(DashboardHelper), ex);
            }
            return result;
        }

        public static bool UninstallSection(string sectionAlias)
        {
            bool result = false;
            try
            {
                string dashboardConfig;
                string dashboardConfigPath = HttpContext.Current.Server.MapPath("~/config/dashboard.config");
                using (StreamReader streamReader = File.OpenText(dashboardConfigPath))
                    dashboardConfig = streamReader.ReadToEnd();
                if (string.IsNullOrEmpty(dashboardConfig))
                    throw new Exception("Unable to remove dashboard: Couldn't read current ~/config/dashboard.config, permissions issue?");
                XDocument dashboardDoc = XDocument.Parse(dashboardConfig, LoadOptions.PreserveWhitespace);
                if (dashboardDoc == null)
                    throw new Exception("Unable to remove dashboard: Unable to parse current ~/config/dashboard.config file, invalid XML?");
                XElement dashBoardElement = dashboardDoc.Element("dashBoard");
                if (dashBoardElement == null)
                    throw new Exception("Unable to remove dashboard: dashBoard element not found in ~/config/dashboard.config file");
                List<XElement> sectionElements = dashBoardElement.Elements("section").ToList();
                if (sectionElements == null || !sectionElements.Any())
                    return true;
                List<XElement> existingSectionElements = sectionElements.Where(x => x.Attribute("alias") != null && x.Attribute("alias").Value == sectionAlias).ToList();
                if (existingSectionElements != null)
                {
                    foreach (XElement sectionElement in existingSectionElements)
                    {
                        sectionElement.Remove();
                    }
                    dashboardDoc.Save(dashboardConfigPath, SaveOptions.None);
                }
                result = true;
            }
            catch (Exception ex)
            {
                ExceptionHelper.LogExceptionMessage(typeof(DashboardHelper), ex);
            }
            return result;
        }


    }
}