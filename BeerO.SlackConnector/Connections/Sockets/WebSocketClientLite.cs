using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Threading;
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
        private readonly IMessageInterpreter     _interpreter;
        private          IMessageWebSocketRx     _webSocket;
        private          int                     _currentMessageId;
        private          CancellationTokenSource _outerCancelationSource;
        private          IDisposable             _disposableWebsocketStatus;
        private          IDisposable             _disposableMessageReceiver;

        public bool IsAlive => this._outerCancelationSource.IsCancellationRequested;

        public WebSocketClientLite(IMessageInterpreter interpreter)
        {
            this._interpreter = interpreter;
            this._outerCancelationSource = new CancellationTokenSource();
        }

        public async Task Connect(string webSockerUrl)
        {
            while (!this._outerCancelationSource.IsCancellationRequested)
            {
                var innerCancellationSource = new CancellationTokenSource();

                await this.StartWebSocketAsync(innerCancellationSource, webSockerUrl);

                while (!innerCancellationSource.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), innerCancellationSource.Token);
                }

                await Task.Delay(TimeSpan.FromSeconds(5), this._outerCancelationSource.Token);
            }
        }

        private async Task StartWebSocketAsync(CancellationTokenSource innerCancellationTokenSource,
                                               string webSocketUrl)
        {
            using (TcpClient tcpClient = new TcpClient { LingerState = new LingerOption(true, 0) })
                this._webSocket = new MessageWebSocketRx(tcpClient)
                {
                    IgnoreServerCertificateErrors = true,
                    Headers = new Dictionary<string, string>
                    {
                        { "Pragma", "no-cache" }, 
                        { "Cache-Control", "no-cache" }
                    },
                    TlsProtocolType = SslProtocols.Tls12,
                };
            Console.WriteLine("Start");

            this._disposableWebsocketStatus =
                this._webSocket.ConnectionStatusObservable
                    .Subscribe(this.OnConnectionChange(innerCancellationTokenSource));

            this._disposableMessageReceiver =
                this._webSocket.MessageReceiverObservable.Subscribe(this.OnWebSocketOnMessage());

            await this._webSocket.ConnectAsync(new Uri(webSocketUrl));
        }

        public async Task SendMessage(BaseMessage message)
        {
            Interlocked.Increment(ref this._currentMessageId);
            message.Id = this._currentMessageId;
            string json = JsonConvert.SerializeObject(message);

            await this._webSocket.SendTextAsync(json);
        }

        public async Task Close()
        {
            using (this._webSocket)
            {
                this._disposableMessageReceiver.Dispose();
                this._disposableWebsocketStatus.Dispose();
                await this._webSocket.DisconnectAsync();
            }
        }

        public event EventHandler<InboundMessage> OnMessage;

        private Action<string> OnWebSocketOnMessage()
        {
            return replyMessage =>
            {
                string messageJson = replyMessage ?? "";
                var inboundMessage = this._interpreter.InterpretMessage(messageJson);
                this.OnMessage?.Invoke(this, inboundMessage);
            };
        }

        public event EventHandler OnClose;

        private Action<ConnectionStatus> OnConnectionChange(CancellationTokenSource innerCancellationTokenSource)
        {
            return status =>
            {
                switch (status)
                {
                    case ConnectionStatus.Aborted:
                    case ConnectionStatus.ConnectionFailed:
                    case ConnectionStatus.Disconnected:
                        this.OnClose?.Invoke(this, null);
                        break;
                }

                innerCancellationTokenSource
                    .Cancel();
            };
        }
    }
}