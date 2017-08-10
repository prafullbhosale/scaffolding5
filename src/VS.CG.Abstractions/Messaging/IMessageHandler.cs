namespace Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Messaging
{
    public interface IMessageHandler
    {
        bool HandleMessage(IMessageSender sender, Message message);
    }
}
