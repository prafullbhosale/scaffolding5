namespace Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Messaging
{
    public interface IMessageSender
    {
        bool Send(Message message);

        Message CreateMessage(MessageType messageType, object o, int protocolVersion);
    }
}
