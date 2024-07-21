/// <reference path="models.js" />

var User = (function () {
    function User(reportNo, fileToUpload) {
        this.reportNo = reportNo;
        this.fileToUpload = fileToUpload;
    }
    return User;
}());

var myApp = angular.module('myApp', []);
myApp.directive('fileModel', ['$parse', function ($parse) {
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

myApp.service('userService', ['$http', function ($http) {
    this.createUser = function (user) {
        var fd = new FormData();
        fd.append('reportNo', user.reportNo);
        fd.append('fileToUpload', user.fileToUpload);

        return $http.post('/report/upload', fd, {
            transformRequest: angular.identity,
            headers: {
                'Content-Type': undefined
            }
        });
    };
}]);

myApp.controller('myCtrl', ['$scope', 'userService', function ($scope, userService) {
    $scope.createUser = function () {
        $scope.showUploadStatus = false;
        $scope.showUploadedData = false;

        var user = new User($scope.reportNo, $scope.fileToUpload);

        userService.createUser(user).then(function (response) { // success
            if (response.status == 200) {
                $scope.uploadStatus = "User created sucessfully.";
                $scope.uploadedData = response.data;
                $scope.showUploadStatus = true;
                $scope.showUploadedData = true;
                $scope.errors = [];
            }
        },
            function (response) { // failure
                $scope.uploadStatus = "User creation failed with status code: " + response.status;
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