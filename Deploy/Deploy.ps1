$root = ".\Source"

Remove-Item -Path $root\WetPicsRebirth\bin -Recurse
Remove-Item -Path $root\WetPicsRebirth\obj -Recurse

$version = dotnet-gitversion /output json /showvariable MajorMinorPatch
$version
$tag = "imoutochan/wetpicsrebirth:" + $version
$tag

cd $root
docker build --tag=$tag -t imoutochan/wetpicsrebirth . -f WetPicsRebirth\Dockerfile
docker push imoutochan/wetpicsrebirth:$version
docker push imoutochan/wetpicsrebirth:latest

cd ..

pause