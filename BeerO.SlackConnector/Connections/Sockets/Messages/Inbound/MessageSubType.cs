namespace BeerO.SlackConnector.Connections.Sockets.Messages.Inbound
{
    internal enum MessageSubType
    {
        Unknown = 0,
        BotMessage,
        MeMessage,
        MessageChanged,
        MessageDeleted,
        ChannelJoin,
        ChannelLeave,
        ChannelTopic,
        ChannelPurpose,
        ChannelName,
        ChannelArchive,
        ChannelUnarchive,
        GroupJoin,
        GroupLeave,
        GroupTopic,
        GroupPurpose,
        GroupName,
        GroupArchive,
        GroupUnarchive,
        FileShare,
        FileComment,
        FileMention,
        PinnedItem,
        UnpinnedItem
    }
}