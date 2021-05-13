Echo "Set Version!"
Echo ".\next-patch.ps1"
Echo ".\next-minor.ps1"

pause

Echo "Cleaning release folder"
rm -r release\
mkdir release\
mkdir release\ChiaPlotStatus\


Echo "Building linux deb and rpm packages"
rm -r ChiaPlotStatusCLI\bin
rm -r ChiaPlotStatusGUI\bin
cd ChiaPlotStatusCLI
dotnet publish -c Release -f net5.0 -r linux-x64 -p:Version=0.9.7
cd ..\ChiaPlotStatusGUI
dotnet publish -c Release -f net5.0 -r linux-x64 -p:Version=0.9.7
cp ..\ChiaPlotStatusCLI\bin\Release\net5.0\linux-x64\publish\ChiaPlotStatusCLI* .\bin\Release\net5.0\linux-x64\publish\
dotnet rpm -r linux-x64 -f net5.0 -c Release
cp bin\Release\net5.0\linux-x64\ChiaPlotStatus.1.0.0.linux-x64.rpm ..\release\
dotnet deb -r linux-x64 -f net5.0 -c Release
cp bin\Release\net5.0\linux-x64\ChiaPlotStatus.1.0.0.linux-x64.deb ..\release\


Echo "Building for Windows"
dotnet publish -r win-x64 --configuration .\ChiaPlotStatus.sln /p:Configuration=Release /p:Platform="Any CPU" -p:Version=0.9.7


Echo ""
Echo "The warnings are normal. Only formal flaws in the code"
Echo ""


Echo "Copying Windows build to release folder"
xcopy /sy C:\Users\mk\IdeaProjects\ChiaPlotStatus\ChiaPlotStatusLib\bin\Release\net5.0\win-x64\publish\* release\ChiaPlotStatus\
xcopy /sy C:\Users\mk\IdeaProjects\ChiaPlotStatus\ChiaPlotStatusGUI\bin\Release\net5.0\win-x64\publish\* release\ChiaPlotStatus\
xcopy /sy C:\Users\mk\IdeaProjects\ChiaPlotStatus\ChiaPlotStatusCli\bin\Release\net5.0\win-x64\publish\* release\ChiaPlotStatus\

Echo "Now open and build setup using InstallForge and InstallerConfig.ifp as config"
