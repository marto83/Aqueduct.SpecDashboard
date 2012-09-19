properties { 
	#Folder Settings
	$base_dir  = resolve-path .
	$build_dir = "$base_dir\build" 
	$tools_dir = "$base_dir\tools"
	$publish_dir = "$build_dir\publish"
	$frontend_base_dir = "$build_dir\_PublishedWebsites\Aqueduct.SpecDashboard"
	$unit_tests_dir = "$build_dir\unit tests"
	$integration_tests_dir = "$build_dir\integration tests"
	
	#MSBuild Settings
	$sln_file = "$base_dir\src\Aqueduct.SpecDashboard.sln"
	$website_file = "$base_dir\src\Aqueduct.SpecDashboard\Aqueduct.SpecDashboard.csproj"	
	$framework = "4.0" #Needed by PSAKE
	$configs = "Release"
} 

task default -depends UnitTestps1

task Clean { 
	KillNUnitAgent
	if (Test-Path $build_dir) { remove-item -force -recurse $build_dir }
} 

task Init -depends Clean { 
	new-item $build_dir -itemType directory 
	new-item $build_dir\reports -itemType directory 
}

task Compile -depends Init { 
	$msbuild_out_dir = $build_dir.Replace("\", "\\") + "\\"
	exec { msbuild /p:configuration=debug /p:outdir="$msbuild_out_dir" /verbosity:quiet /t:rebuild "$sln_file"  }
} 

function RunTests($test_dlls, $folder) {
	$old = pwd
	cd $folder
		
	foreach ($dll in $test_dlls)
	{
		$name = $dll.name
		Write-Host $name
		
		exec { & $tools_dir\opencover\opencover.console.exe "-target:$tools_dir\nunit\nunit-console.exe" "-targetargs:$name /noshadow /nologo /xml:TestResult-$name.xml" -register:user "-output:results-$name.xml" "-filter:+[Aqueduct*]* -[*Tests]* -[Aqueduct.Site]Aqueduct.Site.layouts.*" -log:all -returntargetcode }
	}
	
	KillNUnitAgent	
	cd $old
}

function KillNUnitAgent {
	#URRRGH. There's a bug which sometimes leaves nunit-agent hanging around using up processor after a test. Sorry!
	Stop-Process -processname nunit-agent -ErrorAction SilentlyContinue
}

task UnitTest -depends Compile {
	$test_dlls = Get-ChildItem $unit_tests_dir\*.Tests.dll
	
	RunTests $test_dlls $unit_tests_dir
}

task IntegrationTest -depends UnitTest {
	$test_dlls = Get-ChildItem $integration_tests_dir\*.Tests.dll -Exclude *.Selenium.Tests.dll
	
	RunTests $test_dlls $integration_tests_dir
}

task CoverageReport -depends UnitTest, IntegrationTest { 	
	$reports = Get-ChildItem "$build_dir\* tests\results*.xml"
	$reports_list = [string]::join(';', $reports)
	
	exec { & $tools_dir\ReportGenerator\bin\ReportGenerator.exe "-reports:$reports_list" "-targetdir:$build_dir\reports" -verbosity:Error }	
}

task CombineMinify -depends Compile {
	exec { & $tools_dir\nodejs\node.exe "$tools_dir\requirejs\r.js" -o "build.js" appDir="$frontend_base_dir" dir="$frontend_base_dir" > $build_dir\reports\combineminify.txt } 
}

task PublishLocal -depends Compile, CombineMinify {
	new-item $publish_dir -itemType directory
	
	foreach ($config in $configs)
	{
		write-host "Publishing $config"
		exec { msbuild /t:PipelinePreDeployCopyAllFilesToOneFolder /p:_PackageTempDir="$publish_dir\$config\\" /verbosity:quiet /p:Configuration="$config" "$website_file"  }
		
		robocopy "$frontend_base_dir\css" "$publish_dir\$config\css" /e /njh /njs /ndl /nc /ns /np /nfl
		robocopy "$frontend_base_dir\js" "$publish_dir\$config\js" /e /njh /njs /ndl /nc /ns /np /nfl
		robocopy "$frontend_base_dir\lib" "$publish_dir\$config\lib" /e /njh /njs /ndl /nc /ns /np /nfl
	}
}

task Zip -depends PublishLocal {
	foreach ($config in $configs)
	{
		write-host "Zipping $config"
		exec { & $tools_dir\7-zip\7za.exe a $publish_dir\$config.zip $publish_dir\$config > $null  }
	}
}

