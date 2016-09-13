app.factory('authService', ['$http', '$q', 'localStorageService', 'ngAuthSettings', function ($http, $q, localStorageService, ngAuthSettings) {

    "use strict";

    var serviceBase = ngAuthSettings.apiServiceBaseUri;

    var authentication = {
        isAuth: false,
        userName: "",
        publisher: false,
        useRefreshTokens: false
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
            const isPublisher = response.publisher === "True";

            const storageData = {
                token: response.access_token,
                userName: loginData.userName,
                refreshToken: "",
                useRefreshTokens: false,
                publisher: isPublisher
            };

            localStorageService.set('authorizationData', storageData);

            authentication.publisher = isPublisher;
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
            authentication.publisher = authData.publisher;
            authentication.userName = authData.userName;
            authentication.useRefreshTokens = authData.useRefreshTokens;
        }
    };




    const authServiceFactory = {
        saveRegistration,
        login,
        logOut,
        fillAuthData,
        authentication,
    }


    return authServiceFactory;
}]);