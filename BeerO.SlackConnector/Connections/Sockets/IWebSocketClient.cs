﻿using System;
using System.Threading.Tasks;
using BeerO.SlackConnector.Connections.Sockets.Messages.Inbound;
using BeerO.SlackConnector.Connections.Sockets.Messages.Outbound;

namespace BeerO.SlackConnector.Connections.Sockets
{
    internal interface IWebSocketClient
    {
        //bool IsAlive { get; }

        Task Connect(string webSockerUrl);
        Task SendMessage(BaseMessage message);
        Task Close();

        event EventHandler<InboundMessage> OnMessage;
        event EventHandler OnClose;
    }
}