var jsBundles = [
    "../Content/js/app.js.bundle"
];
var cssBundles = [
    "../Content/css/app.css.bundle"
];


var fs = require("fs"),
    path = require("path"),
	jsp = require("uglify-js").parser,
	pro = require("uglify-js").uglify,
    less = require('less'),
    coffee = require('coffee-script');

/*
less.render('.class { width: 1 + 1 }', function (e, css) {
    console.log(css);
});
var jsCoffee = coffee.compile("a = (x) -> x * x * x");
console.log(jsCoffee);
*/

String.prototype.startsWith = function (str){
	return this.indexOf(str) === 0;
};
String.prototype.endsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};

jsBundles.forEach(function (jsBundle) {
    var bundleDir = path.dirname(jsBundle);
    var jsFiles = fs.readFileSync(jsBundle).toString('utf-8').replace("\r","").split("\n");
    var bundleName = jsBundle.replace('.bundle','');

    var allJs="", allMinJs="";
    jsFiles.forEach(function (file) {
        if (!(file = file.trim()) || file.startsWith(".")) return; // . ..
        
        var isCoffee = file.endsWith(".coffee"), jsFile = isCoffee
            ? file.replace(".coffee", ".js")
            : file;

        var filePath = path.join(bundleDir, file),
            jsPath = path.join(bundleDir, jsFile);
        var minJsPath = jsPath.replace(".js", ".min.js");

        //console.log(jsPath, minJsPath);

        var js = isCoffee
            ? getOrCreateJs(fs.readFileSync(filePath).toString('utf-8'), filePath, jsPath)
            : fs.readFileSync(jsPath).toString('utf-8');

        var minJs = getOrCreateMinJs(js, jsPath, minJsPath);

        allJs += js + ";";
        allMinJs += minJs + ";";
    });

    console.log("writing " + bundleName + "...");
    fs.writeFileSync(bundleName, allJs);
    fs.writeFileSync(bundleName.replace(".js", ".min.js"), allMinJs);
});

function getOrCreateJs(coffeeScript, csPath, jsPath) {
    var csStat = fs.statSync(csPath);

    var compileJs = !path.existsSync(jsPath) 
        || fs.statSync(jsPath).mtime.getTime() < csStat.mtime.getTime();

    console.log(jsPath + ":" + compileJs)

    var js = compileJs
        ? coffee.compile(coffeeScript)
        : fs.readFileSync(jsPath).toString('utf-8');

    if (compileJs) 
        fs.writeFileSync(jsPath, js);
    
    return js;    
}

function getOrCreateMinJs(js, jsPath, minJsPath) {
    var jsStat = fs.statSync(jsPath);

    var rewriteMinJs = !path.existsSync(minJsPath) 
        || fs.statSync(minJsPath).mtime.getTime() < jsStat.mtime.getTime();

    console.log(minJsPath + ":" + rewriteMinJs)

    var minJs = rewriteMinJs
        ? minifyjs(js)
        : fs.readFileSync(minJsPath).toString('utf-8');

    if (rewriteMinJs) 
        fs.writeFileSync(minJsPath, minJs);
    
    return minJs;    
}

function minifyjs(js) {
    var ast = jsp.parse(js);
    ast = pro.ast_mangle(ast);
    ast = pro.ast_squeeze(ast);
    var minJs = pro.gen_code(ast);
    return minJs;
}
