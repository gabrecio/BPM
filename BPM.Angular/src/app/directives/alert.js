'use strict';

angular.module('myapp').directive('alerts', function() {
  return {
    restrict: 'A',
    controller: function($scope, $location, $timeout, loginSvc) {

      $scope.alert = false;

      $scope.$on('alertSvc', function(event, alert) {
        if (alert.type == 'authInterceptor:0') {
          alert = {
            type: 'danger',
            msg: 'Se ha perdido la conexión con el servidor.'
          };
        }
        $scope.addAlert(alert);
      });

      $scope.$on('authInterceptor:403', function(event, alert) {
        if (!$scope.alert) {
          $scope.addAlert(alert);
        }
      });

      $scope.$on('loginSvc:idleWarning', function(event, alert) {
        if (!$scope.alert) {
          $scope.addAlert(alert);
        }
      });

      $scope.$on('authInterceptor:401', function(event, alert) {
        if (alert.code === 'invalid_token') {
          alert.msg = 'Su sesión ha expirado, inicie sesión nuevamente.'
          loginSvc.logout();
        }
        if (!$scope.alert) {
          $scope.addAlert(alert);
        }
      });

      $scope.$on('authInterceptor:500', function(event, alert) {
        $scope.addAlert(alert);
      });

      $scope.$on('authInterceptor:404', function(event, alert) {
        $scope.addAlert(alert);
      });

      $scope.alerts = [];

      $scope.addAlert = function(alert) {
        $scope.alert = true;
        $scope.alerts.push(alert);
        $timeout(function() {
          $scope.alert = false;
          $scope.closeAlert(0);
        }, 8000);
      };

      $scope.closeAlert = function(index) {
        $scope.alerts.splice(index, 1);
      };

      $scope.$on('loginSvc:login', function(index) {
        $scope.alerts = [];
      });
    },
    templateUrl: 'views/alerts.html'
  };
});