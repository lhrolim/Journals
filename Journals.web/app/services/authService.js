﻿app.factory('authService', ['$http', '$q', 'localStorageService', 'ngAuthSettings', function ($http, $q, localStorageService, ngAuthSettings) {

    "use strict";

    var serviceBase = ngAuthSettings.apiServiceBaseUri;

    var authentication = {
        isAuth: false,
        userName: "",
        useRefreshTokens: false
    };

    const externalAuthData = {
        provider: "",
        userName: "",
        externalAccessToken: ""
    };

    const logOut = function () {

        localStorageService.remove('authorizationData');

        authentication.isAuth = false;
        authentication.userName = "";
        authentication.useRefreshTokens = false;

    };

    const saveRegistration = function (registration) {

        logOut();

        return $http.post(serviceBase + 'api/account/register', registration).then(function (response) {
            return response;
        });

    };
    const login = function (loginData) {

        var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password;

        if (loginData.useRefreshTokens) {
            data = data + "&client_id=" + ngAuthSettings.clientId;
        }

        var deferred = $q.defer();

        $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {

            if (loginData.useRefreshTokens) {
                localStorageService.set('authorizationData', { token: response.access_token, userName: loginData.userName, refreshToken: response.refresh_token, useRefreshTokens: true });
            }
            else {
                localStorageService.set('authorizationData', { token: response.access_token, userName: loginData.userName, refreshToken: "", useRefreshTokens: false });
            }
            authentication.isAuth = true;
            authentication.userName = loginData.userName;
            authentication.useRefreshTokens = loginData.useRefreshTokens;

            deferred.resolve(response);

        }).error(function (err, status) {
            logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };


    const fillAuthData = function () {
        const authData = localStorageService.get('authorizationData');
        if (authData) {
            authentication.isAuth = true;
            authentication.userName = authData.userName;
            authentication.useRefreshTokens = authData.useRefreshTokens;
        }
    };

    const refreshToken = function () {
        var deferred = $q.defer();
        const authData = localStorageService.get('authorizationData');
        if (authData) {

            if (authData.useRefreshTokens) {
                const data = "grant_type=refresh_token&refresh_token=" + authData.refreshToken + "&client_id=" + ngAuthSettings.clientId;
                localStorageService.remove('authorizationData');

                $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {

                    localStorageService.set('authorizationData', { token: response.access_token, userName: response.userName, refreshToken: response.refresh_token, useRefreshTokens: true });

                    deferred.resolve(response);

                }).error(function (err, status) {
                    _logOut();
                    deferred.reject(err);
                });
            }
        }

        return deferred.promise;
    };
    const obtainAccessToken = function (externalData) {

        var deferred = $q.defer();

        $http.get(serviceBase + 'api/account/ObtainLocalAccessToken', { params: { provider: externalData.provider, externalAccessToken: externalData.externalAccessToken } }).success(function (response) {

            localStorageService.set('authorizationData', { token: response.access_token, userName: response.userName, refreshToken: "", useRefreshTokens: false });

            authentication.isAuth = true;
            authentication.userName = response.userName;
            authentication.useRefreshTokens = false;

            deferred.resolve(response);

        }).error(function (err, status) {
            logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };
    const registerExternal = function (registerExternalData) {

        var deferred = $q.defer();

        $http.post(serviceBase + 'api/account/registerexternal', registerExternalData).success(function (response) {

            localStorageService.set('authorizationData', { token: response.access_token, userName: response.userName, refreshToken: "", useRefreshTokens: false });

            authentication.isAuth = true;
            authentication.userName = response.userName;
            authentication.useRefreshTokens = false;

            deferred.resolve(response);

        }).error(function (err, status) {
            logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };

    const authServiceFactory = {
        saveRegistration,
        login,
        logOut,
        fillAuthData,
        authentication,
        refreshToken,
        obtainAccessToken,
        externalAuthData,
        registerExternal
    }


    return authServiceFactory;
}]);