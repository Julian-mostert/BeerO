namespace BeerO.SlackConnector.Connections.Sockets.Messages.Inbound
{
    internal enum MessageType
    {
        Unknown = 0,
        Message,
        GroupJoined,
        ChannelJoined,
        ImCreated,
        TeamJoin,
        Pong,
        ReactionAdded,
        ChannelCreated
    }
}
