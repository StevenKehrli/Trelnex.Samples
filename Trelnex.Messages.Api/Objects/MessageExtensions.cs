using Trelnex.Messages.Client;

namespace Trelnex.Messages.Api.Objects;

internal static class MessageExtensions
{
    public static MessageModel ConvertToModel(
        this IMessage message)
    {
        return new MessageModel
        {
            MailboxId = Guid.Parse(message.PartitionKey),
            MessageId = Guid.Parse(message.Id),
            Contents = message.Contents,
        };
    }
}
