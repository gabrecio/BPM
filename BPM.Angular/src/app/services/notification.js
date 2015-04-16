/*
 * Notification service
 *
 * This service handles Server-Side Events from aliens api
 */

// TODO: Unit testing
angular.module('myapp').factory('notification', function($rootScope, loginSvc, API, localStorageService) {
  'use strict';
  /* global EventSource */
  var eventSource;
  var registeredEvents = {};

  var registerEventSource = function() {
    if (eventSource) {
      eventSource.close();
    }
    eventSource = new EventSource(API + '/notification-events/' + localStorageService.get('token'));
  }

  if (loginSvc.isAuthenticated()) {
    registerEventSource();
  }
  var windowStorageListener = function(event) {
    if (eventSource === undefined || eventSource.readyState === eventSource.CONNECTING) {
      if (event.key === 'ls.notification' && event.newValue) {
        var notification = JSON.parse(event.newValue);
        _.map(registeredEvents[notification.name], function(listener) {
          listener(notification.event);
        });
      }
    }
  };
  if (!window.addEventListener) {
    window.attachEvent('storage', windowStorageListener);
  } else {
    window.addEventListener('storage', windowStorageListener, false);
  }

  $rootScope.$on('loginSvc:login', function() {
    registerEventSource();
    _.map(registeredEvents, function(event) {
      _.map(event, function(listener) {
        eventSource.addEventListener(event, listener);
      });
    });
  });

  $rootScope.$on('loginSvc:logout', function() {
    if (eventSource) {
      eventSource.close();
      eventSource = undefined;
    }
  });

  return {
    addEventListener: function(eventName, callback) {
      if (eventName === undefined || callback === undefined) {
        throw 'Parameters \'event\' and \'callback\' are required';
      }

      var listener = function(event) {
        // if(event.origin !== API){
        //   throw 'The event origin is not from API!';
        // }
        localStorageService.set('notification', {
          name: eventName,
          event: event
        });

        callback(JSON.parse(event.data));
        $rootScope.$apply();
      };

      if (registeredEvents[eventName]) {
        registeredEvents[eventName].push(listener);
      } else {
        registeredEvents[eventName] = [listener];
      }

      if (eventSource) {
        eventSource.addEventListener(eventName, listener);
      }
      return function() {
        _.remove(registeredEvents[eventName], function(attachedListener) {
          return listener === attachedListener.listener;
        });
        return eventSource.removeEventListener(listener);
      };
    },
    close: function() {
      if (eventSource) {
        eventSource.close();
      }
    }
  };
})