(function () {
    'use strict';

    config.$inject = ['$routeProvider', '$locationProvider'];

    angular.module('flashTrashMob', [
        'ngRoute', 'ui.bootstrap', 'cleanupEventsService'
    ]).config(config);

    function config($routeProvider, $locationProvider) {
        $routeProvider
            .when('/', {
                title: 'Flash Trash Mob',
                templateUrl: '/views/home.html',
                controller: 'homeController'
            })
            .when('/cleanupEvents/all', {
                title: 'Flash Trash Mob - All Cleanup Events',
                templateUrl: '/views/list.html',
                controller: 'listController'
            })
            .when('/cleanupEvents/my', {
                title: 'Flash Trash Mob - My Cleanup Events',
                templateUrl: '/views/my.html',
                controller: 'myController',
                resolve: { isUserAuthenticated: 'authService' }
            })
            .when('/cleanupEvents/add', {
                title: 'Flash Trash Mob - Host Cleanup Event',
                templateUrl: '/views/add.html',
                controller: 'addController',
                resolve: { isUserAuthenticated: 'authService' }
            })
            .when('/cleanupEvents/detail/:id', {
                title: 'Flash Trash Mob - Details',
                templateUrl: '/views/detail.html',
                controller: 'detailController',
                resolve: { isUserAuthenticated: 'authService' }
            })
            .when('/cleanupEvents/edit/:id', {
                title: 'Flash Trash Mob - Edit Cleanup Event',
                templateUrl: '/views/edit.html',
                controller: 'editController',
                resolve: { isUserAuthenticated: 'authService' }
            })
            .when('/cleanupEvents/delete/:id', {
                title: 'Flash Trash Mob - Delete Cleanup Event',
                templateUrl: '/views/delete.html',
                controller: 'deleteController',
                resolve: { isUserAuthenticated: 'authService' }
            })
            .when('/account/login', {
                title: 'Flash Trash Mob - Log In',
                templateUrl: '/account/login',
                controller: 'loginController',
            })
            .when('/account/register', {
                title: 'Flash Trash Mob - Register',
                templateUrl: '/account/register',
                controller: 'registerController'
            })
            .when('/about', {
                title: 'Flash Trash Mob - About',
                templateUrl: '/views/about.html',
            })
            .otherwise({ redirectTo: '/' });

        $locationProvider.html5Mode({
            enabled: true,
            requireBase: false
        });
    }
})();