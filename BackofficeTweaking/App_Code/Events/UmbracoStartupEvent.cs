using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using umbraco.cms.presentation;
using Umbraco.Core;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.web;
using Umbraco.Core.Persistence;
using Umbraco.Core.Logging;

using BackofficeTweaking.EmbeddedAssembly;
using BackofficeTweaking.Handlers;

namespace BackofficeTweaking.Events
{
    public class UmbracoStartupEvent : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            //LogHelper.Info(typeof(UmbracoStartupEvent), string.Format("Startup event ..."));

            // Register routes for embedded files
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Process rules
            System.Web.Http.GlobalConfiguration.Configuration.MessageHandlers.Add(new RulesHandler());

        }
    }
}