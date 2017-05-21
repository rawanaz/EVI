choco install opencover -y
OpenCover.Console.exe -register:user -returntargetcode -target:"vstest.console.exe" -targetargs:"src\Slp.Evi.Storage\Slp.Evi.Test.Unit\bin\Release\Slp.Evi.Test.Unit.dll src\Slp.Evi.Storage\Slp.Evi.Test.System\bin\Release\Slp.Evi.Test.System.dll /logger:Appveyor" -filter:"+[Slp.Evi.Storage*]*" -output:".\coverage.xml"
Add-AppveyorMessage -Message "Tests exit code: $LastExitCode"
