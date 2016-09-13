(function (angular, bootbox) {
    "use strict";

    app.factory('alertService', ["$q", alertService]);

    function alertService($q) {
        /**
        * @param {string} msg
        * @param {string} applicationName
        * @param {string} applicationId
        */
        const confirm = function (msg) {
            var deferred = $q.defer();
            const alertMessage = msg;

            bootbox.setDefaults({ locale: 'en' });
            bootbox.confirm({
                message: alertMessage,
                title: 'Confirm...',
                callback: function (result) {
                    result ? deferred.resolve() : deferred.reject();
                }
            });

            return deferred.promise;
        };

      

        /**
        * @param {string} msg
        */
        const alert = function (msg, title) {
            var deferred = $q.defer();

            bootbox.setDefaults({ locale: 'en' });
            bootbox.alert({
                templates: {
                    header:
                        "<div class='modal-header'>" +
                            "<i class='fa fa-times-circle'></i>" +
                            "<h4 class='modal-title'></h4>" +
                            "</div>"
                },
                message: msg,
                title: title || 'Success',
                callback: function () {
                    deferred.resolve();
                }
            });
            return deferred.promise;
        };

   

        const service = {
            confirm,
            alert,
          
        };

        return service;
    }
})(angular,bootbox);
