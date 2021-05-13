dotnet version --no-git --patch
cd ChiaPlotStatusGUI
dotnet version --no-git --patch
cd ..\ChiaPlotStatusCli
dotnet version --no-git --patch
cd ..\ChiaPlotStatusLib
dotnet version --no-git --patch
cd ..
.\release.ps1
