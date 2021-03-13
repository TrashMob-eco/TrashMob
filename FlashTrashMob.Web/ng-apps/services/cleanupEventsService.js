(function () {
    'use strict';

    angular
        .module('cleanupEventsService', ['ngResource'])
        .factory('cleanupEvent', cleanupEvent)

    cleanupEvent.$inject = ['$resource', '$http', '$q'];

    function cleanupEvent($resource, $http, $q) {
        return {
            all: $resource('/api/cleanupEvents/:id', { id: "@_id" }),

            popular: $resource('/api/cleanupEvents/popular'),

            my: $resource('/api/cleanupEvents/my'),

            count: function () {
                return cleanupEventHelper($http, $q, '/api/cleanupEvents/count')
            },

            isUserHost: function (cleanupEventId) {
                return cleanupEventHelper($http, $q, '/api/cleanupEvents/isUserHost?id=' + cleanupEventId)
            },

            isUserRegistered: function (cleanupEventId) {
                return cleanupEventHelper($http, $q, '/api/cleanupEvents/isUserRegistered?id=' + cleanupEventId)
            },

            addCleanupEvent: function (cleanupEvent) {
                var deferredObject = $q.defer();
                $http.post(
                    '/api/cleanupEvents', cleanupEvent
                ).
                success(function (data) {
                    if (data) {
                        deferredObject.resolve({ success: true, data: data });
                    } else {
                        deferredObject.resolve({ success: false });
                    }
                }).
                error(function (err) {
                    deferredObject.resolve({ error: err });
                });

                return deferredObject.promise;
            },

            editCleanupEvent: function (cleanupEventId, cleanupEvent) {
                var deferredObject = $q.defer();
                $http.put(
                    '/api/cleanupEvents/' + cleanupEventId, cleanupEvent
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
            },

            deleteCleanupEvent: function (cleanupEventId) {
                var deferredObject = $q.defer();
                $http.delete(
                    '/api/cleanupEvents/' + cleanupEventId
                ).
                success(function (data) {
                    deferredObject.resolve({ success: true });
                }).
                error(function () {
                    deferredObject.resolve({ success: false });
                });

                return deferredObject.promise;
            },
        };
    }

    function cleanupEventHelper($http, $q, url) {
        var deferredObject = $q.defer();
        $http.get(url).
        success(function (data) {
            if (data) {
                deferredObject.resolve({ success: data });
            }
        }).
        error(function () {
            deferredObject.resolve({ success: false });
        });

        return deferredObject.promise;
    }
})();