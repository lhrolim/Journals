(function (app) {
    "use strict";

    app.directive("fileread", ["$log",  function ($log) {

        return {
            scope: {
                fileread: "=",
                path: "=",
                field: "=fileReadField"
            },

            link: function (scope, element, attrs) {
                scope.jselement = element;

                var readFiles = function (changeEvent, fileRead, reader, current) {
                    var fileNew = changeEvent.target.files[current]; // get the first from queue and store in file

                    current++;
                    if (current === changeEvent.target.files.length + 1) {
                        scope.fileread = fileRead;
                        return;
                    }
                    reader.onloadend = function (loadEvent) { // when finished reading file, call recursive readFiles function
                        scope.$apply(function () {
                            fileRead.push(loadEvent.target.result);
                            readFiles(changeEvent, fileRead, reader, current);
                        });
                    };
                    reader.readAsDataURL(fileNew);
                };

                element.bind("change", function (changeEvent) {
                    $log.debug('file change detected');

                    const fileName = [];
                    const reader = new FileReader();

                    //Getting the File extension.
                    for (let i = 0; i < changeEvent.target.files.length; i++) {
                        const temp = changeEvent.target.files[i].name.split(".").pop().toLowerCase();
                    }
                    var file;
                    const fileRead = [];
                    readFiles(changeEvent, fileRead, reader, 0);
                    for (let j = 0; j < changeEvent.target.files.length; j++) {
                        file = changeEvent.target.files[j];
                        fileName.push(file.name);
                    }



                });;
            }
        };
    }]);

})(app);
