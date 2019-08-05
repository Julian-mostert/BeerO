using System.Threading.Tasks;
using BeerO.SlackConnector.BotHelpers;
using BeerO.SlackConnector.Connections;
using BeerO.SlackConnector.Connections.Monitoring;
using BeerO.SlackConnector.Models;

namespace BeerO.SlackConnector
{
    internal class SlackConnectionFactory : ISlackConnectionFactory
    {
        public async Task<ISlackConnection> Create(ConnectionInformation connectionInformation)
        {
            var slackConnection = new SlackConnection(new ConnectionFactory(), new MentionDetector(), new MonitoringFactory());
            await slackConnection.Initialise(connectionInformation);
            return slackConnection;
        }
    }
}