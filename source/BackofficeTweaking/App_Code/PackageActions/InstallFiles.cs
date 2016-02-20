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
    public class InstallFiles : IPackageAction
    {
        private const string ActionAlias = "installFiles";
        private const string UninstallFilename = "uninstall.info";

        private string _TempDirectory = HostingEnvironment.MapPath("~/App_Data/TEMP/Install/");

        public string Alias()
        {
            return ActionAlias;
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            string fileToInstallPath = HostingEnvironment.MapPath(GetInstallFiles(xmlData));
            try
            {
                if (File.Exists(fileToInstallPath))
                {
                    // Get the install directory
                    var installDirectory = Path.GetDirectoryName(fileToInstallPath);

                    // Create temp directory
                    _TempDirectory += Guid.NewGuid().ToString();
                    Directory.CreateDirectory(_TempDirectory);

                    // Unzip all files into the temp directory
                    ZipFile.ExtractToDirectory(fileToInstallPath, _TempDirectory);

                    // Create a listing with all the files and move the files to the installation directory
                    string uninstallFilePath = Path.Combine(installDirectory, UninstallFilename);
                    if (!File.Exists(uninstallFilePath))
                    {
                        File.WriteAllText(uninstallFilePath, string.Empty);
                    }
                    foreach (var file in Directory.GetFiles(_TempDirectory, "*", SearchOption.AllDirectories))
                    {
                        string destinationFilePath = installDirectory + file.Replace(_TempDirectory, string.Empty);
                        File.AppendAllText(uninstallFilePath, destinationFilePath + Environment.NewLine);
                        if (File.Exists(destinationFilePath))
                        {
                            File.Delete(destinationFilePath);
                        }
                        if (!Directory.Exists(Path.GetDirectoryName(destinationFilePath)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(destinationFilePath));
                        }
                        File.Move(file, destinationFilePath);
                    }

                    // Remove the temp directory
                    Directory.Delete(_TempDirectory, true);

                    // Remove the install fiel
                    File.Delete(fileToInstallPath);

                }
            }
            catch (System.Exception ex)
            {
                LogHelper.Error<InstallFiles>(string.Format("Error installing the file: {0}", fileToInstallPath), ex);
                return false;
            }
            return true;
        }

        private string GetInstallFiles(XmlNode xmlData)
        {
            return XmlHelper.GetAttributeValueFromNode(xmlData, "installFiles");
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            string packageDirectory = Path.GetDirectoryName(HostingEnvironment.MapPath(GetInstallFiles(xmlData)));
            string uninstallFilePath = Path.Combine(packageDirectory, UninstallFilename);
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
                    // Remove all empty directories
                    foreach (var directory in Directory.GetDirectories(packageDirectory, "", SearchOption.AllDirectories))
                    {
                        if (Directory.Exists(directory) && Directory.GetFiles(directory, "", SearchOption.AllDirectories).Length == 0)
                        {
                            try
                            {
                            Directory.Delete(directory, true);
                        }
                            catch (Exception)
                            {
                            }
                    }
                    }
                    if (Directory.Exists(packageDirectory) && Directory.GetFiles(packageDirectory, "", SearchOption.AllDirectories).Length == 0)
                    {
                        try
                        {
                        Directory.Delete(packageDirectory, true);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.Error<UninstallFiles>(string.Format("Error uninstalling the file: {0}", uninstallFilePath), ex);
                return false;
            }
            return true;
        }

        public XmlNode SampleXml()
        {
            string sampleXml = string.Format("<Action runat=\"install\" undo=\"false\" alias=\"{0}\" installFiles=\"~/App_Plugins/FilesToInstall.zip\" />", Alias());
            var doc = new XmlDocument();
            doc.LoadXml(sampleXml);
            return doc.DocumentElement;
        }
    }

}
