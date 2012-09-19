/*
 * Require Js build config
 * =======================
 * Full set of options can be found at: 
 * https://github.com/jrburke/r.js/raw/master/build/example.build.js
 */
({
    appDir: "build/_PublishedWebsites/Aqueduct.SpecDashboard",
    dir: "build/_PublishedWebsites/Aqueduct.SpecDashboard",
    baseUrl: "./js",
    paths: {
        jquery: "../lib/jquery-core",
        lib: "../lib", 
        "jquery-plugin": "../lib/jquery-plugins",
        images: "../images"
    },
 
    optimize: "uglify",
    uglify: {
        toplevel: true,
        ascii_only: true
    },
	
    optimizeCss: "standard",
    //cssImportIgnore: null,
 
    //Inlines the text for any text! dependencies, to avoid the separate
    //async XMLHttpRequest calls to load those dependencies.
    inlineText: true,
 
    //Skip processing for pragmas.
    skipPragmas: false,
    skipModuleInsertion: false,
 
    optimizeAllPluginResources: false,
    findNestedDependencies: true,
 
	fileExclusionRegExp: /^(sitecore|lib|sitecore modules|sitecore_overrides|contenteditor)/,

    //List the modules that will be optimized. All their immediate and deep
    //dependencies will be included in the module's file when the build is
    //done. If that module or any of its dependencies includes i18n bundles,
    //only the root bundles will be included unless the locale: section is set above.
    
 	
})

