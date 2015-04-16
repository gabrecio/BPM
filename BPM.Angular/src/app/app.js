'use strict';
var permissionList;
angular.module('myapp', [
  'myapp.constants',
  'ngRoute',
  'ngResource',
  'security',
  'ui.bootstrap',
  'ui.select',
  'chieffancypants.loadingBar',
  'ngAnimate',
  'LocalStorageModule',
  'angularFileUpload',
  'ngIdle'
 // 'multi-select'
])
  .config(function($httpProvider) {
    $httpProvider.interceptors.push('authInterceptor');
  })
  .config(function(cfpLoadingBarProvider) {
    cfpLoadingBarProvider.includeSpinner = false;
  })
  .config(function($idleProvider, $keepaliveProvider) {
    $idleProvider.idleDuration(9 * 60);
    $idleProvider.warningDuration(60);
    $keepaliveProvider.interval(5 * 60);
  })
  .config(function($routeProvider, $locationProvider) {
    $routeProvider
      .when('/admin/users', {
        authRequired: true,
        templateUrl: 'views/users.html',
        permission: 'Usuario.Consultar'
      })
      .when('/admin/users/new', {
          authRequired: true,
        templateUrl: 'views/usersNew.html',
        controller: 'usersNewCtrl',
        permission: 'Usuario.Agregar'
      })
      .when('/admin/users/edit/:userId', {
        authRequired: true,
        templateUrl: 'views/usersNew.html',
        controller: 'usersNewCtrl',
        permission: 'Usuario.Editar'
      })
      .when('/admin/roles', {
        authRequired: true,
        templateUrl: 'views/roles.html',
        permission: 'Roles.Consultar'
      })
      .when('/admin/roles/new', {
          authRequired: true,
        templateUrl: 'views/rolesNew.html',
        controller: 'rolesNewCtrl',
        permission: 'Roles.Agregar'
      })
      .when('/admin/roles/edit/:rolId', {
        authRequired: true,
        templateUrl: 'views/rolesNew.html',
        controller: 'rolesNewCtrl',
        permission: 'Roles.Editar'
      })
      .when('/unauthorized', {
        templateUrl: 'views/error.html',
        controller: 'ErrorCtrl'
      })
      .when('/login', {
        templateUrl: 'views/login.html',
        controller: 'LoginCtrl'
      })
      .otherwise({
        redirectTo: '/admin/users'
      });
  }).run(['$rootScope', 'localStorageService', '$location', 'permissions', 'alertSvc',
    function($rootScope, localStorageService, $location, permissions, alertSvc) {


      $rootScope.$on("$routeChangeStart", function(event, next, current) {
        // if you're logged out send to login page.
        if (next.permission) {
          console.log(localStorageService.get('permissions'));
          permissions.setPermissions(localStorageService.get('permissions'));

          var permission = next.$$route.permission;
          //console.log(permission);
          if (_.isString(permission) && !permissions.hasPermission(permission)) {
            alertSvc.send('danger', 'No tiene autorizaciÃ³n para el recurso seleccionado.');
            $location.path('/login');
            //angular.bootstrap(document, ['aliens']);
            //event.preventDefault();
          }
        }
      });
    }
  ]);