$Name="Baavgai.Db"
$ProjFile="$($PSScriptRoot)\$Name.csproj"
$NuspecFile="$($PSScriptRoot)\$Name.nuspec"
$NuspecBasePath="$($PSScriptRoot)\nuget"
if (Test-Path $NuspecBasePath) {
    ri -Recurse $NuspecBasePath
}
dotnet pack $ProjFile -o $NuspecBasePath -p:NuspecFile=$NuspecFile # -p:NuspecBasePath=$NuspecBasePath

