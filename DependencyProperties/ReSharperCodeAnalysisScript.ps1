$slnFile = Get-ChildItem -Path ".\**" -Filter *.sln -Recurse
$settingsFile = Get-ChildItem -Path ".\**" -Filter *.sln.DotSettings -Recurse
$severity = "WARNING"
$outputFile = ".\inspect-code-log.xml"
#just a container for Resharper CLT Nuget
$projectForResharperClt = Get-ChildItem -Path ".\**" -Filter *.csproj -Recurse
$packageDirectory = ".\packages"

#Preparing inspectCode tool
& dotnet add $projectForResharperClt package JetBrains.ReSharper.CommandLineTools --package-directory $packageDirectory

#Running code analysis
$inspectCode = Get-ChildItem -Path ".\**" -Filter *inspectcode.exe -Recurse
& $inspectCode --profile=$settingsFile $slnFile -o="$outputFile" -s="$severity"

#processing result file
[xml]$xml = gc $outputFile
if ($xml.Report.Issues.ChildNodes.Count -gt 0)
{
 write-error ("`nCode analysis failed: `n" + ((gc $outputFile) -join "`n"))
}
else
{
 echo "No issues found"
}