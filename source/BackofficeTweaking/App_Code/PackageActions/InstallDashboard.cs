using System.IO;
using System.Web.Hosting;
using System.Xml;
using Umbraco.Core.Logging;
using umbraco.interfaces;

using BackofficeTweaking.Helpers;

namespace BackofficeTweaking.PackageActions
{
    public class InstallDashboard : IPackageAction
    {
        private const string ActionAlias = "installDashboard";

        public string Alias()
        {
            return ActionAlias;
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            string dashboardSection = GetDashboardSection(xmlData);
            string dashboardCaption = GetDashboardCaption(xmlData);
            string dashboardControl = GetDashboardControl(xmlData);
            return DashboardHelper.InstallDashboard(dashboardSection, dashboardCaption, dashboardControl);
        }

        private string GetDashboardSection(XmlNode xmlData)
        {
            return XmlHelper.GetAttributeValueFromNode(xmlData, "dashboardSection");
        }

        private string GetDashboardCaption(XmlNode xmlData)
        {
            return XmlHelper.GetAttributeValueFromNode(xmlData, "dashboardCaption");
        }

        private string GetDashboardControl(XmlNode xmlData)
        {
            return XmlHelper.GetAttributeValueFromNode(xmlData, "dashboardControl");
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            string dashboardSection = GetDashboardSection(xmlData);
            string dashboardCaption = GetDashboardCaption(xmlData);
            string dashboardControl = GetDashboardControl(xmlData);
            return DashboardHelper.UninstallDashboard(dashboardSection, dashboardCaption, dashboardControl);
        }

        public XmlNode SampleXml()
        {
            string sampleXml = string.Format("<Action runat=\"install\" undo=\"false\" alias=\"{0}\" GetDashboardSection=\"Developer\" dashboardCaption=\"My Dashboard\" dashboardControl=\"/App_Plugins/example/myControl.html\" />", Alias());
            var doc = new XmlDocument();
            doc.LoadXml(sampleXml);
            return doc.DocumentElement;
        }
    }

}
