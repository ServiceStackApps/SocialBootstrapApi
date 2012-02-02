var appBundles = ["../Content/js/app.bundle.txt"];

var fs = require("fs"),
    path = require("path"),
	jsp = require("uglify-js").parser,
	pro = require("uglify-js").uglify;

String.prototype.startsWith = function (str){
	return this.indexOf(str) === 0;
};
String.prototype.endsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};

appBundles.forEach(function (appBundle) {
    var bundleDir = path.dirname(appBundle);
    var jsFiles = fs.readFileSync(appBundle).toString('utf-8').replace("\r","").split("\n");
    var bundleName = path.join(bundleDir, path.basename(appBundle, '.txt'));

    var allJs="", allMinJs="";
    jsFiles.forEach(function (jsFile) {
        if (!(jsFile = jsFile.trim()) || jsFile.startsWith(".")) return; // . ..
        
        var jsPath = path.join(bundleDir, jsFile);
        var minJsPath = jsPath.replace(".js", ".min.js");

        //console.log(jsPath, minJsPath);

        var js = fs.readFileSync(jsPath).toString('utf-8');
        var minJs = getOrCreateMinJs(js, jsPath, minJsPath);

        allJs += js + ";";
        allMinJs += minJs + ";";
    });

    console.log("writing.. " + bundleName + ".js");
    fs.writeFileSync(bundleName + ".js", allJs);
    fs.writeFileSync(bundleName + ".min.js", allMinJs);
});

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

/*
var srcDir = '../Content/js', targetDir = '../dist', allJsMap = {}, allMinJsMap = {};
var files = fs.readdirSync(srcDir);

files.forEach(function(file) { 
	if (file.charAt(0) == ".") return;
	var srcPath = srcDir + '/' + file, 
		targetPath = targetDir + '/' + file.replace('.js', '.min.js');
	var js = fs.readFileSync(srcPath).toString('utf-8');
	var ast = jsp.parse(js);
	ast = pro.ast_mangle(ast);
	ast = pro.ast_squeeze(ast);
	var minJs = pro.gen_code(ast); 
	console.log("writing " + file);
	if (file.startsWith("jquip") && !file.startsWith("jquip.q-"))
	{
		allJsMap[file] = js;	
		allMinJsMap[file] = minJs;	
	}
	fs.writeFileSync(targetPath, minJs);
});


var topFile = 'jquip.js';

//write /dist/jquip.all.js
var allJs = allJsMap[topFile] + ";";
for (var file in allJsMap) {
	if (file == topFile) continue;
	allJs += allJsMap[file] + ";";
}
fs.writeFileSync(targetDir + '/jquip.all.js', allJs);

//write /dist/jquip.all.min.js
var allMinJs = allMinJsMap[topFile] + ";";
for (var file in allMinJsMap) {
	if (file == topFile) continue;
	allMinJs += allMinJsMap[file] + ";";
}
fs.writeFileSync(targetDir + '/jquip.all.min.js', allMinJs);
*/