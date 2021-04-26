dotnet build -c Release
rm -r release\
mkdir release\
mkdir release\ChiaPlotStatus\
copy bin\Release\net5.0-windows\ChiaPlotStatus.dll release\ChiaPlotStatus\
copy bin\Release\net5.0-windows\ChiaPlotStatus.exe release\ChiaPlotStatus\
copy bin\Release\net5.0-windows\ChiaPlotStatus.runtimeconfig.json release\ChiaPlotStatus\
copy bin\Release\net5.0-windows\Ookii.Dialogs.Wpf.dll release\ChiaPlotStatus\
Compress-Archive release\ChiaPlotStatus release\ChiaPlotStatus.zip
