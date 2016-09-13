
var app = angular.module('Journals', ['ngRoute', 'LocalStorageModule', 'angular-loading-bar', 'ui.grid', 'ui.grid.pagination']);

app.config(function ($routeProvider) {

    $routeProvider.when("/home", {
        controller: "homeController",
        templateUrl: "/app/views/home.html"
    });

    $routeProvider.when("/login", {
        controller: "loginController",
        templateUrl: "/app/views/login.html"
    });

    $routeProvider.when("/signup", {
        controller: "signupController",
        templateUrl: "/app/views/signup.html"
    });

    $routeProvider.when("/journals", {
        controller: "journalscontroller",
        templateUrl: "/app/views/journals.html"
    });

    $routeProvider.when("/publications", {
        controller: "publicationcontroller",
        templateUrl: "/app/views/publication.html",
    });

    $routeProvider.when("/refresh", {
        controller: "refreshController",
        templateUrl: "/app/views/refresh.html"
    });

    $routeProvider.when("/tokens", {
        controller: "tokensManagerController",
        templateUrl: "/app/views/tokens.html"
    });

    $routeProvider.otherwise({ redirectTo: "/home" });

});

const serviceBase = 'http://localhost:8089/';

app.constant('ngAuthSettings', {
    apiServiceBaseUri: serviceBase,
    clientId: 'journalsapp'
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

app.run(['authService', function (authService) {
    authService.fillAuthData();
}]);


