/// <binding BeforeBuild='build:dev' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    babel = require("gulp-babel");

var paths = {
    webroot: "./wwwroot/",
    node_modules: 'node_modules/',
    assets: "./Assets/"
};

paths.js = paths.assets + "js/*.js";
paths.bootstrapJs = paths.node_modules + 'bootstrap/dist/js/bootstrap.js';
paths.vueJs = paths.node_modules + 'vue/dist/vue.js';
paths.d3Js = paths.node_modules + 'd3/dist/d3.js';
paths.jquery = paths.node_modules + 'jquery/dist/jquery.js';
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.assets + "css/*.css";
paths.bootstrapCss = paths.node_modules + "bootstrap/dist/css/bootstrap.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.js";
paths.concatCssDest = paths.webroot + "css/site.css";
paths.concatMinJsDest = paths.webroot + "js/site.min.js";
paths.concatMinCssDest = paths.webroot + "css/site.min.css";

paths.imgs = paths.assets + "img/*";
paths.imgDest = paths.webroot + "images";

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task('clean', ['clean:js', 'clean:css']);

gulp.task("clean:img", function(cb) {
    rimraf(paths.imgDest + "/", cb);
});

gulp.task("move:img", function() {
    return gulp.src(paths.imgs)
        .pipe(gulp.dest(paths.imgDest));
});

gulp.task("dev:js", function () {
    return gulp.src([paths.jquery, paths.bootstrapJs, paths.vueJs, paths.d3Js, paths.js, "!" + paths.minJs], { base: "." })
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
    return gulp.src([paths.jquery, paths.bootstrapJs, paths.vueJs, paths.d3Js, paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatMinJsDest))
        .pipe(babel({ presets: ['es2015'] }))
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
gulp.task('img', ['clean:img', 'move:img']);