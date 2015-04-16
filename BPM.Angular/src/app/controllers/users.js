angular.module('myapp').controller('UsersCtrl', function($scope, $routeParams, apiSvc, alertSvc, $timeout) {
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
    $scope.users = null;
    if ($scope.keyTimer != null) {
      clearTimeout($scope.keyTimer);
    }
    $scope.keyTimer = setTimeout(function() {
      $scope.pagingInfo.search = $scope.searchUsers ? $scope.searchUsers : '';
      var usuarios = null;
     /* usuarios = apiSvc.findUsers.findUsers({
        pagingInfo: JSON.stringify($scope.pagingInfo)
      }, function() {
        $scope.users = usuarios.users;
        $scope.pagingInfo.totalItems = usuarios.totalUsers;
      })*/
      usuarios = apiSvc.findUsers.findUsers($scope.pagingInfo, function () {
          $scope.users = usuarios.users;
          $scope.pagingInfo.totalItems = usuarios.totalUsers;
      })

    }, 200);

  };

  $scope.doSearch();



  $scope.deleteUser = function(item) {

    apiSvc.User['delete']({
        id: item.id
      },
      function(result) {
        if (result.status == 200) {
          $scope.users.splice($scope.users.indexOf(item), 1);
          $scope.pagingInfo.totalItems--;
          alertSvc.send('success', result);
        }
      }, function(error) {
        //Error ahndler code
      });
  };
});