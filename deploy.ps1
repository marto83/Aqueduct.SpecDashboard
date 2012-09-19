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
	$site_url = "http://specs.aquepreview.com"
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