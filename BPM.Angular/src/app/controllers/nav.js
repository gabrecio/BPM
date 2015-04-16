'use strict';
angular.module('myapp').controller('NavCtrl', function($rootScope, $scope, loginSvc, localStorageService, $location) {
  if (loginSvc.isAuthenticated()) {
    //$scope.recentIssues = localStorageService.get('recentIssues');

    $scope.username = localStorageService.get('username');
  }
  $rootScope.$on('loginSvc:login', function() {
    //$scope.recentIssues = localStorageService.get('recentIssues');
    $scope.username = localStorageService.get('username');
  });
  $rootScope.$on('loginSvc:logout', function() {
    //$scope.recentIssues = undefined;
    $scope.username = undefined;
  });

  $rootScope.$on('recentIssues:updated', function(e) {
    //$scope.recentIssues = localStorageService.get('recentIssues');
  });

  $scope.active = function(path) {
    return path === $location.path();
  };
});