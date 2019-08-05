using System.Threading.Tasks;
using BeerO.SlackConnector.Models;

namespace BeerO.SlackConnector
{
    internal interface ISlackConnectionFactory
    {
        Task<ISlackConnection> Create(ConnectionInformation connectionInformation);
    }
}