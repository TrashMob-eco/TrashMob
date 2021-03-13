(function () {
    'use strict';

    angular
        .module('flashTrashMob')
        .controller('homeController', homeController)
        .controller('listController', listController)
        .controller('myController', myController)
        .controller('detailController', detailController)
        .controller('addController', addController)
        .controller('editController', editController)
        .controller('deleteController', deleteController);

    /* Home Controller  */
    homeController.$inject = ['$scope', '$location', 'cleanupEvent', 'mapService'];

    function homeController($scope, $location, cleanupEvent, mapService) {
        $scope.cleanupEvent = cleanupEvent.popular.query();

        $scope.selectCleanupEvent = function (cleanupEventId) {
            $location.path("/cleanupEvents/detail/" + cleanupEventId).search({ nocache: new Date().getTime() });
        };

        $scope.loadMap = function () {
            mapService.loadMap($scope.cleanupEvents, 4);
        };

        $scope.showLocationPin = function (cleanupEvent) {
            mapService.showInfoBoxPin(cleanupEvent);
        };

        $scope.hideInfoBoxPin = function (cleanupEvent) {
            mapService.hideInfoBoxPin();
        };

        $scope.showLocation = function (searchText) {
            mapService.findAddress(searchText, false);
        }

        $scope.home = function () {
            $location.path('/');
        };
    }

    /* CleanupEvent List Controller  */
    listController.$inject = ['$scope', '$location', 'cleanupEvent'];

    function listController($scope, $location, cleanupEvent) {
        cleanupEvent.count().then(function (result) {
            $scope.bigTotalItems = result.success;
        });

        $scope.maxSize = 3;
        $scope.bigCurrentPage = 1;
        $scope.itemPerPage = 12;

        $scope.cleanupEvents = cleanupEvent.all.query({ pageIndex: $scope.bigCurrentPage, pageSize: $scope.itemPerPage });

        $scope.selectCleanupEvent = function (cleanupEventId) {
            $location.path('/cleanupEvents/detail/' + cleanupEventId);
        };

        $scope.pageChanged = function () {
            $scope.cleanupEvents = cleanupEvent.all.query({ pageIndex: $scope.bigCurrentPage, pageSize: $scope.itemPerPage });
        };
    }

    /* CleanupEvent My Controller  */
    myController.$inject = ['$scope', '$location', 'cleanupEvent', 'isUserAuthenticated'];

    function myController($scope, $location, cleanupEvent, isUserAuthenticated) {
        if (isUserAuthenticated.success === 'False') {
            $location.path('/account/login');
        }

        $scope.cleanupEvents = cleanupEvent.my.query();

        $scope.selectCleanupEvent = function (cleanupEventId) {
            $location.path('/cleanupEvents/detail/' + cleanupEventId);
        };
    }

    /* CleanupEvents Detail Controller  */
    detailController.$inject = ['$scope', '$routeParams', '$location', 'cleanupEvent', 'mapService', 'isUserAuthenticated'];

    function detailController($scope, $routeParams, $location, cleanupEvent, mapService, isUserAuthenticated) {
        $scope.cleanupEvent = cleanupEvent.all.get({ id: $routeParams.id });

        if (isUserAuthenticated.success === 'False') {
            $scope.isUserAuthenticated = isUserAuthenticated.success;
        }

        cleanupEvent.isUserHost($routeParams.id).then(function (result) {
            $scope.isUserHost = result.success;
        });

        cleanupEvent.isUserRegistered($routeParams.id).then(function (result) {
            $scope.isUserRegistered = result.success;
        });

        $scope.editCleanupEvent = function (cleanupEventId) {
            $location.path('/cleanupEvents/edit/' + cleanupEventId);
        };

        $scope.deleteCleanupEvent = function (cleanupEventId) {
            $location.path('/cleanupEvents/delete/' + cleanupEventId);
        };

        $scope.loadMap = function () {
            mapService.loadMap($scope.cleanupEvent);
        }
    }

    /* CleanupEvents Create Controller */
    addController.$inject = ['$http', '$scope', '$location', 'cleanupEvent', 'mapService', 'isUserAuthenticated'];

    function addController($http, $scope, $location, cleanupEvent, mapService, isUserAuthenticated) {
        $scope.cleanupEvent = {
            title: '',
            description: '',
            eventDate: '',
            address: '',
            contactPhone: ''
        };

        if (isUserAuthenticated.success === 'False') {
            $location.path('/account/login');
        }

        $scope.loadDefaultMap = function () {
            mapService.loadDefaultMap();
        }

        $scope.changeAddress = function (address) {
            mapService.findAddress(address, true);
        }

        $scope.add = function () {
            var result = cleanupEvent.addCleanupEvent($scope.cleanupEvent);
            result.then(function (result) {
                if (result.success) {
                    if (result.data.cleanupEventId) {
                        $location.path('/cleanupEvents/detail/' + result.data.cleanupEventId);
                        $scope.error = false;
                    } else {
                        $scope.error = true;
                    }
                } else {
                    $scope.error = true;
                    $scope.errorMessage = result.error;
                }
            });
        };

        $scope.cancel = function () {
            $location.path('/');
        }
    }
    /* CleanupEvents Edit Controller  */
    editController.$inject = ['$scope', '$routeParams', '$location', 'cleanupEvent', 'mapService', 'isUserAuthenticated'];

    function editController($scope, $routeParams, $location, cleanupEvent, mapService, isUserAuthenticated) {
        if (isUserAuthenticated.success === 'False') {
            $location.path('/account/login');
        }

        $scope.cleanupEvent = cleanupEvent.all.get({ id: $routeParams.id });

        $scope.$watch(eventDate, function (newValue) {
            $scope.cleanupEvent.eventDate = newValue;
        });

        $scope.$watch('cleanupEvent.eventDate', function (newValue) {
            $scope.eventDate = new Date($scope.cleanupEvent.eventDate);
        });

        $scope.loadMap = function () {
            mapService.loadMap($scope.cleanupEvent);
        }

        $scope.changeAddress = function (address) {
            mapService.findAddress(address, true);
        }

        $scope.edit = function () {
            var result = cleanupEvent.editCleanupEvent($routeParams.id, $scope.cleanupEvent);
            result.then(function (result) {
                if (result.success) {
                    $location.path('/cleanupEvents/detail/' + $routeParams.id);
                } else {
                    $scope.error = true;
                    $scope.errorMessage = result.error;
                }
            });
        }

        $scope.cancel = function () {
            $location.path('/cleanupEvents/detail/' + $routeParams.id);
        }
    }

    /* CleanupEvents Delete Controller */
    deleteController.$inject = ['$scope', '$routeParams', '$location', 'cleanupEvent', 'isUserAuthenticated'];

    function deleteController($scope, $routeParams, $location, cleanupEvent, isUserAuthenticated) {
        if (isUserAuthenticated.success === 'False') {
            $location.path('/account/login');
        }

        $scope.cleanupEvent = cleanupEvent.all.get({ id: $routeParams.id });

        $scope.delete = function () {
            var result = cleanupEvent.deleteCleanupEvent($routeParams.id);
            result.then(function (result) {
                if (result.success) {
                    $location.path('/cleanupEvents/my');
                }
            });
        };

        $scope.cancel = function () {
            $location.path('/cleanupEvents/detail/' + $routeParams.id);
        };
    }

})();