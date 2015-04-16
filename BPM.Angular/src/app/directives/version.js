'use strict';
// TODO: We already depend on git to build a file, why not build an angular
// constant to save us the http call to get a value that is constant for the
// build?
angular.module('myapp').directive('version', function(REVISION, BUILD) {
  return {
    restrict: 'E',
    controller: function($scope) {
      $scope.revision = REVISION;
      $scope.build = BUILD;
    },
    template: 'Build: <strong>{{build}}</strong> &mdash; Revision: <strong>{{revision}}</strong>'
  };
});