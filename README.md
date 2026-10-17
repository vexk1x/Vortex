# About Vortex

Vortex is a Biome/Aura Detector (Whatever you call it) for the Roblox Game "Sol's Rng" (Not officialy supported!).
(And yes it does support multiple accounts & instances running at the same time).

## Features
Vortex contains features like:

* AntiAfk
> Prevents you from being sent into what's known as a "Ghost PS" by pressing space in each Roblox Window every 15 minutes.

* Aura Notifications
> Sends Details about you currently equipped aura (Name, Rarity, from what biome you got it and Native Rarity is supported aswell)

* Minimum Ping Rarity
> Won't send Notifications or Ping you for Auras with less than a Rarity of which you have selected, you can set the minimum rarity like this: 300,000,000 300.000.000 300_000_000 or the classic 300000000. (Pings require you to have the 'Ping User' Check Box checked and having entered your discord user id)

* Custom Webhook Pings for Rare Biomes
> Vortex ONLY pings if you get a rare biome to prevent unwanted accidents of pinging everyone for a Rainy. You can choose between Role ID, User ID, @Everyone and just no pings to let the biome forwarder handle it for you.

* Treat Singularity as a Rare Biome
> Before Selecting this, speak with your members or server admin about having this option selected, as the name says it would treat singularity as a rare biome and therefore also send pings.

* Include Username
> Includes your Username on Webhook messages for Rolled Auras / Biomes you have gotten.

* Clickable Discord Server and Tutorial Video
> Located inside the 'Info' Tab, you can find our discord server and a tutorial video on the macro made by Fiko!

* Logs
> Located inside the 'Logs' Tab, you can find Macro logs with timestamps. It logs when it started, stopped, auras an account has rolled, biomes you have gotten and if a webhook was sent, great for locating bugs if you encountered any (please report it then)

* Stats
> Stats of how many of each Biomes you have gotten while using the Macro (total) and also a few smaller fields like Biomes this session, rare biomes this session, globals rolled (total and session), Biomes found (Total, session).

* Accounts
> Located inside the 'Accounts' Tab you can find a list of all of your added Accounts, wether they are active or not and in which Biome they currently are.

* Custom UI
> Customizably UI where you can select Themes and Fonts. (Very experimental and may break the Ui slightly...)

## How to Download or get the Macro

For this, You have 2 Main options, either

Download the latest (recommended) Version from [here](https://github.com/vexk1x/Vortex/releases)

or

clone/download this repository and cd into the folder containing Vortex.slnx and run:

```
dotnet publish "Vortex\Vortex.slnx" -c Release -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

## I found a bug and want to report it

Great, Create an issue on this github page or Direct Message me on my Discord
Discord: vexk1x

## I want to be cool

Of course, join us on discord then: https://discord.gg/gSbuSNuAAW