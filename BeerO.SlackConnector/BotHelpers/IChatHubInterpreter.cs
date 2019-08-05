using BeerO.SlackConnector.Models;

namespace BeerO.SlackConnector.BotHelpers
{
    public interface IChatHubInterpreter
    {
        SlackChatHub FromId(string hubId);
    }
}