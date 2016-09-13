'use strict';
app.controller('journalscontroller', ['$scope', '$location', 'journalService', 'alertService', "authService", function ($scope, $location, journalService, alertService, authService) {

    
    $scope.vm = {};

    $scope.vm.journals = {
        paginationPageSizes: [10, 50, 75],
        paginationPageSize: 10,
        enableFiltering: true,
        rowTemplate: '<div ng-click="grid.appScope.loadPublications(row)" ng-repeat="(colRenderIndex, col) in colContainer.renderedColumns track by col.uid" class="ui-grid-cell" ng-class="col.colIndex()" ui-grid-cell></div>',
        columnDefs: [
          { name: 'name' },
          { name: 'description' },
          { name: 'publicationCount', displayName: 'Publications' },
          { name: 'isSubscribed', displayName: 'Subscribed', type: 'boolean', cellTemplate: '<input type="checkbox" data-ng-model="row.entity.isSubscribed" ng-click="grid.appScope.changeSubscriptionStatus(row.entity,$event)">' }
        ]
    };

    journalService.listJournals().then(function (results) {

        $scope.vm.journals.data = results.data;
        




    }, function (error) {
        //alert(error.data.message);
    });

    $scope.addJournal = function() {
        $scope.vm.editing = true;
        $scope.vm.currentJournal = {};
    }

    $scope.saveJournal = function () {
        journalService.addJournal($scope.vm.currentJournal).then((journal) => {
            alertService.alert(`jornal ${$scope.vm.currentJournal.name} created successfully`);
            $scope.vm.editing = false;
            $scope.vm.currentJournal = null;
            journal.publicationCount = 0;
            $scope.vm.journals.data.push(journal);
        });
    }

    $scope.isPublisher = function() {
        return authService.authentication.publisher;
    }

    $scope.cancel = function () {
        $scope.vm.editing = false;
        $scope.vm.currentJournal = null;
    }

    $scope.changeSubscriptionStatus = function (journal,$event) {
        journalService.changeSubscriptionStatus(journal);
        $event.stopImmediatePropagation();
    }

    $scope.loadPublications = function (journal) {
        journalService.currentJournal(journal.entity);
        $location.path('/publications');
    }


}]);