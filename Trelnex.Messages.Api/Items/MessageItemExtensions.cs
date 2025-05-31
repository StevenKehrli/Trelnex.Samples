using Trelnex.Messages.Client;

namespace Trelnex.Messages.Api.Items;

internal static class MessageItemExtensions
{
    public static MessageModel ConvertToModel(
        this IMessageItem messageItem)
    {
        return new MessageModel
        {
            UserId = Guid.Parse(messageItem.PartitionKey),
            MessageId = Guid.Parse(messageItem.Id),
            Contents = messageItem.Contents,
        };
    }
}
