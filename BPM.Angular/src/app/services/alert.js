'use strict';

angular
  .module('myapp')
  .service('alertSvc', function($rootScope) {

    var alertSvc = this;

    alertSvc.send = function(evt, type, alert) {
      if (!alert) {
        alert = {
          message: type.message || type
        };
        type = evt;
        evt = 'alertSvc';
      }

      $rootScope.$broadcast(evt, {
        type: type,
        msg: alert.message,
        code: alert.code
      });
    };

    return alertSvc;
  });