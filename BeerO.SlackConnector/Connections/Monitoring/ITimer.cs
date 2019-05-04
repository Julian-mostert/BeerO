using System;

namespace BeerO.SlackConnector.Connections.Monitoring
{
    internal interface ITimer : IDisposable
    {
        void RunEvery(Action action, TimeSpan tick);
    }
}