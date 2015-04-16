angular.module('myapp').controller('rolesNewCtrl', function($scope, $routeParams, apiSvc, alertSvc, $log, $location, $resource, $parse) {
  $scope.Role = {
    permissions: []
  };

  $scope.RolId = "0";

  $scope.Estados = [{
    name: 'Activo',
    value: 'true'
  }, {
    name: 'Inactivo',
    value: 'false'
  }];
  $scope.rolPermissions = [];

  init();

  function init() {

    $scope.RolId = ($routeParams.rolId) ? $routeParams.rolId : "0";
    var rolPerm = apiSvc.Rol.getRolPermissions({
        id: $scope.RolId
      },
      function(perm) {
        if (perm) {
          //console.log(perm);
          perm.forEach(function(element, index, array) {
            var newItem = {
              icon: (element.imagen) ? elem.imagen : '',
              name: '<strong>' + element.menu + '</strong>',
              multiSelectGroup: true
            }
            $scope.rolPermissions.push(newItem);
            //console.log('menu:' + element.menu);
            element.operaciones.forEach(function(elem, index2, array2) {
              var newNode = {
                icon: (elem.imagen) ? elem.imagen : '',
                name: elem.operacion + '-' + element.menu,
                listaPermisoId: elem.listaPermisoId,
                maker: '',
                selected: elem.activo
              }
              $scope.rolPermissions.push(newNode);
            });
            $scope.rolPermissions.push({
              multiSelectGroup: false
            });
          });

        }
      });
    if ($scope.RolId != "0") {
      apiSvc.Rol.getRolById({
          id: $scope.RolId
        },
        function(rolToEdit) {
          if (rolToEdit) {
            $scope.Role.Activo = (rolToEdit.activo) ? $scope.Estados[0] : $scope.Estados[1];
            $scope.Role.Nombre = rolToEdit.nombre;
          }
        });
    } else {
      $scope.Role.Activo = $scope.Estados[0];
    }

    $scope.shouldBeOpen = true;
  };


  $scope.save = function() {

    angular.forEach($scope.rolPermissions, function(value, key) {
      var newPermission = {
        Operacion: value.name,
        Activo: value.selected,
        ListaPermisoId: value.listaPermisoId
      }
      $scope.Role.permissions.push(newPermission);
    });

    $scope.Role.Activo = ($scope.Role.Activo.value == 'true') ? true : false;
    //$scope.Role.permissions = $scope.rolPermissions;
    if ($scope.RolId == "0") {
      apiSvc.roles.save($scope.Role,
        function(result) {

          alertSvc.send('success', 'El rol se creo correctamente.');
        });
    } else {
      $scope.Role.Id = $scope.RolId;
      apiSvc.Rol.update({
        id: $scope.RolId
      }, $scope.Role, function(result) {
        if (result.status == 200) {
          alertSvc.send('success', result);
        }
      });
    }
    $location.path('/admin/roles');
  };

  $scope.cancel = function() {

    $location.path('/admin/roles');
  };

  $scope.nameChaged = function() {
    if ($scope.Role.Nombre) {
      var nameValidation = apiSvc.nameValidation.nameValidation({
        name: $scope.Role.Nombre
      }, function() {
        var serverMessage2 = $parse('rol.nombre.$error.rolExists');
        switch (nameValidation.status) {
          case 200:
            $scope.rol.$setValidity('nombre', true, $scope.rol);
            serverMessage2.assign($scope, undefined);
            break;
          default:
            $scope.rol.$setValidity('nombre', false, $scope.rol);
            serverMessage2.assign($scope, true);
        }

      });
    }

  };



});