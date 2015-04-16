'use strict';
var gulp = require('gulp'),
  connect = require('gulp-connect'),
  bower = require('gulp-bower'),
  concat = require('gulp-concat'),
  minifyHTML = require('gulp-minify-html'),
  minifyCSS = require('gulp-minify-css'),
  notify = require('gulp-notify'),
  changed = require('gulp-changed'),
  plumber = require('gulp-plumber'),
  uglify = require('gulp-uglify'),
  karma = require('gulp-karma'),
  ngConstant = require('gulp-ng-constant');

var paths = {
  config: {
    src: 'src/app/config.json',
    dst: './dist/app'
  },
  scripts: {
    src: ['src/**/*.js'],
    dst: './dist'
  },
  stylus: {
    src: ['src/styls/*.css'],
    dst: './dist/css'
  },
  views: {
    src: ['src/**/*.html'],
    dst: './dist'
  }
};

gulp.task('bower', function() {
  bower()
    .pipe(connect.reload());
});

gulp.task('scripts', function() {
  return gulp.src(paths.scripts.src)
    .pipe(plumber())
    .pipe(changed(paths.scripts.dst))
    .pipe(gulp.dest(paths.scripts.dst))
    .pipe(connect.reload());
});

gulp.task('htmlpage', function() {
  gulp.src(paths.views.src)
    .pipe(changed(paths.views.dst))
    .pipe(minifyHTML())
    .pipe(changed(paths.views.dst))
    .pipe(gulp.dest(paths.views.dst))
    .pipe(connect.reload());
});

gulp.task('stylus', function() {
  gulp.src(paths.stylus.src)
    .pipe(minifyCSS({
      keepBreaks: true
    }))
    .pipe(gulp.dest(paths.stylus.dst))
});

gulp.task('connect', function() {
  connect.server({
    root: ['dist'],
    port: 2000,
    livereload: true
  })
});

gulp.task('config', function() {
  gulp.src(paths.config.src)
    .pipe(ngConstant({
      name: 'myapp.constants',
      constants: {
        API: 'http://local.bpm.com/api',
        REVISION: process.env.GIT_REV,
        BUILD: process.env.GIT_BLD
      }
    }))
    .pipe(gulp.dest(paths.config.dst));
});
/*
var testFiles = [
  'dist/components/es5-shim/es5-shim.js',
  'dist/components/jquery/dist/jquery.min.js',
  'dist/components/lodash/dist/lodash.compat.js',
  'dist/components/angular/angular.js',
  'dist/components/angular-local-storage/angular-local-storage.js',
  'dist/components/angular-bootstrap/ui-bootstrap.js',
  'dist/components/angular-ui-select/dist/select.js',
  'dist/components/angular-animate/angular-animate.js',
  'dist/components/angular-loading-bar/build/loading-bar.js',
  'dist/components/angular-resource/angular-resource.js',
  'dist/components/angular-route/angular-route.js',
  'dist/components/angular-file-upload/angular-file-upload.js',
  'dist/components/angular-mocks/angular-mocks.js',*/
//'dist/app/config.js',
//'src/app/**/*.js',
// 'test/spec/**/*.js'
//];

gulp.task('test', function() {
  return gulp.src(testFiles)
    .pipe(karma({
      configFile: 'karma.conf.js',
      action: 'run'
    }))
    .on('error', notify.onError(function(error) {
      return error;
    }));
});


// Rerun the task when a file changes
// Globs are resolved before they're sent to Karma, so if you add a new file
// that matches a glob you passed using gulp.src('test/*').pipe(karma),
// it won't be caught by Karma
gulp.task('watch', function() {
  gulp.watch(paths.scripts.src, ['scripts']);
  gulp.watch(paths.stylus.src, ['stylus']);
  gulp.watch(paths.views.src, ['htmlpage']);
  /* gulp.src(testFiles)
    .pipe(karma({
      configFile: 'karma.conf.js',
      action: 'watch'
    }));*/
});

gulp.task('default', ['scripts', 'config', 'bower', 'stylus', 'htmlpage', 'connect', 'watch']);
gulp.task('build', ['scripts', 'config', 'bower', 'stylus', 'htmlpage']);