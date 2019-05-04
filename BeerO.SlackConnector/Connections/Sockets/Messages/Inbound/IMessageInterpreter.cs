namespace BeerO.SlackConnector.Connections.Sockets.Messages.Inbound
{
    internal interface IMessageInterpreter
    {
        InboundMessage InterpretMessage(string json);
    }
}