angular.module('myapp').controller('LoginCtrl', function($scope, loginSvc, $location) {

  $scope.email = null;
  $scope.password = null;
  $scope.confirm = null;
  $scope.rememberMe = false;
  $scope.signupMode = false;

  $scope.login = function(provider) {

    var credentials = {
      username: $scope.email,
      password: $scope.password
    };

    loginSvc.login(credentials, function(err, user) {
      if (err) {
        console.log('User ' + credentials.username + ' failed to login');
      }
      if (user) {
        console.log('User ' + credentials.username + ' logged in succesfuly');
      }
    });
  };

  $scope.signup = function() {
    // TODO: What about signup?
  };

  $scope.toggleSignup = function() {
    if ($scope.signupMode) {
      $scope.signupMode = false;
    } else {
      $scope.signupMode = true;
    }
  };

  $scope.logout = function() {
    loginSvc.logout();
  };
});