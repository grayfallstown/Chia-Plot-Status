<p align="center">
  <img width="200" alt="Chia Plot Status Log" src="./Icon%20-%20Color%20changed.svg">
</p>

# [Chia](https://www.chia.net/) Plot Status

![Screenshot](./Screenshot.jpg)
![Screenshot](./Screenshot-Dark.jpg)


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

Windows: [Download latest version busy-plotter](https://github.com/grayfallstown/Chia-Plot-Status/releases/download/0.9.6-beta4/Setup.exe)

Mac and Linux support coming soon.

## Working with many distributed plotting rigs

Either mount the log folders of all rigs as network shares or collect them by your favorite means like Google Drive or something similar (Chia Plot Status does not talk to any cloud services for you, you have to install those apps and mount your log folders in them yourself if you want to use them)

Best Practice:
- Each plotting rig should have its own log folder, so they don't mix and mess up estimates and warning thresholds for each other.
- Always log locally. If you log directly to a network share / NAS your plotting can crash if the connection becomes flaky. Prefer connecting your host machine to networkshares on the plotters, not the other way around or use your own cloud solutions which offer local folders (should be pretty much all of them).

## Open Source

MIT opensource licence, free to keep or change.

## Thanks to

[charlie](https://freeicons.io/profile/740) on [freeicons.io](https://freeicons.io) for the Logo [(details)](https://github.com/grayfallstown/Chia-Plot-Status/blob/main/Icon%20-%20Readme.txt)
