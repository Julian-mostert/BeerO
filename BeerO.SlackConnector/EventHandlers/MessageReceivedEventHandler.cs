using System.Threading.Tasks;
using BeerO.SlackConnector.Models;

namespace BeerO.SlackConnector.EventHandlers
{
    public delegate Task MessageReceivedEventHandler(SlackMessage message);
}