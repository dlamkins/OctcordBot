using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using OctcordBot.Services;

namespace OctcordBot.Modules {
    [Name("Moderator")]
    [RequireContext(ContextType.Guild)]
    public class SetupModule : ModuleBase<SocketCommandContext> {

        private readonly IConfigurationRoot  _config;
        private readonly ReactHandlerService _reactHandler;
        private readonly ActivityService     _activity;

        public SetupModule(IConfigurationRoot config, ReactHandlerService reactHandler, ActivityService activity) {
            _config       = config;
            _reactHandler = reactHandler;
            _activity     = activity;
        }

        private string FormatAsCommand(string command) {
            return $"{_config["command_prefix"]}{command}";
        }

        [Command("SetupHelp")]
        public async Task SetupHelp() {
            if (Context.Channel.Id.ToString() != _config["bot_channel"]) return;

            await ReplyAsync("**Setup:**\r\n\r\n" +
                             $":one:  {FormatAsCommand("SetupEmotes *<ack_emote> <featurerequest_emote> <bug_emote>*")}\r\n" +
                             $":two:  {FormatAsCommand("SetApprover *<user>*")}\r\n" +
                             $":three:  {FormatAsCommand("SetGame *\"<name of game>\"*")}");
        }

        private string FormatAsEmote(Emote emote) {
            return $"<:{emote.Name}:{emote.Id}>";
        }

        [Command("SetupEmotes")]
        public async Task SetupEmotes(Emote ack, Emote feature, Emote bug) {
            if (Context.Channel.Id.ToString() != _config["bot_channel"]) return;

            if (ack == null || feature == null || bug == null) {
                await ReplyAsync("You didn't provide the correct emote params. :confused:");
                return;
            }

            _config["EMOTE_ACK"]     = FormatAsEmote(ack);
            _config["EMOTE_FEATURE"] = FormatAsEmote(feature);
            _config["EMOTE_BUG"]     = FormatAsEmote(bug);

            _reactHandler.UpdateEmotes();

            await ReplyAsync("Got it! :thumbsup:\r\n\r\n"                       +
                             $"{FormatAsEmote(ack)} => Ack\r\n"                 +
                             $"{FormatAsEmote(feature)} => Feature Request\r\n" +
                             $"{FormatAsEmote(bug)} => Bug\r\n");
        }

        [Command("SetApprover")]
        public async Task SetApprover(IUser user) {
            if (Context.Channel.Id.ToString() != _config["bot_channel"]) return;

            _config["approver"] = user.Id.ToString();

            await ReplyAsync($"Got it! Hello {user.Username}! :wave:");
        }

        [Command("SetGame")]
        public async Task SetGame(string game) {
            if (Context.Channel.Id.ToString() != _config["bot_channel"]) return;

            _config["activity_game"] = game;

            _activity.UpdateStatus();

            await ReplyAsync(":metal:");
        }

    }
}
