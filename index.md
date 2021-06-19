![GitHub all releases](https://img.shields.io/github/downloads/grayfallstown/Chia-Plot-Status/total)
![GitHub Workflow Status (branch)](https://img.shields.io/github/workflow/status/grayfallstown/Chia-Plot-Status/.NET/main)
![GitHub](https://img.shields.io/github/license/grayfallstown/Chia-Plot-Status)
![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/grayfallstown/Chia-Plot-Status?color=green&include_prereleases)
![GitHub last commit](https://img.shields.io/github/last-commit/grayfallstown/Chia-Plot-Status)
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate?hosted_button_id=PDLLVF5XVMJPC)
[![Liberapay](https://img.shields.io/liberapay/patrons/grayfallstown?label=Liberapay)](https://liberapay.com/grayfallstown/donate)

<p align="center">
  <img width="200" alt="Chia Plot Status Log" src="./Icon%20-%20Color%20changed.svg">
</p>


![Screenshot](./Screenshots/Screenshot.jpg)
![Screenshot](./Screenshots/Screenshot-Dark.jpg)


## Features

 - Monitor Progress of running plots
 - Show estimated time to completion based on your already finished plots best matching your current plot config
 - Monitor Health of plotting processes
 - Already compatible with madMAx43v3r/chia-plotter (currently getting improved)
 - Compatible with all plotters and plotting managers that use or are based on the official chia plotter (see Troubleshooting if something does not work from the get go)
 - Show important information from log file in easy to read table
 - Multiple folders with log files supported (multiple plotting rigs, anyone?)
 - Multiple plots per log file supported (plot create --num n)
 - Export of readable or raw data to Json, Yaml and CSV


## See Chia Plot Status in action:

### On Upside Down Crypto (YouTube):

<a href="http://www.youtube.com/watch?feature=player_embedded&v=lK0o3KyGFW8" target="_blank">
 <img src="http://img.youtube.com/vi/lK0o3KyGFW8/hqdefault.jpg" alt="Watch the video" />
</a>


### On Patro TV (YouTube):

<a href="http://www.youtube.com/watch?feature=player_embedded&v=JLVhG86-4UI" target="_blank">
 <img src="http://img.youtube.com/vi/JLVhG86-4UI/hqdefault.jpg" alt="Watch the video" />
</a>

## How it works

Chia Plot Status observes the log folders you give it to monitor which can be local or connected via network. By this it supports monitoring multiple plotting rigs as you can access them on your desktop even if your plotting rigs are headless. It regulary checks for new Log files in those folders and analyses them.

On basis of finished plots it builds a local statistic (on your machine, no data is send anywhere or pulled from any cloud) and uses them to calculate ETA / time remaining and warning thresholds for the Health of your plotting processes.


## Working with many distributed plotting rigs

**Recommended way:** Use sshfs (with [sshfs-win](https://github.com/billziss-gh/sshfs-win) for Windows) to securely mount the log dirs of your plotting rigs on your desktop via highly encrypted network connection, where it is your desktop that initiates the mount. This can be set up so that the desktop can only access the log dirs and only has read access. Even if you use remote plotting rigs that you access over the internet this is the most secure way and you most likely access your remote servers via ssh already.

Other Options: Mount the log folders of all rigs as network shares (via samba on linux) if all your plotting rigs are in the local network or connected via VPN. Another way would be to make a cronjob on your plotting rigs that uses scp or rsync in append mode to copy the log dir to your desktop where you run Chia Plot Status, but if you can manage to set this up you should set up sshfs instead. Last, least preferred option: collect them with cloud apps like Google Drive (Chia Plot Status does not talk to any cloud services for you, you have to install those apps and mount your log folders in them yourself if you want to use them).


**Best Practice:**
- Only delete log files of finished plots if your hardware or the way you plot has significantly changed. Chia Plot Status uses finished plots to calculate ETA/Time Remaining as well as warning/error thresholds. If you delete finished log files the quality of those values decreases significantly.
- Use SSHFS to access the log directories of your plotting rigs
- Each plotting rig should have its own log folder, so they don't mix and mess up estimates and warning thresholds for each other.
- Always log locally. If you log directly to a network share / NAS your plotting processes will crash if the connection becomes flaky. Prefer connecting your host machine (which runs Chia Plot Status) to networkshares on the plotting rigs, not the other way around.


## Security / Trustworthiness

See [a reddit comment made by the Chia Plot Status Core Developer, summarized in the following:](https://www.reddit.com/r/chia/comments/nlmwk7/safety_of_chiabot_from_joaquimguimaraes_on_github/gzn4xu3/?utm_source=reddit&utm_medium=web2x&context=3)

### There are multiple attack vectors to consider:

##### 1. The possibility that the core developer (me) is or becomes malicious

There is a saying: Where is a money trail, there is a way to sue/prosecute someone.

Chia Plot Status has buttons to donate via paypal both in the application itself and on the website.

Should the core developer (me) turn malicious, people could easily sue the core developer (me) and by that get the necessary details as in full name, address and day of birth, IBAN/Bic, everything from paypal.

If the core developer (me) becomes malicious this would be basically a how to get imprisoned speedrun (any %)

Even if you think you would not sue the core developer as he (me) might sit in a different country (germany), someone will as the Chia Plot Status Github Repository has between 2k to 4k visits daily and currently 24k downloads.

This should be more than enough to deter the core developer (me) from doing anything malicious.

#### 2. The core developer (me) merges a pull request (code changes made by someone else) which contains malicious code without noticing

As seen on [https://github.com/grayfallstown/Chia-Plot-Status/graphs/contributors](https://github.com/grayfallstown/Chia-Plot-Status/graphs/contributors) there is only one other person who contributed a pull request so far and that wasn't code but a documentation change.

The core developer (me) will check each pull request before merging as he (me) would have to run the code himself to check if the application works properly after merging that pull request and by that he (I) would get attacked by any malicious code that was contained in that pull request.

#### 3. External Dependencies (as in libraries / code written by someone else) the application uses to do certain things (like to create the graphical user interface) become malicious.

Well, this is a tough one as even the core developer (me) has very little means to check external binaries for malicious code. The core developer (me) and every other developer using those libraries will get attacked by any malicious code in those libraries before they (we) distribute a new version of their (our) software containing that library to the users of their (our) softwares, as they (we) generally test their (our) applications before each release.

The core developer (me) takes the following precautions to mitigate that risk:

- External dependencies are kept at a minimum to reduce this attack vector (chia-blockchain devs do the same)

- Every release build is build on the same system and previously downloaded dependencies are never deleted/redownloaded. This prevents pulling in malicious code if the external dependency version used gets replaced with malicious code. But it also prevents  reproducible builds that everyone can follow and reproduce step by step on their system, if the external dependency version actually does get changed. Well, this should raise concern anyway and in any case.

- Updating Dependencies (external libraries / code written by someone else) is delayed (possibly indefinitely) until an update is required to implement a feature or to fix a bug. This gives anti virus providers time to determine if that library version is malicious, which would prevent an update.


## Installation / Download

![GitHub all releases](https://img.shields.io/github/downloads/grayfallstown/Chia-Plot-Status/total)

Windows: [Download latest version](https://github.com/grayfallstown/Chia-Plot-Status/releases/latest/download/Setup.exe)
You will get a blue warning saying this was published by an unknown developer.

Linux: First install [dotnet 5.0 SDK](https://dotnet.microsoft.com/download/dotnet/5.0), then either the Chia Plot Status [deb](https://github.com/grayfallstown/Chia-Plot-Status/releases/latest/download/ChiaPlotStatus.linux-x64.deb) or [rpm](https://github.com/grayfallstown/Chia-Plot-Status/releases/latest/download/ChiaPlotStatus.linux-x64.rpm) package depending on your linux distribution (deb for ubuntu)

For Mac you currently have to [build it](#Build-it-).

## Getting Log Files from PowerShell

```
$Temp1="D:\PlotTemp"
$Temp2="D:\PlotTemp"
...

chia.exe plots create --tmp_dir "$TEMP1" --tmp2_dir "$TEMP2" [and so on] 2>&1 | % ToString | Tee-Object -FilePath "C:\Users\$USERNAME\.chia\mainnet\plotter\$([GUID]::NewGUID().ToString('D')).log"
```

The last part with `2>&1 | % ToString | Tee-Object` writes the log to the PowerShell and to a file with a unique name for each plotting process.

You can download a [full example script with Tee-Object](https://gist.github.com/grayfallstown/8530acb84eb131d3dae074e4be23badb) as well.


## Getting Log Files from madMAx43v3r/chia-plotter

On Windows without WSL:

Take the same command you are currently using and just add ` 2>&1 | % ToString | Tee-Object -FilePath "C:\Users\$env:UserName\.chia\mainnet\plotter\$([GUID]::NewGUID().ToString('D')).log"` at the end and run it in PowerShell, not CMD.
Note: there must be a whitespace between your command and this and there is nothing to be replaced in this line. Just leave it as it is.

On Windows with WSL:

```
# make sure you have got uuid installed (sudo apt install uuid -y)
# use 'chia keys show' to get this keys:
export POOLKEY="replace-me"; \
export FARMERKEY="replace-me"; \
export WINDOWS_USERNAME="replace-me" \
export THREADS="$(expr $(nproc) / 2)"; \
/home/mk/chia-plotter/build2/chia_plot \
--poolkey=$POOLKEY \
--farmerkey=$FARMERKEY \
--tmpdir2=replace-me \
--tmpdir=replace-me \
--finaldir=replace-me/ \
--count=-1 \
--threads=$THREADS \
--buckets=7 \
 2>&1  | tee /mnt/c/Users/$WINDOWS_USERNAME/.chia/mainnet/plotter/chia-plotter-$(uuid).log
```
Note: There is nothing to be replaced in the last line. Just leave it as it is.


On Linux directly:

```
# make sure you have got uuid installed (sudo apt install uuid -y)
# use 'chia keys show' to get this keys:
export POOLKEY="replace-me"; \
export FARMERKEY="replace-me"; \
export THREADS="$(expr $(nproc) / 2)"; \
/home/mk/chia-plotter/build2/chia_plot \
--poolkey=$POOLKEY \
--farmerkey=$FARMERKEY \
--tmpdir2=replace-me \
--tmpdir=replace-me \
--finaldir=replace-me/ \
--count=-1 \
--threads=$THREADS \
--buckets=7 \
 2>&1 | tee /home/$(whoami)/.chia/mainnet/plotter/chia-plotter-$(uuid).log;
```

Note: There is nothing to be replaced in the last line. Just leave it as it is.


## Need the columns in a different order?

See https://github.com/grayfallstown/Chia-Plot-Status/issues/36#issuecomment-843351280


## Troubleshooting

If you use Cloud Sync Services, rsync/scp cronjobs or tools like Syncthing to collect your log files you might run into an issue with the files not properly syncing. Sonething like `The process cannot access the file because it is being used by another process.`. See [Issue #40](https://github.com/grayfallstown/Chia-Plot-Status/issues/40#issuecomment-841025993) for how to fix that, or even better, use sshfs instead.

The same works if you use harry plotter as plotting manager.

If Chia Plot Status does no longer start, try renaming `ChiaPlotStatu.config.json` to `ChiaPlotStatu.config.json.backup`. The file is located in your home directory at `C:\Users\<your username>\ChiaPlotStatu.config.json` on windows, `/home/<your username>/ChiaPlotStatu.config.json` on linux and `<your user profile directory>/ChiaPlotStatu.config.json` on mac.

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

Note: Write your tools or home automation in a way that new fields/properties/columns added to the exported files do not crash it.


## Open Source

MIT opensource licence, free to keep or change.


## Build it yourself

This **should** work on x86_64bit, x86_32bit, ARM 64bit and ARM 32 bit Systems. If not, open an [Issue](https://github.com/grayfallstown/Chia-Plot-Status/issues/new) to tell me whats wrong.

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


## Donations

PayPal: https://www.paypal.com/donate?hosted_button_id=PDLLVF5XVMJPC

Bitcoin/BTC bc1qy2fvr0js9xunlcgndlz9m9yu2qrydtlnlh4fgm

Etherium/ETH 0x0e07b5A73F571a98bAf19Fc42EBDE15d6B1664f0

Chia/XCH xch15p8swrrdt5ujv0dxy4hwjrvpjseyvuquwtfwnrjhxqt6ws9uf90qzq4axl

Tether/USDT 0x0e07b5A73F571a98bAf19Fc42EBDE15d6B1664f0

Binance Coin/BNB bnb1pes46l2jkw8dd9tw6j7sc4r9muzv5kuepnal0r

Cardano/ADA addr1q86sat0hpuvg5mp85qwke5kk6rjf6yntvr7epaz8j3k2g784p6klwrcc3fkz0gqadnfdd58yn5fxkc8ajr6y09rv53uqzjzv2r

XRP r9Wrpa2JsHh74nKqmcjJduy9GXDgmX44ic

USD Coin/USDC 0x0e07b5A73F571a98bAf19Fc42EBDE15d6B1664f0

Polkadot/DOT 16Mp3siK7qFJ9567R6ynPMNf3W9SYcumgUMXacMkH9UC7817

Uniswap/UNI 0x0e07b5A73F571a98bAf19Fc42EBDE15d6B1664f0

Litecoin/LTC LdbWCtQu7L9J9sYeDcp3M1qTWyA6Acck28

Bitcoin Cash/BCH qrxfzf3dact3crqu8l8uvxdp639scekw9qn20akrdc

Chainlink/LINK 0x0e07b5A73F571a98bAf19Fc42EBDE15d6B1664f0

Stellar/XLM GAIEQ6N7V77WLDYNFBESCGONNURKGCQZ2GS6THEENOWUSBVHQBBFL4XJ

VeChain/VET 0xBf8AC5799333a8B14A51228Be9E02629e034A039

Tron/TRX TKMnrH4P2L4PigJVNijZz9BQiSewrABosc

Dai/DAI 0x0e07b5A73F571a98bAf19Fc42EBDE15d6B1664f0

Neo/NEO ANZiysMZZS3PgzKpqTs8jATRcrpujZTBsB

Tezos/XTZ tz1XhtmaVr6RnqbTGPmPvD1XgsK7kHt8rUuH

Cosmos/ATOM cosmos1mu89q07xv9m8furg06f7tsw3u32da553wh0uv2

NEM/XEM NDM7SU2CVAN6CNOEFRMPPOIZSXQVYHIQ3BVJGH7T

0x/ZRX 0x0e07b5A73F571a98bAf19Fc42EBDE15d6B1664f0

Monero/XMR 43RbqKt37UUgY62ow4N3ptTT263zK1sfC88szAhThihU6bQKeURF3TrYLDumSak5gkX8Bj2FNzeWiduoEcPjLppHHSoBQi5

Etherium Classic/ETC 0x9347727443e8808c14062A7Fb95625B0284F2F8d



## Special Thanks

- [@charlie](https://freeicons.io/profile/740) on [freeicons.io](https://freeicons.io) for the Logo [(details)](https://github.com/grayfallstown/Chia-Plot-Status/blob/main/Logo/Icon%20-%20Readme.txt)
- @ใครๆก็ทําได้ DiY
- @Alpha One
- @Çağlar
- @Cuello
- @DazEB2
- @DjMuffin_top
- @Dujapan
- @Gridjump
- @@getchiaplot
- @Hellfall1
- @Jazeon
- @Jonesyj83
- @JoseAngelB
- @KJP Gaming
- @Lucky_Length2676
- @Lyushen
- @Manic!
- @Mr pq
- @Ok-Studio5311
- @Oguzhan Oda
- @Patro TV
- @R3htribution
- @RaySmalley
- @RedxLus
- @SERVAK
- @TormodSan
- @Upside Down Crypto
- @Waloumi
- @WeAreNotAngels
- @Worldly-Mind3108
- @Zubon102
- @aDilly
- @badi95
- @bathrobehero
- @bestq8.com
- @blood5322
- @buettgenbach
- @c-pool
- @carfnann
- @chefsas
- @chiaxch
- @cyperbg
- @darkfriend77
- @djdookie81
- @dorofino
- @douwebusschops
- @dvlzgrmz
- @j.spracher
- @jcmarco
- @jimshank
- @johnamtl
- @jonnnny
- @kata32
- @littleneko
- @magallanesrafa
- @magnusmyklebost
- @massimo de rovere
- @mmoingame
- @ouoam
- @ozulu
- @puperinoo
- @raf-cr
- @revlisoft
- @rsegrest77
- @rul3s
- @sirdeekay
- @tajchert
- @tiberiu puscas
- @toddouimet
- @whitetechnologies
- @Vera Toro
- @whoismos3s
- @wild9
- @wizbowes
- @z.ostroff
- @zeroarst
- @The Malware Analysts of Microsoft and Malwarebytes for checking Chia Plot Status after every false positive

For contributing to Chia Plot Status either by [donating](https://www.paypal.com/donate?hosted_button_id=PDLLVF5XVMJPC) or otherwise.
