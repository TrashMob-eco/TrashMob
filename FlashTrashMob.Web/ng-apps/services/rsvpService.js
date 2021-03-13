(function () {
    'use strict';

    angular
        .module('flashTrashMob')
        .service('rsvpService', rsvpService);

    rsvpService.$inject = ['$http', '$q'];

    function rsvpService($http, $q) {
        this.addRsvp = function (cleanupEventId) {
            var deferredObject = $q.defer();
            $http.post(
                '/api/rsvp?cleanupEventId=' + cleanupEventId
            ).
            success(function (data) {
                if (data) {
                    deferredObject.resolve({ success: true });
                } else {
                    deferredObject.resolve({ success: false });
                }
            }).
            error(function (err) {
                deferredObject.resolve({ error: err });
            });

            return deferredObject.promise;
        };

        this.cancelRsvp = function (cleanupEventId) {
            var deferredObject = $q.defer();
            $http.delete(
                '/api/rsvp?cleanupEventId=' + cleanupEventId
            ).
            success(function (data) {
                if (data) {
                    deferredObject.resolve({ success: true });
                } else {
                    deferredObject.resolve({ success: false });
                }
            }).
            error(function (err) {
                deferredObject.resolve({ error: err });
            });

            return deferredObject.promise;
        };
    }
})();