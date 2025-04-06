# Twitch Customers

Twitch Customers is a MelonLoader (formerly BepInEx) Unity mod for the relatively new but already massively popular Steam game, [Schedule I](https://store.steampowered.com/app/3164500/Schedule_I/).

Its primary focus is on Twitch streamers who wish to add an element of interactivity with their audience, by allowing Twitch chatters to appear in the game as customers **in their single player sessions**.

> [!Note]
> If you are a user that just cares about the mod and not how it was developed, feel free to skip the “How It Works” section, unless that sounds of interest to you!

## Table of Contents

* [Prerequisites](#prerequisites)
* [Getting Started](#getting-started)
* [Configuration](#configuration)
  * [`Enable mod`](#enable-mod)
  * [`Preserve Original NPC Name`](#preserve-original-npc-name)
  * [`Channel Name`](#channel-name)
  * [`Blocklisted Chatters`](#blocklisted-chatters)
  * [`Message Command`](#message-command)
  * [`Subscriber Weight`](#subscriber-weight)
  * [`Queue Size`](#queue-size)
* [Frequently Asked Questions (FAQ)](#frequently-asked-questions-faq)
  * [Does this work with multiplayer?](#does-this-work-with-multiplayer)
  * [What Unity mod loader does TwitchCustomers use?](#what-unity-mod-loader-does-twitchcustomers-use)
* [How It Works](#how-it-works)


## Prerequisites

* [Schedule I](https://store.steampowered.com/app/3164500/Schedule_I/): Yes, you need to buy the game on Steam. It’s fun, I promise! 🙂
* [MelonLoader](https://melonwiki.xyz/#/?id=requirements): The mod was developed against MelonLoader v0.7.0 for Il2Cpp games.

## Getting Started

Before installing the mod, install MelonLoader for Schedule I and run the game. This may take some time, as MelonLoader will decompile the game first. Once the game loads and the main menu appears, close the game.

Download the `TwitchCustomers.dll` and its dependency packages from our [Releases page](https://github.com/ReservedKeyword/TwitchCustomers/releases), dragging them into the `Mods` directory, located within your game’s Steam directory.

![Steam Game Location](./images/find-game-location.png)

Start Schedule I again, allowing the game *and mod* time to fully launch, before exiting the game (again) once reaching the main menu.

Proceed to the next section in this document to learn how to configure the mod!

## Configuration

The configuration file can be found in your game's root directory, where the mod was installed.

The path will look similar to `/path/to/game/UserData/MelonPreferences.cfg`, where `/path/to/game` is the path to the Schedule I game directory. (See image above on how to locate where the game was downloaded.)

There are two sections (categories) that belong to the TwitchCustomers mod:

* `TwitchCustomers_General`

* `TwitchCustomers_Integration`

All mod configuration options are as follows:

### `Enable mod`

Set to `true` if you want the mod to load when the game launches, otherwise set it to `false`.

### `Preserve Original NPC Name`

Defines if, once a contract/deal has finished, the NPC should return to its original name.

If this value is set to `true`, once a contract/deal has finished, the NPC will return back to its original character name, meaning both the character, as well as the text message conversation chain found in the in-game Messages app, given the following conditions:

* An offer expires, meaning the NPC reached out to you and you didn't respond in time.

* You explicitly rejected the offer the NPC made.

* You counter-offered the NPC, and they rejected your counter-offer after evaluation.

* The contract failed (e.g., you didn't show up and hand over any product).

* The contract was successful, meaning you successfully met with the NPC and gave them the product.

If this value is set to `false`, once a contract/deal has finished, the NPC’s character and text message conversation chain will remain the Twitch chatter’s username until the character places another contract order, at which point their name will change to reflect a new Twitch chatter.

> [!Important]
> Regardless of this setting’s value, name preservation only persists within a single session. Once the session ends, either by exiting to the main menu or fully closing the game, all NPCs will return to their original name.

### `Channel Name`

The username of the Twitch channel the mod should connect to in order to process the message command (see below).

Since the mod effectively enters the chat room as an anonymous, read-only user, there is no authentication needed.

### `Blocklisted Chatters`

A comma-separated list of chatters whose messages will be fully ignored by the mod.

Though you may add whomever you wish to this list, its original intent was to allow explicitly blocking messages from certain users, such as bots, like Stream Elements or Streamlabs.

### `Message Command`

The command that registers a Twitch chatter’s intent to be in the game. This string can be set to whatever you like, though it is recommended to be fairly unique to reduce the likelihood of unwanted messages getting caught.

As a quick aside, a Twitch chatter’s message is inspected to *contain* this phrase, meaning that it does not have to be their full message, nor does it have to begin with an exclamation mark (!).

### `Subscriber Weight`

A weight value that can be used to determine the "luck" of subscribers when getting drawn. Non-subscribers have a static weight of 1.0.

* If you would like, for example, subscribers to have 20% more luck than non-subscribers, this value should be set to `1.2`.

* If you would like subscribers to have the same luck as non-subscribers, this value should be set to `1.0`.

### `Queue Size`

The maximum number of unique chatters that should be selected from at any given time. Chatters will always be chosen at "[random](https://en.wikipedia.org/wiki/Pseudorandomness);" this value simply defines the maximum number of unique chatters you would like the mod to choose from.

## Frequently Asked Questions (FAQ)

### Does this work with multiplayer?

Maybe? I don’t know. This was developed and tested purely in single player sessions. It is likely that it will work in multiplayer, but it’s also just as equally unlikely that it’ll work in multiplayer, so **use at your own risk**!. To rephrase, **there was zero effort put in to ensuring the mod works in a multiplayer session**.

### What Unity mod loader does TwitchCustomers use?

Prior to v0.3.1, all versions used BepInEx 6 for Il2Cpp. Since v0.3.1, all versions use MelonLoader for Il2Cpp.

## How It Works

After reverse engineering Schedule I, it was immediately apparent that NPCs are not spawned in the game in a dynamic way. What this means is that each NPC has its own C# class, such as Austin, Kyle, etc. directly in the game code. Though it is likely we could create and spawn custom, generic NPCs in the game using a combination of Schedule I’s current `NPC` class and Unity, doing so is well outside the scope of this project. (This project’s scope was set to go from concept to product in just a few days.)

Instead, the mod takes advantage of the current NPCs that are already in the game, renaming them when they place a new order (also known as contracts in the game). This is done primarily in two locations:

1. The NPC’s character, which you can see when mousing over their body.

2. The NPC’s text messages, which you can see (a) in the Messages app on the in-game mobile device and (b) in the conversations screen, where you can send a reply.

### Character Name Modification

As Schedule I has two built-in properties in the `NPC` class—`FirstName` and `LastName`—the mod modifies the `FirstName` property to be that of a Twitch chatter, leaving the `LastName` property empty, effectively giving the character no last name.

### Text Message Modification

Schedule I has two distinct locations where an NPCs name is placed within the in-game Messages app: the app’s home page, where all conversations are listed with some preview text, and a conversation entry page, where messages sent to and from a specific NPC are shown. (The latter is the screen that you would use to “reply” to the NPC, scheduling a time window to complete the contract.)

**Home Page**: The NPCs name that is shown on the Messages home page is created via a `CreateUI()` method that is called, as far as I can tell, once per game session. Because of this, modifying the text to change its value to a Twitch chatter requires capturing and referencing Unity’s `RectTransform` and `Text` component, respectively, manually updating its text, rather than relying on any Schedule I’s internal game code.

**Conversations Page**: The NPCs name that is shown in a conversations window, however, is created and managed by Schedule I’s internal game code, allowing the mod to only need to modify the `contactName` property specified within a `MSGConversation`.

If you like to learn more about how some specific things work in the mod, it's open source, so feel free to poke around in this repository!