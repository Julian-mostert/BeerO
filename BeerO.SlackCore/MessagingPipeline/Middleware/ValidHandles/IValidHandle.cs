namespace BeerO.SlackCore.MessagingPipeline.Middleware.ValidHandles
{
    public interface IValidHandle
    {
        bool IsMatch(string message);
        string HandleHelpText { get; }
    }
}