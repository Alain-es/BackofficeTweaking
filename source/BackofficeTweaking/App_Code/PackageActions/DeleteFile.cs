using System.IO;
using System.Web.Hosting;
using System.Xml;
using Umbraco.Core.Logging;
using umbraco.interfaces;

using BackofficeTweaking.Helpers;

namespace BackofficeTweaking.PackageActions
{
    /// <summary>
    /// Delete a file
    /// </summary>
    public class DeleteFile : IPackageAction
    {
        private const string ActionAlias = "deleteFile";

        public string Alias()
        {
            return ActionAlias;
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            string fileToDelete = GetDeleteFile(xmlData);
            try
            {
                if (File.Exists(HostingEnvironment.MapPath(fileToDelete)))
                {
                    File.Delete(HostingEnvironment.MapPath(fileToDelete));
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.Error<DeleteFile>(string.Format("Error deleting the file: {0}", fileToDelete), ex);
                return false;
            }
            return true;
        }

        private string GetDeleteFile(XmlNode xmlData)
        {
            return XmlHelper.GetAttributeValueFromNode(xmlData, "deleteFile");
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            return true;
        }

        public XmlNode SampleXml()
        {
            string sampleXml = string.Format("<Action runat=\"install\" undo=\"false\" alias=\"{0}\" deleteFile=\"~/bin/example.txt\" />", Alias());
            var doc = new XmlDocument();
            doc.LoadXml(sampleXml);
            return doc.DocumentElement;
        }
    }

}
