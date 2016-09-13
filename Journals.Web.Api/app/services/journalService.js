'use strict';
app.factory('journalService', ['$http', '$httpParamSerializer', 'ngAuthSettings', 'alertService', function ($http, $httpParamSerializer, ngAuthSettings, alertService) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;

    var currentJournalData = null;

    const currentJournal = function(journal) {
        if (journal!==undefined) {
            currentJournalData = journal;
        }
        return currentJournalData;
    }

    const listJournals = function (pageNumber, pageSize) {
        let querySt = serviceBase + 'api/generic/journal/listJournals?';

        const parameters = {
            //TODO: pagination on server side
            pageSize: pageSize || 10000,
            pageNumber: pageNumber || 1
        }

        querySt += $httpParamSerializer(parameters);


        return $http.get(querySt).then(function (results) {
            return results;
        });
    };

    const addJournal = function(journalData) {
        return $http.post(serviceBase + 'api/generic/journal/CreateJournal', journalData).then(response  => response.data);
    }

    const changeSubscriptionStatus = function (journal) {
        const operation = !journal.isSubscribed ? "Unsubscribe" : "Subscribe";

        alertService.confirm(`are you sure you want to ${operation} for journal ${journal.name}`)
            .then(() => {
                let querySt = serviceBase + `api/generic/journal/${operation}?journalid=${journal.id}`;

                return $http.post(querySt).then(function (results) {
                    return alertService.alert(`Successfully ${operation}d to journal ${journal.name}`);
                });

            }).catch(error => {
                journal.isSubscribed = !journal.isSubscribed;
            });
    }


    const findPublications = function (journalid, pageNumber,pageSize) {
        let querySt = serviceBase + "api/generic/journal/FindPublicationsOfJournal?";

        const parameters = {
            //TODO: pagination on server side
            pageSize: pageSize || 100000,
            pageNumber: pageNumber || 1,
            journalid: journalid
        }

        querySt += $httpParamSerializer(parameters);


        return $http.get(querySt).then(function (results) {
            return results;
        });
    }

    const ordersServiceFactory = {
        listJournals,
        changeSubscriptionStatus,
        addJournal,
        findPublications,
        currentJournal
    };


    return ordersServiceFactory;

}]);



