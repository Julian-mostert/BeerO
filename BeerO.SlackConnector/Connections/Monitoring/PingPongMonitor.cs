using System;
using System.Threading.Tasks;

namespace BeerO.SlackConnector.Connections.Monitoring
{
    internal class PingPongMonitor : IPingPongMonitor
    {
        private readonly ITimer _timer;
        private readonly IDateTimeKeeper _dateTimeKeeper;

        private TimeSpan _pongTimeout;
        private Func<Task> _pingMethod;
        private Func<Task> _reconnectMethod;
        private bool _isReconnecting;
        private readonly object _reconnectLock = new object();

        public PingPongMonitor(ITimer timer, IDateTimeKeeper dateTimeKeeper)
        {
            this._timer = timer;
            this._dateTimeKeeper = dateTimeKeeper;
        }

        public async Task StartMonitor(Func<Task> pingMethod, Func<Task> reconnectMethod, TimeSpan pongTimeout)
        {
            if (this._dateTimeKeeper.HasDateTime())
            {
                throw new MonitorAlreadyStartedException();
            }

            this._pingMethod = pingMethod;
            this._reconnectMethod = reconnectMethod;
            this._pongTimeout = pongTimeout;

            this._timer.RunEvery(this.TimerTick, TimeSpan.FromSeconds(5));

            await pingMethod().ConfigureAwait(false);
        }

        private void TimerTick()
        {
            if (this.NeedsToReconnect() && !this._isReconnecting)
            {
                lock (this._reconnectLock)
                {
                    this._isReconnecting = true;
                    this._reconnectMethod()
                        .ContinueWith(task => this._isReconnecting = false)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                }
            }

            this._pingMethod();
        }

        private bool NeedsToReconnect()
        {
            return this._dateTimeKeeper.HasDateTime() && this._dateTimeKeeper.TimeSinceDateTime() > this._pongTimeout;
        }

        public void Pong()
        {
            this._dateTimeKeeper.SetDateTimeToNow();
        }
    }
}