'use strict';

angular
  .module('myapp')
  .service('loginSvc', function(
    $rootScope,
    $http,
    $idle,
    $keepalive,
    localStorageService,
    alertSvc,
    API,
    permissions
  ) {

    var loginSvc = this;

    loginSvc.isAuthenticated = function() {
      return !!localStorageService.get('token');
    };
    var windowStorageListener = function(event) {
      if (event.key === 'ls.token') {
        if (event.newValue) {
          $rootScope.$broadcast('loginSvc:login');
        } else {
          $rootScope.$broadcast('loginSvc:logout');
        }
        $rootScope.$apply();
      }
    };

    var watch = function() {
      if (!$idle.running()) {
        console.log('Idle watch, I am watching you!');
        $idle.watch();
      }
    };

    var unwatch = function() {
      if ($idle.running()) {
        console.log('Idle unwatch, bye bye!');
        $idle.unwatch();
      }
    };

    if (!window.addEventListener) {
      window.attachEvent('storage', windowStorageListener);
    } else {
      window.addEventListener('storage', windowStorageListener, false);
    }

    loginSvc.login = function(credentials, callback) {
      // TODO: use $q deferred promises and return them to chain further
      var data = "grant_type=password&username=" + credentials.username + "&password=" + credentials.password;

      $http.post(API + '/token', data, {
        headers: {
          'accept': 'application/json',
          'Content-Type': 'application/x-www-form-urlencoded'
        }
      })
        .success(function(response) {
          watch();
          localStorageService.set('token', response.access_token);
          localStorageService.set('username', credentials.username);
          localStorageService.set('permissions', response.PermissionList.split("-"));
          permissions.setPermissions(localStorageService.get('permissions'));


          $rootScope.$broadcast('loginSvc:login');
          if (angular.isFunction(callback)) {
            callback(null, response.profile);
          }
        })
        .error(function(data) {
          unwatch();
          localStorageService.remove('token');
          localStorageService.remove('username');
          localStorageService.remove('permissions');

          $rootScope.$broadcast('loginSvc:error');
          if (angular.isFunction(callback) && angular.isDefined(data.error)) {
            callback(data.error, null);
          }
        });
    };

    loginSvc.logout = function() {
      unwatch();
      localStorageService.remove('token');
      localStorageService.remove('username');
      localStorageService.remove('permissions');
      $rootScope.$broadcast('loginSvc:logout');
    };

    /*  loginSvc.refresh = function() {
      $http.get(API + '/auth/refresh').success(function(response) {
        localStorageService.set('token', response.access_token);
        localStorageService.set('username', response.profile.username);
      });
    };
*/
    $rootScope.$on('loginSvc:login', function() {
      watch();
    });

    $rootScope.$on('loginSvc:logout', function() {
      unwatch();
    });

    $rootScope.$on('$idleStart', function() {
      console.log('You are idling now, taking a nap?');
    });

    $rootScope.$on('$idleWarn', function() {
      alertSvc.send(
        'loginSvc:idleWarning',
        'info', {
          message: 'Tu sesión está por expirar, continúa trabajando para renovar la sesión!'
        }
      );
    });

    $rootScope.$on('$idleTimeout', function() {
      console.log('Idle timeout, you are dead now!');
      loginSvc.logout();
      alertSvc.send('danger', 'Tu sesión expiró, entra de nuevo!');
    });

    $rootScope.$on('$keepalive', function() {
      console.log('I am keeping your session alive!');
      //loginSvc.refresh();
    });

    if (loginSvc.isAuthenticated()) {
      watch();
    }

    return loginSvc;
  });