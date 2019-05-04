using System.Threading.Tasks;

namespace BeerO.SlackConnector
{
    public interface ISlackConnector
    {
        Task<ISlackConnection> Connect(string slackKey);
    }
}