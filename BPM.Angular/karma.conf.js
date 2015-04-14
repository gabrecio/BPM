module.exports = function(config) {
  'use strict';
  config.set({
    basePath: '',
    frameworks: ['jasmine'],
    files: [
      'dist/components/es5-shim/es5-shim.js',
      'dist/components/jquery/dist/jquery.min.js',
      'dist/components/lodash/dist/lodash.min.js',
      'dist/components/bootstrap/dist/js/bootstrap.min.js',
      'dist/components/angular/angular.min.js',
      'dist/components/angular-local-storage/angular-local-storage.js',
      'dist/components/angular-route/angular-route.min.js',
      'dist/components/angular-animate/angular-animate.min.js',
      'dist/components/angular-resource/angular-resource.js',
      'dist/components/angular-bootstrap/ui-bootstrap-tpls.min.js',
      'dist/components/angular-ui-select/dist/select.js',
      'dist/components/angular-loading-bar/src/loading-bar.js',
      'dist/components/angular-file-upload/angular-file-upload.js',
      'dist/components/angular-mocks/angular-mocks.js',
      'dist/app/config.js',
      'src/app/**/*.js',
      'test/spec/**/*.js'
    ],
    exclude: [],
    reporters: ['progress'],
    port: 9876,
    colors: true,
    logLevel: config.LOG_INFO,
    autoWatch: true,
    browsers: ['PhantomJS'],
    captureTimeout: 60000,
    singleRun: false
  });
};