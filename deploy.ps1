properties { 
	#Folder Settings
	$base_dir  = resolve-path .
	$build_dir = "$base_dir\build" 
	$tools_dir = "$base_dir\tools"
	$publish_dir = "$build_dir\publish"
	$integration_tests_dir = "$build_dir\integration tests"
	
	#MSBuild Settings
	$framework = "4.0" #Needed by PSAKE
	
	#Deploy to live. Yes, immediately.
	$config = "Release"
	$site_unc = "\\aque-sqldev\d$\Websites\Aqueduct\SpecDashboard"
	$site_url = "http://spec.aquepreview.com"
} 

task UnZip {
	if (Test-Path $publish_dir\$config) { remove-item -force -recurse $publish_dir\$config }
	
	$old = pwd
	cd $publish_dir
	
	exec { & $tools_dir\7-zip\7za.exe x $publish_dir\$config.zip }
	
	cd $old
}

task Deploy -depends UnZip { 	
	robocopy "$publish_dir\$config" "$site_unc" /e /njh /njs /ndl /nc /ns /np 
	
	$webclient = New-Object Net.WebClient
	$webclient.DownloadString($site_url) | Out-Null
}

task SeleniumTest {
	$env:testUrl = $site_url #Set environment variable. It's the only way to pass a variable to an NUnit test.
	$test_dlls = Get-ChildItem $integration_tests_dir\*.Selenium.Tests.dll
	
	foreach($dll in $test_dlls) { 
		$name = $dll.name
		exec { & $tools_dir\nunit\nunit-console.exe $dll /xml:$integration_tests_dir\TestResult-$name.xml }
	}
}