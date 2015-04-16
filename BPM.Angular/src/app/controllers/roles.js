angular.module('myapp').controller('RolesCtrl', function($scope, $routeParams, apiSvc, alertSvc, $timeout) {
  'use strict';
  $scope.pagingInfo = {
    page: 1,
    itemsPerPage: 10,
    sortBy: 'nombre',
    reverse: false,
    search: '',
    totalItems: 0
  };

  $scope.sort = function(sortBy) {
    if (sortBy === $scope.pagingInfo.sortBy) {
      $scope.pagingInfo.reverse = !$scope.pagingInfo.reverse;
    } else {
      $scope.pagingInfo.sortBy = sortBy;
      $scope.pagingInfo.reverse = false;
    }
    $scope.pagingInfo.page = 1;
    $scope.doSearch();
  };

  $scope.selectPage = function(page) {
    $scope.pagingInfo.page = page;
    $scope.doSearch();
  };

  $scope.keyTimer = null;
  $scope.doSearch = function() {
    $scope.roles = null;
    if ($scope.keyTimer != null) {
      clearTimeout($scope.keyTimer);
    }
    $scope.keyTimer = setTimeout(function() {
      $scope.pagingInfo.search = $scope.searchRoles ? $scope.searchRoles : '';
      var roles = null;
      roles = apiSvc.findRoles.findRoles({
        pagingInfo: JSON.stringify($scope.pagingInfo)
      }, function() {
        $scope.roles = roles.rols;
        $scope.pagingInfo.totalItems = roles.totalRoles;
      })

    }, 200);

  };

  $scope.doSearch();



  $scope.deleteRol = function(item) {

    apiSvc.Rol['delete']({
        id: item.id
      },
      function(result) {
        if (result.status == 200) {
          $scope.roles.splice($scope.roles.indexOf(item), 1);
          $scope.pagingInfo.totalItems--;
          alertSvc.send('success', result);
        } else {
          alertSvc.send('danger', result);
        }
      }, function(error) {

        //Error ahndler code
      });
  };
});