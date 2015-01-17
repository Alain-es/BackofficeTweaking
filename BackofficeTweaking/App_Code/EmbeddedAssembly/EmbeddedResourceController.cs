using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Umbraco.Core.Logging;

namespace BackofficeTweaking.EmbeddedAssembly
{
    // Get embedded resources files (.html, .js, .css, ...) 
    public class EmbeddedResourceController : Controller
    {

        public FileStreamResult GetResourcePath0(string resource)
        {
            var path = "/";
            return GetResource(path, resource);
        }

        public FileStreamResult GetResourcePath1(string directory1, string resource)
        {
            var path = string.Format("/{0}/", directory1);
            return GetResource(path, resource);
        }

        public FileStreamResult GetResourcePath2(string directory1, string directory2, string resource)
        {
            var path = string.Format("/{0}/{1}/", directory1, directory2);
            return GetResource(path, resource);
        }

        private FileStreamResult GetResource(string url, string resource)
        {
            try
            {
                // Get this assembly
                Assembly assembly = typeof(EmbeddedResourceController).Assembly;

                // If resource can be found
                string resourceName = string.Format("{0}.{1}{2}{3}", assembly.GetName().Name, "App_Plugins", url.Replace("/", "."), resource);
                //LogHelper.Info(typeof(EmbeddedResourceController), string.Format("Getting the resource: {0}", resourceName));
                if (Assembly.GetExecutingAssembly().GetManifestResourceNames().Any(x => x.Equals(resourceName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return new FileStreamResult(assembly.GetManifestResourceStream(resourceName), this.GetMimeType(resourceName));
                }
                else
                {
                    LogHelper.Warn(typeof(EmbeddedResourceController), string.Format("Couldn't get the resource: {0}{1}", url, resource));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(EmbeddedResourceController), string.Format("Couldn't get the resource: {0}{1}", url, resource), ex);
            }

            return null;
        }

        private string GetMimeType(string resource)
        {
            switch (Path.GetExtension(resource))
            {
                case ".html": return "text/html";
                case ".css": return "text/css";
                case ".js": return "text/javascript";
                case ".png": return "image/png";
                case ".jpg": return "image/jpeg";
                case ".jpeg": return "image/jpeg";
                case ".gif": return "image/gif";
                default: return "text";
            }
        }

    }
}