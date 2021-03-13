
(function () {
    'use strict';

    angular
        .module('flashTrashMob')
        .directive('rsvpSection', rsvpSection)

    /* RSVP Controller  */
    rsvpController.$inject = ['$scope', '$routeParams', '$location', 'rsvpService'];

    function rsvpController($scope, $routeParams, $location, rsvpService) {
        $scope.showMessage = false;
        $scope.addRsvp = function (cleanupEventId) {
            if ($scope.isUserAuthenticated === 'False') {
                $location.path('/account/login');
            }

            var result = rsvpService.addRsvp(cleanupEventId);
            result.then(function (result) {
                if (result) {
                    $scope.message = 'Thanks - we\'ll see you there!';
                    $scope.showMessage = true;
                    $scope.isUserRegistered = true;
                } else {
                    $scope.showMessage = false;
                }
            });
        }

        $scope.cancelRsvp = function (cleanupEventId) {
            if ($scope.isUserAuthenticated === 'False') {
                $location.path('/account/login');
            }

            var result = rsvpService.cancelRsvp(cleanupEventId);
            result.then(function (result) {
                if (result) {
                    $scope.message = 'Sorry you can\'t make it!';
                    $scope.showMessage = true;
                    $scope.isUserRegistered = false;
                } else {
                    $scope.showMessage = false;
                }
            });
        }
    }

    function rsvpSection() {
        return {
            restrict: 'E',
            templateUrl: "/views/rsvp.html",
            controller: rsvpController
        }
    }
})();