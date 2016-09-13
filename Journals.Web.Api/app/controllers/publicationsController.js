
(function () {
    'use strict';

    app.controller('publicationcontroller', ['$scope', 'publicationService', 'journalService', '$location', 'alertService', 'authService', publicationcontroller]);

    function publicationcontroller($scope, publicationService, journalService, $location, alertService, authService) {



        function activate() {




            $scope.vm = {
                publications: {
                    enableFiltering: true,
                    paginationPageSizes: [10, 50, 75],
                    paginationPageSize: 10,
                    columnDefs: [
                      { name: 'title' },
                      { name: 'description' },
                      { name: 'volume' },
                      { name: 'issue' }
                    ],

                },
                journal: journalService.currentJournal(),
                currentPub : null,
            };

            $scope.vm.journal = journalService.currentJournal();

            if (!$scope.vm.journal) {
                $scope.back();
                return;
            }

            journalService.findPublications($scope.vm.journal.id).then(function (results) {
                $scope.vm.publications.data = results.data;
            }, function (error) {
            });

        }

        $scope.isPublisher = function () {
            return authService.authentication.publisher;
        }


        $scope.addPublication = function () {
            $scope.vm.editing = true;
            $scope.vm.currentJournal = {};
        }

        $scope.savePublication = function () {
            publicationService.addPublication($scope.vm.currentPub, $scope.vm.journal.id).then((publication) => {
                alertService.alert(`Publication ${$scope.vm.currentPub.title} added successfully`);
                $scope.vm.editing = false;
                $scope.vm.currentPub = null;
                $scope.vm.journal.publicationCount++;
                $scope.vm.publications.data.push(publication);
            });
        }

        $scope.cancel = function () {
            $scope.vm.editing = false;
            $scope.vm.currentPub = null;
        }

        $scope.back = function () {
            journalService.currentJournal(null);
            $location.path("/journals");
        }


        activate();


    }
})();
