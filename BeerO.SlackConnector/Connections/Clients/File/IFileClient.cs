using System.IO;
using System.Threading.Tasks;

namespace BeerO.SlackConnector.Connections.Clients.File
{
    internal interface IFileClient
    {
        Task PostFile(string slackKey, string channel, string filePath);
        Task PostFile(string slackKey, string channel, Stream stream, string fileName);
    }
}
