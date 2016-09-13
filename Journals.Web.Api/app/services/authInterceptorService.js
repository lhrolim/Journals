'use strict';
app.factory('authInterceptorService', ['$q', '$injector','$location', 'localStorageService', function ($q, $injector,$location, localStorageService) {

    const request = function (config) {

        config.headers = config.headers || {};
        const authData = localStorageService.get('authorizationData');
        if (authData) {
            config.headers.Authorization = 'Bearer ' + authData.token;
        }

        return config;
    };

    const responseError = function (rejection) {
        if (rejection.status === 401) {
            const authService = $injector.get('authService');
            const authData = localStorageService.get('authorizationData');
            if (authData) {
                if (authData.useRefreshTokens) {
                    $location.path('/refresh');
                    return $q.reject(rejection);
                }
            }
            authService.logOut();
            $location.path('/login');
        }
        return $q.reject(rejection);
    };
    const authInterceptorServiceFactory = {
        request,
        responseError
    }

    return authInterceptorServiceFactory;
}]);