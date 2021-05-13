Echo "Set Version!"
Echo ".\next-patch.ps1"
Echo ".\next-minor.ps1"

pause

Echo "Cleaning release folder"
rm -r release\
mkdir release\
mkdir release\ChiaPlotStatus\


Echo "Building linux deb and rpm packages"
cd ChiaPlotStatusCLI
dotnet publish -c Release -f net5.0 -r linux-x64
cd ..\ChiaPlotStatusGUI
dotnet publish -c Release -f net5.0 -r linux-x64
cp ..\ChiaPlotStatusCLI\bin\Release\net5.0\linux-x64\publish\ChiaPlotStatusCLI* .\bin\Release\net5.0\linux-x64\publish\
dotnet rpm -r linux-x64 -f net5.0 -c Release
cp bin\Release\net5.0\linux-x64\ChiaPlotStatus.*.linux-x64.rpm ..\release\ChiaPlotStatus.linux-x64.rpm
dotnet deb -r linux-x64 -f net5.0 -c Release
cp bin\Release\net5.0\linux-x64\ChiaPlotStatus.*.linux-x64.deb ..\release\ChiaPlotStatus.linux-x64.deb
cd ..


# Echo "Building for MacOS"
# dotnet publish -r osx-x64 --configuration .\ChiaPlotStatus.sln /p:Configuration=Release /p:Platform="Any CPU"
# dotnet pkg -r osx-x64 -f net5.0 -c Release


Echo "Building for Windows"
dotnet publish -r win-x64 --configuration .\ChiaPlotStatus.sln /p:Configuration=Release /p:Platform="Any CPU"


Echo ""
Echo "The warnings are normal. Only formal flaws in the code"
Echo ""


Echo "Copying Windows build to release folder"
xcopy /sy C:\Users\mk\IdeaProjects\ChiaPlotStatus\ChiaPlotStatusLib\bin\Release\net5.0\win-x64\publish\* release\ChiaPlotStatus\
xcopy /sy C:\Users\mk\IdeaProjects\ChiaPlotStatus\ChiaPlotStatusGUI\bin\Release\net5.0\win-x64\publish\* release\ChiaPlotStatus\
xcopy /sy C:\Users\mk\IdeaProjects\ChiaPlotStatus\ChiaPlotStatusCli\bin\Release\net5.0\win-x64\publish\* release\ChiaPlotStatus\

Echo "Now open and build setup using InstallForge and InstallerConfig.ifp as config"
