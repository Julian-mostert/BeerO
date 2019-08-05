using System;

namespace BeerO.SlackConnector.Connections.Monitoring
{
    internal class Timer : ITimer
    {
        private System.Threading.Timer _timer;

        public void RunEvery(Action action, TimeSpan tick)
        {
            if (this._timer != null)
            {
                throw new TimerAlreadyInitialisedException();
            }

            this._timer = new System.Threading.Timer(state => action(), null, TimeSpan.Zero, tick);
        }

        public void Dispose()
        {
            this._timer?.Dispose();
        }

        public class TimerAlreadyInitialisedException : Exception
        { }
    }
}