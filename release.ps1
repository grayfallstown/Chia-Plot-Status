Echo "Set Version!"
pause

dotnet build --configuration .\ChiaPlotStatus.sln /p:Configuration=Release /p:Platform="Any CPU"

Echo ""
Echo "The warnings are normal. Only formal flaws in the code"
Echo ""

Echo "Cleaning release folder"

rm -r release\
mkdir release\
mkdir release\ChiaPlotStatus\

Echo "Copying build to release folder"
xcopy /y C:\Users\mk\IdeaProjects\ChiaPlotStatus\ChiaPlotStatusLib\bin\Release\net5.0\* release\ChiaPlotStatus\
xcopy /y C:\Users\mk\IdeaProjects\ChiaPlotStatus\ChiaPlotStatusGUI\bin\Release\net5.0\* release\ChiaPlotStatus\
xcopy /y C:\Users\mk\IdeaProjects\ChiaPlotStatus\ChiaPlotStatusCli\bin\Release\net5.0\* release\ChiaPlotStatus\

Echo "Now open and build setup using InstallForge and InstallerConfig.ifp as config"
