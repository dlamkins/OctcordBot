using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Octokit;

namespace OctcordBot.Services {
    public class ReactHandlerService {

        public const string EMOTE_ACK     = "emote_ack";
        public const string EMOTE_FEATURE = "emote_feature";
        public const string EMOTE_BUG     = "emote_bug";

        private readonly DiscordSocketClient _discord;
        private readonly IConfigurationRoot  _config;
        private readonly GitHubService       _github;

        public ReactHandlerService(DiscordSocketClient discord, IConfigurationRoot config, GitHubService github) {
            _discord = discord;
            _config  = config;
            _github  = github;

            _discord.ReactionAdded += OnReactionAdded;

            UpdateEmotes();
        }

        private Emote _ack;
        private Emote _feature;
        private Emote _bug;

        public void UpdateEmotes() {
            if (_config[EMOTE_ACK]     != null && Emote.TryParse(_config[EMOTE_ACK],     out var ack)) _ack         = ack;
            if (_config[EMOTE_FEATURE] != null && Emote.TryParse(_config[EMOTE_FEATURE], out var feature)) _feature = feature;
            if (_config[EMOTE_BUG]     != null && Emote.TryParse(_config[EMOTE_BUG],     out var bug)) _bug         = bug;
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3) {
            async Task RevokeReaction(IUserMessage message, SocketUser culpritUser) {
                await message.RemoveReactionAsync(arg3.Emote, culpritUser);
            }

            if (Equals(arg3.Emote, _ack)) {
                var culpritUser = _discord.GetUser(arg3.User.Value.Id);

                // We're the only one allowed to do that one!
                if (culpritUser.Id != _discord.CurrentUser.Id) {
                    await RevokeReaction(await arg1.GetOrDownloadAsync(), culpritUser);
                }
            } else if (Equals(arg3.Emote, _feature)) {
                var message     = await arg1.GetOrDownloadAsync();
                var culpritUser = _discord.GetUser(arg3.User.Value.Id);

                if (_config["approver"] == culpritUser.Id.ToString()) {
                    var issue = await _github.CreateIssue(message,
                                                          $"{message.Author.Username}'s Feature Request [{message.Id}]",
                                                          "feature");

                    await HandleAck(issue, message, GitHubService.ItemType.Feature);
                } else {
                    await RevokeReaction(message, culpritUser);
                }
            } else if (Equals(arg3.Emote, _bug)) {
                var message     = await arg1.GetOrDownloadAsync();
                var culpritUser = _discord.GetUser(arg3.User.Value.Id);

                if (_config["approver"] == culpritUser.Id.ToString()) {
                    var issue = await _github.CreateIssue(message,
                                                          $"{message.Author.Username}'s Bug Report [{message.Id}]",
                                                          "bug");

                    await HandleAck(issue, message, GitHubService.ItemType.Bug);
                } else {
                    await RevokeReaction(message, culpritUser);
                }
            }
        }

        private async Task HandleAck(Issue issue, IUserMessage message, GitHubService.ItemType type) {
            var eb = new EmbedBuilder();

            eb.WithTitle($"[{issue.Number}] {issue.Title}");
            eb.WithAuthor(issue.User.Login, issue.User.AvatarUrl, issue.User.HtmlUrl);
            eb.WithUrl(issue.HtmlUrl);
            eb.WithDescription(StringUtil.TruncateLength(issue.Body, 256) + "...");

            eb.WithColor(type == GitHubService.ItemType.Feature ? Color.Blue : Color.Red);
            
            eb.WithTimestamp(issue.CreatedAt);
            eb.WithFooter($"{_config["github_owner"]}/{_config["github_repo"]}");

            // ack the source message in Discord
            // TODO: This is throwing an exception - fix it
            // await message.AddReactionAsync(_ack);

            string typeText = type == GitHubService.ItemType.Feature ? "feature request" : "bug report";

            await message.Channel.SendMessageAsync($"Thanks {message.Author.Mention}! :thumbsup:\r\n\r\nAn issue was opened on GitHub referencing your {typeText}! You can track it with the link below.", false, eb.Build());
        }

    }
}
