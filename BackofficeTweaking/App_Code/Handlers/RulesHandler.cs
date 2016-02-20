using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Web.Http.Dispatcher;

using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Web;
using Umbraco.Web.Models.ContentEditing;

using BackofficeTweaking.Helpers;
using BackofficeTweaking.Models;
using BackofficeTweaking.Extensions;

namespace BackofficeTweaking.Handlers
{

    public class RulesHandler : DelegatingHandler
    {

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Task<HttpResponseMessage> result = null;
            try
            {
                switch (request.RequestUri.AbsolutePath.ToLower())
                {
                    case "/umbraco/backoffice/umbracoapi/content/getempty":
                    case "/umbraco/backoffice/umbracoapi/content/getbyid":
                        // Get rules for the current user
                        var user = UmbracoContext.Current.Application.Services.UserService.GetUserById(UmbracoContext.Current.Security.GetUserId());
                        IEnumerable<Rule> rules = ConfigFileHelper.getRulesForUser(user);
                        // Process rules
                        result = ProcessRules(request, cancellationToken, rules);
                        break;
                    default:
                        result = base.SendAsync(request, cancellationToken);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(RulesHandler), "Error handling the request.", ex);
            }

            return result;

        }

        private Task<HttpResponseMessage> ProcessRules(HttpRequestMessage request, CancellationToken cancellationToken, IEnumerable<Rule> rules)
        {
            return base.SendAsync(request, cancellationToken)
                    .ContinueWith(task =>
                    {
                        var response = task.Result;

                        try
                        {
                            //Check whether there are rules for the current content
                            var data = response.Content;
                            var content = ((ObjectContent)(data)).Value as ContentItemDisplay;
                            rules = rules.Where(r =>
                                r.Enabled == true
                                && (string.IsNullOrWhiteSpace(r.ContentTypes) || r.ContentTypes.ToDelimitedList().InvariantContains(content.ContentTypeAlias))
                                && (string.IsNullOrWhiteSpace(r.ContentIds) || r.ContentIds.ToDelimitedList().InvariantContains(content.Id.ToString()))
                                && (string.IsNullOrWhiteSpace(r.ParentContentIds) || r.ParentContentIds.ToDelimitedList().InvariantContains(content.ParentId.ToString())));

                            if (rules.Count() < 1)
                            {
                                return response;
                            }

                            List<string> hideProperties = new List<string>();
                            List<string> hideTabs = new List<string>();
                            List<string> hideButtons = new List<string>();
                            List<string> hidePanels = new List<string>();
                            List<string> hideLabels = new List<string>();
                            List<string> runScripts = new List<string>();

                            // Get properties to hide
                            foreach (var propertyRule in rules.Where(r =>
                                r.Type.InvariantEquals(RuleType.HideProperties.ToString())
                                && !string.IsNullOrWhiteSpace(r.Names)
                                ))
                            {
                                // Remove all properties that don't exist for the current ContentType (A rule can be very generic and include properties that don't belong to the current ContentType)
                                IEnumerable<string> currentContentTypeProperties = content.Properties.Select(p => p.Alias);
                                hideProperties
                                    .AddRangeUnique(propertyRule.Names.ToDelimitedList().ToList()
                                        .Where(n => currentContentTypeProperties.InvariantContains(n))
                                    );
                            }

                            // Get tabs to hide
                            foreach (var tabRule in rules.Where(r =>
                                r.Type.InvariantEquals(RuleType.HideTabs.ToString())
                                && !string.IsNullOrWhiteSpace(r.Names)
                                ))
                            {
                                // Remove all tabs that don't exist for the current ContentType (A rule can be very generic and include tabs that don't belong to the current ContentType)
                                IEnumerable<string> currentContentTypeTabs = content.Tabs.Select(t => t.Label);
                                hideTabs
                                    .AddRangeUnique(tabRule.Names.ToDelimitedList().ToList()
                                        .Where(n => currentContentTypeTabs.InvariantContains(n))
                                    );
                            }

                            // Get buttons to hide
                            foreach (var buttonRule in rules.Where(r =>
                                r.Type.InvariantEquals(RuleType.HideButtons.ToString())
                                && !string.IsNullOrWhiteSpace(r.Names)
                                ))
                            {
                                hideButtons.AddRangeUnique(buttonRule.Names.ToDelimitedList().ToList());
                            }

                            // Get panels to hide
                            foreach (var panelRule in rules.Where(r =>
                                r.Type.InvariantEquals(RuleType.HidePanels.ToString())
                                && !string.IsNullOrWhiteSpace(r.Names)
                                ))
                            {
                                hidePanels.AddRangeUnique(panelRule.Names.ToDelimitedList().ToList());
                            }

                            // Get labels to hide
                            foreach (var labelRule in rules.Where(r =>
                                r.Type.InvariantEquals(RuleType.HideLabels.ToString())
                                && !string.IsNullOrWhiteSpace(r.Names)
                                ))
                            {
                                // Remove all labels that don't exist for the current ContentType (A rule can be very generic and include labels that don't belong to the current ContentType)
                                IEnumerable<string> currentContentTypeProperties = content.Properties.Select(p => p.Alias);
                                hideLabels
                                    .AddRangeUnique(labelRule.Names.ToDelimitedList().ToList()
                                        .Where(n => currentContentTypeProperties.InvariantContains(n))
                                    );
                            }

                            // Get scripts to run
                            foreach (var scriptRule in rules.Where(r =>
                                r.Type.InvariantEquals(RuleType.RunScripts.ToString())
                                && !string.IsNullOrWhiteSpace(r.Names)
                                ))
                            {
                                runScripts.AddRangeUnique(scriptRule.Names.ToDelimitedList().ToList());
                            }


                            // Get the first property of the first visible tab in order to add to its config everything that should be run only once (hide tabs, hide buttons, hide panels)
                            var firstProperty = content.Tabs.FirstOrDefault(t => t.IsActive == true).Properties.FirstOrDefault();

                            // Tabs
                            if (hideTabs.Count() > 0)
                            {
                                firstProperty.Config.Add("hidetabs", string.Join(",", hideTabs.Select(x => x)));
                            }

                            // Properties
                            content.Properties.Where(p => hideProperties.InvariantContains(p.Alias)).ForEach(p =>
                            {
                                p.Config.Add("hide", true);
                            });

                            // Buttons
                            if (hideButtons.Count() > 0)
                            {
                                firstProperty.Config.Add("hidebuttons", string.Join(",", hideButtons.Select(x => x)));
                            }

                            // Panels
                            if (hidePanels.Count() > 0)
                            {
                                firstProperty.Config.Add("hidepanels", string.Join(",", hidePanels.Select(x => x)));
                            }

                            // Labels
                            content.Properties.Where(p => hideLabels.InvariantContains(p.Alias)).ForEach(p =>
                            {
                                p.HideLabel = true;
                            });

                            // Scripts
                            if (runScripts.Count() > 0)
                            {
                                firstProperty.Config.Add("runscripts", string.Join(",", runScripts.Select(x => x)));
                            }

                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error(typeof(RulesHandler), "Error processing rules.", ex);
                        }

                        return response;
                    });
        }
    }
}