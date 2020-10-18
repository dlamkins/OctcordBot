# OctcordBot

OctcordBot is a bot to help you keep up with bug reports and feature requests in your Discord channel.

At this time, it can allow up to 1 individual to "react" to messages with either a feature request or bug emote (of your choosing).  Doing so will automatically generate an issue on GitHub with the proper label and a reference back to the post in Discord.  It will then post a reply in the channel letting the user know where they can track the issue's progress.

## Setup
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fdlamkins%2FOctcordBot.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2Fdlamkins%2FOctcordBot?ref=badge_shield)


#### 1. Config Params

After launching you'll be prompted to provide the following in the console:

1. Discord Bot Token
2. Command Prefix (we'll assume you've chosen `$` in this guide)
3. Discord Admin Channel ID (⚠anyone in this channel can use the bot commands so don't use a public channel - right-click a channel and click **Copy ID** for this)
4. GitHub Token
5. GitHub Owner (github.com/**\<owner-name\>**/\<repo-name\>)
6. GitHub Repo (github.com/\<owner-name\>/**\<repo-name\>**)

These (along with all other config items) will be saved to the config.json file in the application's directory.

#### 2. Set the Emotes

In the channel you provided in step #3, run the command:

`$SetupEmote <ack_emote> <featurerequest_emote> <bugreport_emote>`

⚠ These must be emotes (custom emotes that have been uploaded to the server).  Emojis aren't currently supported.

| Emote Field           | Purpose                                                                                                                    |Example
|-----------------------|----------------------------------------------------------------------------------------------------------------------------|-------
| ack\_emote            | The emote the bot will use to acknowledge that it has created the issue \(it will react to the original post with this\)\. |<img src="https://cdn.discordapp.com/emojis/600893240929026048.png?v=1" width="24" />
| featurerequest\_emote | The emote you can react with to specify that the message was a feature request\.                                           |<img src="https://cdn.discordapp.com/emojis/601946852236984341.png?v=1" width="24" />
| bugreport\_emote      | The emote you can react with to specify that the message was a bug report\.                                                |<img src="https://cdn.discordapp.com/emojis/601946852828643328.png?v=1" width="24" />

#### 3. Set the Approver

`$SetApprover @<username>`

This is how you will set who is allowed to use the reacts to have the bot do its thing.  Currently this only supports one approver, but an update will allow you to specify as many as you want or use a role to determine this in the future.

#### 4. Optional

##### Set Game Activity

If you would like, you can set the game the bot will show as playing with the following command:

`$SetGame "<name-of-game>"`

##### Commands

All commands can be shown by running:

`$SetupHelp`

## Contributing
Pull requests are welcome. For major changes, consider opening an issue first to discuss.

## Building
For release, we've used both:

**32-bit Release**

`dotnet publish -r win-x86 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true`

**64-bit Release**

`dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true`

## License
[MIT](https://choosealicense.com/licenses/mit/)

[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fdlamkins%2FOctcordBot.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2Fdlamkins%2FOctcordBot?ref=badge_large)