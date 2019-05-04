using System.Collections.Generic;
using System.Threading.Tasks;
using BeerO.SlackConnector.Models;

namespace BeerO.SlackConnector.Connections.Clients.Chat
{
    internal interface IChatClient
    {
        Task PostMessage(string slackKey, string channel, string text, IList<SlackAttachment> attachments);
    }
}