using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Umbraco.Core.Logging;

namespace BackofficeTweaking.EmbeddedAssembly
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            const string pluginBasePath = "App_Plugins/BackofficeTweaking";
            string url = string.Empty;

            RouteTable.Routes.MapRoute(
                name: "BackofficeTweaking.GetResourcePath0",
                url: pluginBasePath + "/{resource}",
                defaults: new
                {
                    controller = "EmbeddedResource",
                    action = "GetResourcePath0"
                },
                namespaces: new[] { "BackofficeTweaking.EmbeddedAssembly" }
            );

            RouteTable.Routes.MapRoute(
                name: "BackofficeTweaking.GetResourcePath1",
                url: pluginBasePath + "/{directory1}/{resource}",
                defaults: new
                {
                    controller = "EmbeddedResource",
                    action = "GetResourcePath1"
                },
                namespaces: new[] { "BackofficeTweaking.EmbeddedAssembly" }
            );

            RouteTable.Routes.MapRoute(
                name: "BackofficeTweaking.GetResourcePath2",
                url: pluginBasePath + "/{directory1}/{directory2}/{resource}",
                defaults: new
                {
                    controller = "EmbeddedResource",
                    action = "GetResourcePath2"
                },
                namespaces: new[] { "BackofficeTweaking.EmbeddedAssembly" }
            );

        }
    }
}