using System.IO;
using System.Web.Hosting;
using System.Xml;
using Umbraco.Core.Logging;
using umbraco.interfaces;

using BackofficeTweaking.Helpers;

namespace BackofficeTweaking.PackageActions
{
    /// <summary>
    /// Uninstall all sections with the alias specified as parameter
    /// </summary>
    public class UninstallSection : IPackageAction
    {
        private const string ActionAlias = "uninstallSection";

        public string Alias()
        {
            return ActionAlias;
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            string sectionToRemove = GetUninstallSection(xmlData);
            try
            {
                return DashboardHelper.UninstallSection(sectionToRemove);
            }
            catch (System.Exception ex)
            {
                LogHelper.Error<UninstallSection>(string.Format("Error removing section(s): {0}", sectionToRemove), ex);
                return false;
            }
        }

        private string GetUninstallSection(XmlNode xmlData)
        {
            return XmlHelper.GetAttributeValueFromNode(xmlData, "uninstallSection");
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            return true;
        }

        public XmlNode SampleXml()
        {
            string sampleXml = string.Format("<Action runat=\"install\" undo=\"false\" alias=\"{0}\" uninstallSection=\"MySection\" />", Alias());
            var doc = new XmlDocument();
            doc.LoadXml(sampleXml);
            return doc.DocumentElement;
        }
    }

}
