using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Octokit;

namespace OctcordBot.Services {
    public class GitHubService {

        public enum ItemType {
            Feature,
            Bug
        }

        private readonly IConfigurationRoot  _config;

        public GitHubService(IConfigurationRoot config) {
            _config  = config;
        }

        public async Task<Issue> CreateIssue(IUserMessage message, string title, params string[] labels) {
            var ghClient = new GitHubClient(new ProductHeaderValue("OctcordBot"));
            ghClient.Credentials = new Credentials(_config["github_token"]);

            var newIssue = new NewIssue(title);
            foreach (string label in labels) {
                newIssue.Labels.Add(label);
            }

            var issueBody = new StringBuilder();
            issueBody.AppendLine($"**{message.Author.Username} at {message.Timestamp}**");

            using (var reader = new StringReader(message.Content)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    issueBody.AppendLine($"> {line}");
                }
            }

            issueBody.AppendLine();
            foreach (var embed in message.Embeds.Where(embeds => !string.IsNullOrEmpty(embeds.Url))) {
                issueBody.AppendLine(embed.Url);
            }

            issueBody.AppendLine();
            issueBody.AppendLine($"REF: {message.GetJumpUrl()}");

            newIssue.Body = issueBody.ToString();

            return await ghClient.Issue.Create(_config["github_owner"], _config["github_repo"], newIssue);
        }

    }
}
