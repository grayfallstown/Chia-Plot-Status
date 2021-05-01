# dotnet build -c Release
rm -r release\
mkdir release\
mkdir release\ChiaPlotStatus\

copy -r C:\Users\mk\IdeaProjects\ChiaPlotStatus\bin\Release\net5.0\* release\ChiaPlotStatus\
rm release\ChiaPlotStatus\ChiaPlotStatus.dll.config.json
Compress-Archive release\ChiaPlotStatus release\ChiaPlotStatus.zip
