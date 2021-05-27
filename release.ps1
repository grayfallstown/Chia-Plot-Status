Echo "Set Version!"
Echo ".\next-patch.ps1"
Echo ".\next-minor.ps1"

pause

Echo "Killing running ChiaPlotStatus.exe"
taskkill /IM "ChiaPlotStatus.exe" /F
taskkill /IM "ChiaPlotStatus.exe" /F

Echo "Cleaning release folder"
rm -r release\
mkdir release\
mkdir release\ChiaPlotStatus\

rm -r ChiaPlotStatusGUI\bin
rm -r ChiaPlotStatusCLI\bin
rm -r ChiaPlotStatusLIB\bin

Echo "Building linux deb and rpm packages"
cd ChiaPlotStatusGUI
dotnet deb -r linux-x64 -f net5.0 -c Release
dotnet rpm -r linux-x64 -f net5.0 -c Release
# dotnet publish -c Release -f net5.0 -r ubuntu.16.04-x64
# cd ..\ChiaPlotStatusGUI
# dotnet deb -r ubuntu.16.04-x64 -f net5.0 -c Release
# cp ..\ChiaPlotStatusCLI\bin\Release\net5.0\linux-x64\publish\ChiaPlotStatusCLI* .\bin\Release\net5.0\ubuntu.16.04-x64\publish\
# dotnet rpm -r ubuntu.16.04-x64 -f net5.0 -c Release
# cp bin\Release\net5.0\ubuntu.16.04-x64\ChiaPlotStatus.*.ubuntu.16.04-x64.rpm ..\release\ChiaPlotStatus.linux-x64.rpm
# dotnet deb -r ubuntu.16.04-x64 -f net5.0 -c Release
# dotnet deb -c Release
# cp bin\Release\net5.0\ChiaPlotStatus.*.deb ..\release\ChiaPlotStatus.linux-x64.deb
cp bin\Release\net5.0\linux-x64\ChiaPlotStatus*.deb ..\release\ChiaPlotStatus.linux-x64.deb
cp bin\Release\net5.0\linux-x64\ChiaPlotStatus*.rpm ..\release\ChiaPlotStatus.linux-x64.rpm
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
