function Update-Config 
{
	$mssqlConnectionString = "Server=(local)\SQL2014;Database=master;User ID=sa;Password=Password12!";
	
	foreach($o in $input) 
	{
		Write-output ('Updating ' + $o.FullName)
	
		$doc = (Get-Content $o.FullName) -as [xml];
		$doc.SelectSingleNode('//connectionStrings/add[@name="mssql_connection"]').connectionString = $mssqlConnectionString;
		$doc.Save($o.FullName);
	}
	
}

$env:APPVEYOR_BUILD_FOLDER | get-childitem -recurse |? {$_.Name -eq "app.config"} | Update-Config;