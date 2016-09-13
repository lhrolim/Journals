(function () {
    'use strict';


    app.factory('publicationService', ['$http', '$httpParamSerializer', 'ngAuthSettings', 'alertService', publicationService]);

    function publicationService($http, $httpParamSerializer, ngAuthSettings) {

        var serviceBase = ngAuthSettings.apiServiceBaseUri;

        const addPublication = function (publication, journalId) {

            //only one file allowed
            const pubData = publication.data[0];

            //to avoid passing same info twice
            delete publication.data;

            const publicationData = {
                publication,
                journalId,
                base64Data: pubData
            };

            return $http.post(serviceBase + 'api/generic/journal/AddPublication', publicationData).then(function (response) {
                return response.data;
            });

        }
        

        const service = {
            addPublication
        };

        return service;
    }
})();