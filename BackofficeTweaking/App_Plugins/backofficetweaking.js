(function () {

    var count = 1,
        event = function (event) {
            $(".nav-tabs > li").show();  // Display any tab in order to fix a bug regarding dashboard tabs
            $(".umb-tab-buttons").show();
            if (event.animationName == 'nodeInserted') {
                var scope = angular.element(event.target).scope();

                if (scope && $(event.target).hasClass('umb-property')) {

                    if (scope.property && scope.property.config) {

                        // Tabs
                        if (scope.property.config.hidetabs) {
                            var tabLabels = scope.property.config.hidetabs.split(",");
                            for (var i = 0; i < tabLabels.length; i++) {
                                //console.log(tabLabels[i]);
                                $(".nav-tabs > li > a:contains('" + tabLabels[i] + "')").addClass('hidden-tab');
                            }
                        }

                        // Properties
                        if (scope.property.config.hide) {
                            $(event.target).hide();
                        }

                        // Buttons
                        if (scope.property.config.hidebuttons) {
                            var buttons = scope.property.config.hidebuttons.split(",");
                            for (var i = 0; i < buttons.length; i++) {
                                switch (buttons[i].toLowerCase()) {
                                    case 'preview':
                                        // Hide preview buttons
                                        var previewButtons = $(".umb-tab-buttons > div > a:has(localize[key='buttons_showPage'])");
                                        previewButtons.each(function () {
                                            $(this).addClass("hidden-button")
                                        });
                                        break;
                                    case 'actions':
                                        // Hide actions button
                                        var actionsButton = $(".umb-panel-header div[class*='umb-btn-toolbar'] a:has(localize[key='general_actions'])");
                                        //console.log(actionsButton);
                                        actionsButton.each(function () {
                                            $(this).addClass("hidden-button")
                                        });
                                        break;
                                    case 'save':
                                        // Hide save buttons
                                        var saveButtons = $(".umb-tab-buttons > div > a:has(localize[key='buttons_saveAndPublish'])");
                                        saveButtons.each(function () {
                                            $(this).addClass("hidden-button")
                                        });
                                        saveButtons = $(".umb-tab-buttons > div > a:has(localize[key='buttons_saveToPublish'])");
                                        saveButtons.each(function () {
                                            $(this).addClass("hidden-button")
                                        });
                                        saveButtons = $(".umb-tab-buttons > div > a:has(localize[key='buttons_save'])");
                                        saveButtons.each(function () {
                                            $(this).addClass("hidden-button")
                                        });
                                        saveButtons = $(".umb-tab-buttons > div > a:has(localize[key='content_unPublish'])");
                                        saveButtons.each(function () {
                                            $(this).addClass("hidden-button")
                                        });
                                        saveButtons = $(".umb-tab-buttons > div > a:has(span[class='caret'])");
                                        saveButtons.each(function () {
                                            $(this).addClass("hidden-button")
                                        });
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        // Panels
                        if (scope.property.config.hidepanels) {
                            var panels = scope.property.config.hidepanels.split(",");
                            for (var i = 0; i < panels.length; i++) {
                                switch (panels[i].toLowerCase()) {
                                    case 'breadcrumb':
                                        // Hide the bread crumbs panel (at the bottom)
                                        var breadcrumbPanels = $("ul[class*='umb-panel-footer-nav']");
                                        breadcrumbPanels.each(function () {
                                            $(this).addClass("hidden-panel")
                                        });
                                        var bodyPanel = $(".umb-panel:has(div[class*='umb-panel-body'])");
                                        bodyPanel.each(function () {
                                            $(this).removeClass("editor-breadcrumb")
                                        });

                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }

                    //$(".controls", $(event.target)).addClass('show-controls');
                    //$(".nav-tabs").addClass("show-tabs");
                }
            }
        }

    document.addEventListener('animationstart', event, false);
    document.addEventListener('MSAnimationStart', event, false);
    document.addEventListener('webkitAnimationStart', event, false);
})();
