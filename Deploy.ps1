$root = ".\Source"

Remove-Item -Path $root\WetPicsRebirth\bin -Recurse
Remove-Item -Path $root\WetPicsRebirth\obj -Recurse

[xml]$XmlDocument = Get-Content -Path $root\WetPicsRebirth\WetPicsRebirth.csproj

$version = $XmlDocument.Project.PropertyGroup.Version
$version
$tag = "imoutochan/wetpicsrebirth:" + $version
$tag

cd $root
docker build --tag=$tag -t imoutochan/wetpicsrebirth . -f WetPicsRebirth\Dockerfile
docker push imoutochan/wetpicsrebirth:$version
docker push imoutochan/wetpicsrebirth:latest

cd ..

pause