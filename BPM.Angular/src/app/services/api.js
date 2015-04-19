angular.module('myapp').service('apiSvc', function($resource, API, localStorageService) {
  'use strict';

  var base = API + '/api';
  var core = {
    base: base,
    headers: {
      Authorization: 'Bearer ' + localStorageService.get('token')
    }
  };


  //User End Points

  core.User = $resource(base + '/usuario/:id', {
    id: '@id'
  }, {
    'delete': {
      method: 'DELETE',
      isArray: false,
      cache: false
    },
    'update': {
      method: 'PUT',
      isArray: false,
      cache: false
    },
    'getUserById': {
      method: 'GET',
      isArray: false,
      cache: false
    }
  });

  core.users = $resource(base + '/usuario', {}, {
    'users': {
      url: base + '/usuario',
      method: 'GET',
      isArray: false,
      cache: false
    }
  });

 /* core.findUsers = $resource(base + '/usuario/find/:pagingInfo', {
    pagingInfo: '@pagingInfo'
  }, {
    'findUsers': {
      url: base + '/usuario/find/:pagingInfo',
      method: 'GET',
      isArray: false,
      cache: false
    }
  });*/

  core.findUsers = $resource(base + '/usuario/find', {
  }, {
      'findUsers': {
          url: base + '/usuario/find',
          method: 'GET',
          isArray: false,
          cache: false
      }
  });

  core.usernameValidation = $resource(base + '/usuario/usernameValidation', {
  }, {
    'usernameValidation': {
      url: base + '/usuario/usernameValidation',
      method: 'GET',
      isArray: false,
      cache: false
    }
  });

  //Rol End Points

  core.Rol = $resource(base + '/rol/:id', {
    id: '@id'
  }, {
    'delete': {
      method: 'DELETE',
      isArray: false,
      cache: false
    },
    'update': {
      method: 'PUT',
      isArray: false,
      cache: false
    },
    'getRolById': {
      url: base + '/rol/:id',
      method: 'GET',
      isArray: false,
      cache: false
    },
    'getRolPermissions': {
      url: base + '/rol/RolPermission/:id',
      method: 'GET',
      isArray: true,
      cache: false
    }
  });

  core.findRoles = $resource(base + '/rol/Find/:pagingInfo', {
    pagingInfo: '@pagingInfo'
  }, {
    'findRoles': {
      url: base + '/rol/Find/:pagingInfo',
      method: 'GET',
      isArray: false,
      cache: false
    }
  });

  core.roles = $resource(base + '/rol', {}, {
    'roles': {
      url: base + '/rol',
      method: 'GET',
      isArray: false,
      cache: false
    }
  });

  core.nameValidation = $resource(base + '/rol/nameValidation/:name', {
    name: '@name'
  }, {
    'nameValidation': {
      url: base + '/rol/nameValidation/:name',
      method: 'GET',
      isArray: false,
      cache: false
    }
  });


  return core;
});