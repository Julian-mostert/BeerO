using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeerO.SlackConnector.Connections.Sockets.Messages.Inbound;
using BeerO.SlackConnector.Connections.Sockets.Messages.Outbound;
using IWebsocketClientLite.PCL;
using Newtonsoft.Json;
using WebsocketClientLite.PCL;

namespace BeerO.SlackConnector.Connections.Sockets
{
    internal class WebSocketClientLite : IWebSocketClient
    {
        private readonly IMessageInterpreter _interpreter;

        //private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
        private IMessageWebSocketRx _webSocket;
        private int                 _currentMessageId;
        private static ConnectionStatus    CurrentConnectionStatus;
        public bool IsAlive => CurrentConnectionStatus == ConnectionStatus.WebsocketConnected;


        public WebSocketClientLite(IMessageInterpreter interpreter)
        {
            this._interpreter = interpreter;
        }

        private static Action SubscibeToCurrentStatusCompleted()
        {
            return () =>
            {
                Console
                    .WriteLine($"Connection status completed.");
            };
        }

        private static Action<Exception> SubscibeToCurrentStatusException()
        {
            return ex =>
            {
                Console
                    .WriteLine($"Connection status error: {ex}.");
            };
        }

        private static Action<ConnectionStatus> SubscibeToCurrentStatusChange()
        {
            return s =>
            {
                CurrentConnectionStatus = s;
            };
        }

        public async Task Connect(string webSockerUrl)
        {
            if (this._webSocket != null)
            {
                await this.Close();
            }

            this._webSocket = new MessageWebSocketRx();

            this._webSocket.ConnectionStatusObservable.Subscribe(this.OnConnectionChange);

            var uri = new Uri(webSockerUrl);
            var messageObserver =
                await this._webSocket.CreateObservableMessageReceiver(uri, excludeZeroApplicationDataInPong: true);
            this._subscriptions.Add(messageObserver.Subscribe(OnWebSocketOnMessage));
        }

        public async Task SendMessage(BaseMessage message)
        {
            System.Threading.Interlocked.Increment(ref this._currentMessageId);
            message.Id = this._currentMessageId;
            string json = JsonConvert.SerializeObject(message);

            await this._webSocket.SendTextAsync(json);
        }

        public async Task Close()
        {
            using (this._webSocket)
            {
                foreach (var subscription in this._subscriptions)
                {
                    subscription.Dispose();
                }

                this._subscriptions.Clear();

                await this._webSocket.DisconnectAsync();
            }
        }

        public event EventHandler<InboundMessage> OnMessage;

        private void OnWebSocketOnMessage(string message)
        {
            string messageJson = message ?? "";
            var inboundMessage = this._interpreter.InterpretMessage(messageJson);
            this.OnMessage?.Invoke(this, inboundMessage);
        }

        public event EventHandler OnClose;

        private void OnConnectionChange(ConnectionStatus connectionStatus)
        {
            switch (connectionStatus)
            {
                case ConnectionStatus.Aborted:
                case ConnectionStatus.ConnectionFailed:
                case ConnectionStatus.Disconnected:
                    this.OnClose?.Invoke(this, null);
                    break;
            }
        }
    }
}