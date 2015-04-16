'use strict';
var RouteSecurityManager;

angular.module('security', []).run(function($injector, $location, $rootScope, localStorageService, loginRedirectPath) {
  if ($injector.has('$route')) {
    return new RouteSecurityManager($location, $rootScope, localStorageService, $injector.get('$route'), loginRedirectPath);
  }
});

RouteSecurityManager = function($location, $rootScope, localStorageService, $route, path) {
  this._route = $route;
  this._location = $location;
  this._rootScope = $rootScope;
  this._loginPath = path;
  this._redirectTo = null;
  this._localStorage = localStorageService;
  this._authenticated = !! (this._localStorage.get('token'));
  this._init();
};

RouteSecurityManager.prototype = {
  _init: function() {
    var self;
    self = this;
    this._checkCurrent();
    self._rootScope.$on('$routeChangeStart', function(e, next, current) {
      self._authRequiredRedirect(next, self._loginPath, current);
    });
    self._rootScope.$on('loginSvc:login', angular.bind(this, this._login));
    self._rootScope.$on('loginSvc:logout', angular.bind(this, this._logout));
    self._rootScope.$on('loginSvc:error', angular.bind(this, this._error));
  },
  _checkCurrent: function() {
    if (this._route.current) {
      this._authRequiredRedirect(this._route.current, this._loginPath);
    }
  },
  _login: function() {
    this._authenticated = true;
    if (this._redirectTo) {
      this._redirect(this._redirectTo);
      this._redirectTo = null;
    } else if (this._location.path() === this._loginPath) {
      this._location.replace();
      this._location.path('/');
    }
  },
  _logout: function() {
    this._authenticated = false;
    this._checkCurrent();
  },
  _error: function() {
    if (!this._localStorage.get('token')) {
      this._authenticated = false;
    }
    this._checkCurrent();
  },
  _redirect: function(path) {
    this._location.replace();
    this._location.path(path);
  },
  _authRequiredRedirect: function(route, path, current) {
    if (route.authRequired && !this._authenticated) {
      // CLAIM-210: Evitar que se dirija al ultimo path visitado
      // if (route.pathTo === undefined) {
      //   this._redirectTo = this._location.path();
      // } else {
      //   this._redirectTo = (route.pathTo === path ? '/' : route.pathTo);
      // }
      this._redirect(path);
    } else {
      if (this._authenticated && this._location.path() === this._loginPath) {
        this._redirect('/');
      }
    }
  }
};