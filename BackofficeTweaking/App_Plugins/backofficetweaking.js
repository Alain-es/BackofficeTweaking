(function () {

    var count = 1,
        event = function (event) {
            $(".nav-tabs > li").show();  // Display any tab in order to fix a bug regarding dashboard tabs
            $(".umb-tab-buttons > div > a").css("display", "inline-block");
            if (event.animationName == 'nodeInserted') {
                var scope = angular.element(event.target).scope();

                if (scope && $(event.target).hasClass('umb-property')) {

                    // Tabs
                    if (scope.property && scope.property.config && scope.property.config.hidetabs) {
                        var tabLabels = scope.property.config.hidetabs.split(",");

                        for (var i = 0; i < tabLabels.length; i++) {
                            $(".nav-tabs > li > a:contains('" + tabLabels[i] + "')").addClass('hidden-tab');
                        }
                    }

                    // Properties
                    if (scope.property && scope.property.config && scope.property.config.hide) {
                        $(event.target).hide();
                    }

                    // Buttons
                    if (scope.property && scope.property.config && scope.property.config.hidebuttons) {
                        var buttons = scope.property.config.hidebuttons.split(",");
                        for (var i = 0; i < buttons.length; i++) {
                            switch (buttons[i]) {
                                case 'preview':
                                    // Hide preview buttons
                                    var previewButtons = $(".umb-tab-buttons > div > a:has(localize[key='buttons_showPage'])");
                                    previewButtons.each(function () {
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
                                case 'previews':
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    $(".controls", $(event.target)).addClass('show-controls');
                    $(".nav-tabs").addClass("show-tabs");
                }
            }
        }

    document.addEventListener('animationstart', event, false);
    document.addEventListener('MSAnimationStart', event, false);
    document.addEventListener('webkitAnimationStart', event, false);
})();
