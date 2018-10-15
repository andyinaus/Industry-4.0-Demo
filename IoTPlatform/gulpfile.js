"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify");

var paths = {
    webroot: "./wwwroot/",
    node_modules: 'node_modules/',
    assets: "./Assets/"
};

paths.js = paths.assets + "js/*.js";
paths.bootstrapJs = paths.node_modules + 'bootstrap/dist/js/bootstrap.js';
paths.jquery = paths.node_modules + 'jquery/dist/jquery.js';
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.assets + "css/*.css";
paths.bootstrapCss = paths.node_modules + "bootstrap/dist/css/bootstrap.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.js";
paths.concatCssDest = paths.webroot + "css/site.css";
paths.concatMinJsDest = paths.webroot + "js/site.min.js";
paths.concatMinCssDest = paths.webroot + "css/site.min.css";

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task('clean', ['clean:js', 'clean:css']);

gulp.task("dev:js", function () {
    return gulp.src([paths.js, paths.bootstrapJs, paths.jquery, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(gulp.dest("."));
});

gulp.task("dev:css", function () {
    return gulp.src([paths.css, paths.bootstrapCss, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(gulp.dest("."));
});

gulp.task('dev', ['dev:js', 'dev:css']);

gulp.task("min:js", function () {
    return gulp.src([paths.js, paths.bootstrapJs, paths.jquery, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatMinJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, paths.bootstrapCss, "!" + paths.minCss])
        .pipe(concat(paths.concatMinCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task('min', ['min:js', 'min:css']);

gulp.task('build:dev', ['clean', 'dev']);
gulp.task('build:prod', ['clean', 'min']);