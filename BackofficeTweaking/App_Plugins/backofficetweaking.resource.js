angular.module('umbraco.resources').factory('BackofficeTweakingResource', function ($q, $http, $log, umbRequestHelper, angularHelper) {
    return {

        getRules: function () {
            return $http.get("backoffice/BackofficeTweaking/BackofficeTweakingApi/GetRules", {
                params: {}
            });
        },

        saveRules: function (configContent) {
            var data = JSON.stringify(angular.toJson(configContent));
            return $http.post("backoffice/BackofficeTweaking/BackofficeTweakingApi/SaveRules", data);
        }

    };
})

