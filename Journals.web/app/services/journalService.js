'use strict';
app.factory('journalService', ['$http', 'ngAuthSettings', function ($http, ngAuthSettings) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    
    const getOrders = function () {

        return $http.get(serviceBase + 'api/orders').then(function (results) {
            return results;
        });
    };

    const ordersServiceFactory = {
        getOrders
    };


    return ordersServiceFactory;

}]);