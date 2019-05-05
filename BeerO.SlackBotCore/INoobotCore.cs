using System.Collections.Generic;
using System.Threading.Tasks;
using BeerO.SlackBotCore.MessagingPipeline.Response;
using BeerO.SlackConnector.Models;

namespace BeerO.SlackBotCore
{
    public interface INoobotCore
    {
        Task Connect();
        void Disconnect();
        Task MessageReceived(SlackMessage message);
        Task SendMessage(ResponseMessage responseMessage);
        string GetUserIdForUsername(string username);
        string GetUserIdForUserEmail(string email);
        string GetChannelId(string channelName);
        string GetBotUserName();
        Dictionary<string, string> ListChannels();
    }
}