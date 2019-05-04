using System;

namespace BeerO.SlackConnector.Connections.Monitoring
{
    internal class DateTimeKeeper : IDateTimeKeeper
    {
        private DateTime? _dateTime;

        public void SetDateTimeToNow()
        {
            this._dateTime = DateTime.Now;
        }

        public bool HasDateTime()
        {
            return this._dateTime.HasValue;
        }

        public TimeSpan TimeSinceDateTime()
        {
            if (!this._dateTime.HasValue)
            {
                throw new DateTimeNotSetException();
            }

            return DateTime.Now - this._dateTime.Value;
        }

        public class DateTimeNotSetException : Exception
        { }
    }
}