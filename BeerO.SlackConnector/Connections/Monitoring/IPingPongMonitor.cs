using System;
using System.Threading.Tasks;

namespace BeerO.SlackConnector.Connections.Monitoring
{
    internal interface IPingPongMonitor
    {
        Task StartMonitor(Func<Task> pingMethod, Func<Task> reconnectMethod, TimeSpan pongTimeout);
        void Pong();
    }
}