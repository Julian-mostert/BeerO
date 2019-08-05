using System;
using System.Collections.Generic;
using System.Linq;

namespace BeerO.SlackCore.Logging
{
    public class AverageStat
    {
        private readonly string _unitName;
        private readonly object _lock = new object();
        private readonly List<double> _log = new List<double>();

        public AverageStat(string unitName)
        {
            this._unitName = unitName;
        }

        public void Log(double value)
        {
            lock (this._lock)
            {
                this._log.Add(value);
            }
        }

        public override string ToString()
        {
            string value = "Nothing logged yet :-(";

            lock (this._lock)
            {
                if (this._log.Any())
                {
                    double total = this._log.Sum(x => x);
                    value = $"{Math.Round(total / this._log.Count)} {this._unitName}";
                }
            }

            return value;
        }
    }
}