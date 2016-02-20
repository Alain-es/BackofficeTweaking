using System;
using System.IO;
using System.IO.Compression;    //Requires to add reference a to: System.IO.Compression.FileSystem
using System.Web.Hosting;
using System.Xml;

using Umbraco.Core.Logging;
using umbraco.interfaces;

using BackofficeTweaking.Helpers;


namespace BackofficeTweaking.PackageActions
{
    public class UninstallFiles : IPackageAction
    {
        private const string ActionAlias = "uninstallFiles";
        private const string UninstallFilename = "uninstall.info";

        public string Alias()
        {
            return ActionAlias;
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            string uninstallFilePath = Path.Combine(HostingEnvironment.MapPath(GetUninstallFiles(xmlData)), UninstallFilename);
            try
            {
                if (File.Exists(uninstallFilePath))
                {
                    // Remove all the files
                    string[] uninstallFileContent = File.ReadAllLines(uninstallFilePath);
                    foreach (var file in uninstallFileContent)
                    {
                        if (File.Exists(file))
                        {
                            File.Delete(file);
                        }
                    }
                    // Remove the uninstall file
                    File.Delete(uninstallFilePath);
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.Error<UninstallFiles>(string.Format("Error uninstalling the file: {0}", uninstallFilePath), ex);
                return false;
            }
            return true;
        }

        private string GetUninstallFiles(XmlNode xmlData)
        {
            return XmlHelper.GetAttributeValueFromNode(xmlData, "uninstallFiles");
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            return true;
        }

        public XmlNode SampleXml()
        {
            string sampleXml = string.Format("<Action runat=\"install\" undo=\"false\" alias=\"{0}\" uninstallFiles=\"~/App_Plugins/\" />", Alias());
            var doc = new XmlDocument();
            doc.LoadXml(sampleXml);
            return doc.DocumentElement;
        }
    }

}
