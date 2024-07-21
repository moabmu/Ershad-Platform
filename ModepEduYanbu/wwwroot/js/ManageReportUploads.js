var UploadedFile = (function () {
    function UploadedFile(filename, url) {
        this.filename = filename;
        this.url = url;
    }
    return UploadedFile;
}());

var uploadApp = angular.module('uploadApp', []);
uploadApp.directive('fileModel', ['$parse', function ($parse) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var model = $parse(attrs.fileModel);
            var modelSetter = model.assign;

            element.bind('change', function () {
                scope.$apply(function () {
                    modelSetter(scope, element[0].files[0]);
                });
            });
        }
    };
}]);

uploadApp.service('uploadedFileService', ['$http', function ($http) {
    this.createUploadedFile = function (uploadedFile) {
        var fd = new FormData();
        fd.append('filename', uploadedFile.filename);
        fd.append('url', uploadedFile.url);

        return $http.post('/report/upload', fd, {
            transformRequest: angular.identity,
            headers: {
                'Content-Type': undefined
            }
        });
    };
}]);

uploadApp.controller('uploadController', ['$scope', 'uploadedFileService', function ($scope, uploadedFileService) {
    $scope.createUploadedFile = function () {
        $scope.showUploadStatus = false;
        $scope.showUploadedData = false;

        var uploadedFile = new UploadedFile($scope.filename, $scope.url);

        uploadedFileService.createUploadedFile(uploadedFile).then(function (response) { // success
            if (response.status == 200) {
                $scope.uploadStatus = "تم رفع الملف بنجاح.";
                $scope.uploadedData = response.data;
                $scope.showUploadStatus = true;
                $scope.showUploadedData = true;
                $scope.errors = [];
            }
        },
            function (response) { // failure
                $scope.uploadStatus = "فشل رفع الملف مع رمز الحالة: " + response.status;
                $scope.showUploadStatus = true;
                $scope.showUploadedData = false;
                $scope.errors = [];
                $scope.errors = parseErrors(response);
            });
    };
}]);

function parseErrors(response) {
    var errors = [];
    for (var key in response.data) {
        for (var i = 0; i < response.data[key].length; i++) {
            errors.push(key + ': ' + response.data[key][i]);
        }
    }
    return errors;
}