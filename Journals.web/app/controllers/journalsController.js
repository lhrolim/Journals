'use strict';
app.controller('ordersController', ['$scope', 'ordersService', function ($scope, ordersService) {

    $scope.journals = [];

    ordersService.getOrders().then(function (results) {

        $scope.orders = results.data;

    }, function (error) {
        //alert(error.data.message);
    });

}]);