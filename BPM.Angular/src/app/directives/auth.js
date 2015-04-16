angular.module('myapp').directive("ngShowAuth", function($rootScope, localStorageService) {

  return {
    restrict: "A",
    link: function(scope, el, attr) {
      var fn = function() {
        var authorized = attr.ngShowAuth.split('-');
        var profiles = localStorageService.get('permissions') ? localStorageService.get('permissions') : [];
        visible = _.intersection(authorized, profiles);
        el.toggleClass('ng-cloak', visible.length === 0);
      }
      $rootScope.$on('loginSvc:login', fn);
      $rootScope.$on('loginSvc:logout', fn);

      fn();

    }
  };
});