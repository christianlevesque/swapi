$testResultsPath = "./test/UnitTests/TestResults"
$coveragePath = "./coverage"

Remove-Item -Recurse $testResultsPath
Remove-Item -Recurse $coveragePath

dotnet test ./test/UnitTests/UnitTests.csproj --settings runsettings.xml --collect="XPlat Code Coverage"

$resultsHashPath = ls $testResultsPath

reportgenerator.exe "-reports:test/UnitTests/TestResults/$resultsHashPath/coverage.cobertura.xml" "-targetdir:coverage" -reporttypes:Html