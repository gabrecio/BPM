angular.module('myapp')
  .factory('permissions', function($rootScope) {
    var permissionList;
    return {
      setPermissions: function(permissions) {
        permissionList = permissions;
        $rootScope.$broadcast('permissionsChanged')
      },
      hasPermission: function(permission) {
        permission = permission.trim();
        return _.some(permissionList, function(item) {
          if (_.isString(item))
            return item.trim() === permission
        });
      }
    };
  });