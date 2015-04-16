angular.module('myapp')
  .factory('authInterceptor', function($q, alertSvc, localStorageService, $location) {
    return {
      request: function(config) {
        config.headers = config.headers || {};
        var authData = localStorageService.get('token');

        if (authData) {
            config.headers.Accept = 'application/json , text/javascript, */*';
          config.headers.Authorization = 'Bearer ' + authData;
        }

        return config;
      },
      'responseError': function(response) {
        if (typeof(response.data.error) == "object") {
          alertSvc.send('authInterceptor:' + response.status, 'danger',
            response.data.error);
        } else {
          alertSvc.send('danger', response.data.error_description);
        }
        return $q.reject(response);
      }
    };
  });