namespace BeerO.SlackConnector.Connections.Monitoring
{
    internal interface IMonitoringFactory
    {
        IPingPongMonitor CreatePingPongMonitor();
    }
}