(function () {
    'use strict';

    angular
        .module('flashTrashMob')
        .service('authService', authService);

    authService.$inject = ['$http', '$q'];

    function authService($http, $q) {
        var deferredObject = $q.defer();
        $http.get('/account/isUserAuthenticated').
        success(function (data) {
            if (data) {
                deferredObject.resolve({ success: data });
            }
        }).
        error(function () {
            deferredObject.resolve({ success: false });
        });
        return deferredObject.promise;
    };
})();