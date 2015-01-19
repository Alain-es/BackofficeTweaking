(function () {

    var count = 1,
        event = function (event) {
            $(".nav-tabs > li").show();  // Display any tab in order to fixe a bug regarding dashboard tabs
            $(".umb-tab-buttons > div > a").show();
            if (event.animationName == 'nodeInserted') {
                var scope = angular.element(event.target).scope();

                if (scope && $(event.target).hasClass('umb-property')) {
                    if (scope.property.config && scope.property.config.hidetabs) {
                        var tabLabels = scope.property.config.hidetabs.split(",");

                        for (var i = 0; i < tabLabels.length; i++) {
                            $(".nav-tabs > li > a:contains('" + tabLabels[i] + "')").addClass('hidden-tab');
                        }
                    }

                    if (scope.property.config && scope.property.config.hide) {
                        $(event.target).hide();
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
