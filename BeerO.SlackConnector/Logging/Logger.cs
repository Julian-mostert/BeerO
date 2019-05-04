using System;

namespace BeerO.SlackConnector.Logging
{
    public class Logger : ILogger
    {
        public void LogError(string message)
        {
            Console.WriteLine(message);
        }
    }
}