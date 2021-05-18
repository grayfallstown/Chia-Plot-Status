![GitHub all releases](https://img.shields.io/github/downloads/grayfallstown/Chia-Plot-Status/total)
![GitHub Workflow Status (branch)](https://img.shields.io/github/workflow/status/grayfallstown/Chia-Plot-Status/.NET/main)
![GitHub](https://img.shields.io/github/license/grayfallstown/Chia-Plot-Status)
![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/grayfallstown/Chia-Plot-Status?color=green&include_prereleases)
![GitHub last commit](https://img.shields.io/github/last-commit/grayfallstown/Chia-Plot-Status)
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate?hosted_button_id=PDLLVF5XVMJPC)
[![Liberapay](https://img.shields.io/liberapay/patrons/grayfallstown?label=Liberapay)](https://liberapay.com/grayfallstown/donate)

<p align="center">
  <img width="200" alt="Chia Plot Status Log" src="./Logo/Icon%20-%20Color%20changed.svg">
</p>

# [Chia](https://www.chia.net/) Plot Status

Tool to Monitor and Analyse Chia Plotting log files, show health and progress of running plots and estimated time to completion

![Screenshot](./Screenshots/Screenshot.jpg)
![Screenshot](./Screenshots/Screenshot-Dark.jpg)

## Features

 - Monitor Progress of running plots
 - Show estimated time to completion based on your already finished plots best matching your current plot config
 - Monitor Health of plot processes
 - Show important information from log file in easy to read table
 - Multiple folders with log files supported (multiple plotting rigs, anyone?)
 - Multiple plots per log file supported (plot create --num n)
 - Export of readable or raw data to Json, Yaml and CSV

## How it works

Chia Plot Status observes the log folders you give it to monitor which can be local, on a networkshare or mounted from a cloud app like Google Drive (Chia Plot Status does not talk to any cloud services for you, you have to install those apps and mount your log folders in them yourself if you want to use them). By this it supports monitoring multiple plotting rigs as you can access them via a networkshare or by similar means. It regulary checks for new Log files in those folders and analyses them.

On basis of finished plots it builds a local statistic (on your machine, no data is send anywhere or pulled from any cloud) and uses them to calculate ETA / time remaining and warning thresholds for the Health of your plotting processes.


## Installation / Download

![GitHub all releases](https://img.shields.io/github/downloads/grayfallstown/Chia-Plot-Status/total)

**ATTENTION, important notice:** Chia Plot Status uses an external library to provide a graphical user interface that runs on Windows, Linux and MacOS called [AvaloniaUI](https://avaloniaui.net/). Avalonia currently gets flagged as Trojan by Windows Defender and as far as currently known, only by Windows Defender. The file was already [deemed safe by microsoft malware analysts](https://github.com/grayfallstown/Chia-Plot-Status/issues/50#issuecomment-842849470), but then got [flagged again](https://github.com/grayfallstown/Chia-Plot-Status/issues/50#issuecomment-843005699). I am currently trying to resolve this issue, but feel free to postpone the installation of Chia Plot Status until the issue is resolved.

Windows: [Download latest version](https://github.com/grayfallstown/Chia-Plot-Status/releases/latest/download/Setup.exe)
You will get a blue warning saying this was published by an unknown developer.

Linux: First install [dotnet 5.0 SDK](https://dotnet.microsoft.com/download/dotnet/5.0), then either the Chia Plot Status [deb](https://github.com/grayfallstown/Chia-Plot-Status/releases/latest/download/ChiaPlotStatus.linux-x64.deb) or [rpm](https://github.com/grayfallstown/Chia-Plot-Status/releases/latest/download/ChiaPlotStatus.linux-x64.rpm) package depending on your linux distribution (deb for ubuntu)

For Mac you currently have to [build it yourself](#Build-it-yourself).

## Getting Log Files from PowerShell

```
$Temp1="D:\PlotTemp"
$Temp2="D:\PlotTemp"
...

chia.exe plots create --tmp_dir "$TEMP1" --tmp2_dir "$TEMP2" [and so on] | Tee-Object -FilePath "C:\Users\$USERNAME\.chia\mainnet\plotter\$([GUID]::NewGUID().ToString('D')).log"
```

The last part with Tee-Object writes the log to the PowerShell and to a file with a unique name for each plotting process.

You can download a [full example script with Tee-Object](https://gist.github.com/grayfallstown/8530acb84eb131d3dae074e4be23badb) as well.

## Working with many distributed plotting rigs

Either mount the log folders of all rigs as network shares or collect them by your favorite means like Google Drive or something similar (Chia Plot Status does not talk to any cloud services for you, you have to install those apps and mount your log folders in them yourself if you want to use them)

Best Practice:
- Each plotting rig should have its own log folder, so they don't mix and mess up estimates and warning thresholds for each other.
- Always log locally. If you log directly to a network share / NAS your plotting can crash if the connection becomes flaky. Prefer connecting your host machine to networkshares on the plotters, not the other way around or use your own cloud solutions which offer local folders (should be pretty much all of them).
- Only delete log files of finished plots if your hardware or the way you plot has significantly changed. Chia Plot Status uses finished plots to calculate ETA/Time Remaining as well as warning/error thresholds. If you delete finished log files the quality of those values decreases significantly.

## Custom tools / Home automation

You can export plot logs to json, yaml or csv both via the gui or the console for automation:

```
"C:\Program Files (x86)\ChiaPlotStatus\ChiaPlotStatus\ChiaPlotStatusCli.exe" --help
Copyright (C) 2021 grayfallstown

  -o, --outfile            Required. The file to write to

  -f, --format             Required. The format to use while writing the file. Valid values are json, yaml and csv

  -l, --log-folders        The folders where logs can be found, comma separated. Uses default folder when empty

  -s, --sort-property      The property you want the plotlogs sorted by

  -a, --sort-asc           Sort ascending

  --search                 Filter plotlogs by this search terms. You filter for your temp1 folder for example.

  --hide-finished          Hide finished plots

  --hide-possibly-dead     Hide possibly dead plots

  --hide-confirmed-dead    Hide confirmed dead plots

  --hide-healthy           Hide healthy plots

  --keep-updating          Keep updating the file every 10 seconds

  --help                   Display this help screen.



"C:\Program Files (x86)\ChiaPlotStatus\ChiaPlotStatus\ChiaPlotStatusCli.exe" -o test.json -f json
Sorting by Progress
File 'test.json' written
```


## Troubleshooting

If you use Cloud Sync Services like Syncthing to collect your log files you might run into an issue with the files not properly syncing. Sonething like `The process cannot access the file because it is being used by another process.`. See [Issue #40](https://github.com/grayfallstown/Chia-Plot-Status/issues/40#issuecomment-841025993) for how to fix that.


## Open Source

MIT opensource licence, free to keep or change.

## Build it yourself

Download and install [dotnet 5.0 SDK](https://dotnet.microsoft.com/download/dotnet/5.0) and [git](https://git-scm.com/).

On the console, clone/download this repository:

`git clone https://github.com/grayfallstown/Chia-Plot-Status.git`

Build it:

`cd Chia-Plot-Status`

`dotnet build --configuration .\ChiaPlotStatus.sln /p:Configuration=Release /p:Platform="Any CPU"`

Chia-Plot-Status can now be started with 

windows: `.\ChiaPlotStatusGUI\bin\Release\net5.0\ChiaPlotStatus.exe` 

linux: `./ChiaPlotStatusGUI/bin/Release/net5.0/ChiaPlotStatus`

macOS: `dotnet ./ChiaPlotStatusGUI/bin/Release/net5.0/ChiaPlotStatus.dll` (thanks @mahdi-ninja)

alternatively try `dotnet run --build`.

## Thanks to

- [charlie](https://freeicons.io/profile/740) on [freeicons.io](https://freeicons.io) for the Logo [(details)](https://github.com/grayfallstown/Chia-Plot-Status/blob/main/Logo/Icon%20-%20Readme.txt)
- @Jonesyj83
- @RedxLus
- @TormodSan
- @Waloumi
- @bathrobehero
- @bestq8.com
- @carfnann
- @darkfriend77
- @dvlzgrmz
- @kata32
- @magnusmyklebost
- @ouoam
- @rul3s
- @tajchert
- @whitetechnologies
- @Vera Toro
- @wild9
