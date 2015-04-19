angular.module('myapp').controller('usersNewCtrl', function($scope, $routeParams, apiSvc, alertSvc, $location, $resource, $parse) {
  $scope.User = {
    Roles: [{
      nombre: '',
      id: 0,
      activo: true
    }]
  };
  $scope.Estados = [{
    name: 'Activo',
    value: 'true'
  }, {
    name: 'Inactivo',
    value: 'false'
  }];
  $scope.UserId = "0";

  init();

  function init() {
    var roles = apiSvc.roles.query(function() {

      $scope.profiles = roles;


      $scope.UserId = ($routeParams.userId) ? $routeParams.userId : "0";
      if ($scope.UserId != "0") {
        apiSvc.User.getUserById({
            id: $scope.UserId
          },
          function(userToEdit) {
            if (userToEdit) {
              $scope.User.mail = userToEdit.mail;
              $scope.User.nombre = userToEdit.nombre;
              $scope.User.apellido = userToEdit.apellido;
              $scope.User.password = "********";
              $scope.User.profile = userToEdit.roles[0];
              $scope.User.Activo = (userToEdit.activo) ? $scope.Estados[0] : $scope.Estados[1];
            }
          }
        );
      } else {
        $scope.User.profile = $scope.profiles[1];
        $scope.User.Activo = $scope.Estados[0];
      }
    });


    $scope.shouldBeOpen = true;
  };


  $scope.saveUser = function() {
    $scope.User.Roles[0].nombre = $scope.User.profile.nombre;
    $scope.User.Roles[0].id = $scope.User.profile.id;

    $scope.User.Activo = ($scope.User.Activo.value == 'true') ? true : false;
    if ($scope.UserId == "0") {

      apiSvc.users.save($scope.User,
        function(result) {
          alertSvc.send('success', 'El usuario se creo correctamente.');
        });
    } else {
      $scope.User.password = ($scope.User.password === "********") ? "" : $scope.User.password;
      $scope.User.Id = $scope.UserId;

      apiSvc.User.update({
        id: $scope.UserId
      }, $scope.User, function(result) {
        if (result.status == 200) {
          alertSvc.send('success', result);
        }
      });
    }
    $location.path('/admin/users');
  };

  $scope.cancel = function() {

    $location.path('/admin/users');
  };

  $scope.usernameChaged = function() {
      if ($scope.User.mail) {
     
          var userNameValidation = apiSvc.usernameValidation.usernameValidation({'username':$scope.User.mail},
      function() {
        var serverMessage2 = $parse('user.mail.$error.userExists');
        switch (userNameValidation.status) {
          case 200:
            $scope.user.$setValidity('mail', true, $scope.user);
            serverMessage2.assign($scope, undefined);
            break;
          default:
            $scope.user.$setValidity('mail', false, $scope.user);
            serverMessage2.assign($scope, true);
        }

      });
    }

  };

});