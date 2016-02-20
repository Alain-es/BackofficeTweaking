(function () {

    //Main controller
    function BackofficeTweakingDashboardController($rootScope, $scope, $timeout, assetsService, BackofficeTweakingResource, notificationsService) {

        $scope.columns = {
            "columns": [
                {
                    "title": "Rule type", "alias": "Type", "type": "dropdown", "props":
                      {
                          "options": [
                            { "text": "Hide Tabs", "value": "HideTabs" },
                            { "text": "Hide Properties", "value": "HideProperties" },
                            { "text": "Hide Buttons", "value": "HideButtons" },
                            { "text": "Hide Panels", "value": "HidePanels" },
                            { "text": "Hide Labels", "value": "HideLabels" },
                            { "text": "Run Scripts", "value": "RunScripts" }
                          ]
                      }
                },
                { "title": "Enabled", "alias": "Enabled", "type": "checkbox", "props": {} },
                { "title": "Names", "alias": "Names", "type": "textarea", "props": {} },
                { "title": "Users", "alias": "Users", "type": "textarea", "props": {} },
                { "title": "User Types", "alias": "UserTypes", "type": "textarea", "props": {} },
                { "title": "Content Ids", "alias": "ContentIds", "type": "textarea", "props": {} },
                { "title": "Parent content Ids", "alias": "ParentContentIds", "type": "textarea", "props": {} },
                { "title": "Content Types", "alias": "ContentTypes", "type": "textarea", "props": {} },
                { "title": "Description", "alias": "Description", "type": "textarea", "props": {} }
            ]
        };
        $scope.value = [];
        $scope.actionInProgress = false;
        var propertiesEditorswatchers = [];
        var rowObject = {};

        var resetProertiesEditors = function () {
            rowObject = {};
            $scope.propertiesOrder = [];

            // clean watchers before set again.
            for (var index = 0; index < propertiesEditorswatchers.length; ++index) {
                propertiesEditorswatchers[index]();
            }

            angular.forEach($scope.columns.columns, function (value, key) {
                // Default values
                switch (value.alias) {
                    case "Enabled":
                        rowObject[value.alias] = "true";
                        break;
                    default:
                        rowObject[value.alias] = "";
                        break;
                }
                $scope.propertiesOrder.push(value.alias);
                var columnKey = key;
                var editorProperyAlias = value.alias;

            });
        }

        // Instantiate the grid with empty properties editors
        resetProertiesEditors();

        // Load the css file with the grid's styles
        assetsService.loadCss("/App_Plugins/BackofficeTweaking/Dashboard/backofficetweaking.dashboard.css");

        // Load rules
        BackofficeTweakingResource.getRules().then(
            function (response) {
                if (response.data) {
                    var Rules = JSON.parse(JSON.parse(response.data)).Rules;
                    if (Rules) {
                        if (Rules.Rule) {
                            Rules.Rule = jQuery.makeArray(Rules.Rule);
                        }
                        $scope.value = Rules.Rule;
                    }
                    // Check for deleted columns
                    angular.forEach($scope.value, function (row, key) {
                        angular.forEach(row, function (value, alias) {
                            if ($scope.propertiesOrder.indexOf(alias) == -1) {
                                delete row[alias];
                            }
                        });
                    });
                }
            },
            function (error) {
                var errorMessage = "";
                if (error.data && error.data.Message) {
                    errorMessage = error.data.Message;
                }
                notificationsService.error("Error retrieving rules from the config file", errorMessage);
                console.log(errorMessage);
            }
        );

        // New rule
        $scope.addRow = function () {
            $scope.value.push(angular.copy(rowObject));
            var newrowIndex = $scope.value.length - 1;
            var newRow = $scope.value[newrowIndex];

            angular.forEach($scope.columns.columns, function (value, key) {
                var columnKey = key;
                var editorProperyAlias = value.alias;
            });
        }

        // Remove rule
        $scope.removeRow = function (index) {
            $scope.value.splice(index, 1);
        }

        // Sort grid
        $scope.sortableOptions = {
            axis: 'y',
            cursor: "move",
            handle: ".sortHandle",
            start: function (event, ui) {
                var curTH = ui.helper.closest("table").find("thead").find("tr");
                var itemTds = ui.item.children("td");
                curTH.find("th").each(function (ind, obj) {
                    itemTds.eq(ind).width($(obj).width());
                });
            },
            update: function (ev, ui) {

                $timeout(function () {
                    $scope.rtEditors = [];
                    angular.forEach($scope.columns.columns, function (value, key) {
                        var columnKey = key;
                        var editorProperyAlias = value.alias;
                    });
                }, 0);

            }
        };

        // Save rules
        $scope.save = function () {
            $scope.actionInProgress = true;
            BackofficeTweakingResource.saveRules({ "Rule": $scope.value }).then(
                    function (result) {
                        if (result.data && result.data != "" && result.data != '""') {
                            notificationsService.error("Error saving rules", result.data);
                        }
                        else {
                            notificationsService.success("Rules saved successfully", "");
                        }
                        $scope.actionInProgress = false;
                    },
                    function (error) {
                        var errorMessage = "";
                        if (error.data && error.data.Message) {
                            errorMessage = error.data.Message;
                        }
                        notificationsService.error("Error saving rules", errorMessage);
                        $scope.actionInProgress = false;
                    }
                );
            //};
        };

    };

    // Register the controller
    angular.module("umbraco").controller('BackofficeTweaking.DashboardController', BackofficeTweakingDashboardController);

})();



