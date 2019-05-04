using System;
using System.Threading.Tasks;

namespace BeerO.SlackConnector.EventHandlers
{
    public delegate Task PongEventHandler(DateTime timestamp);
}
