using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace Trelnex.Messages.Client;

public record DeleteMessageResponse
{
    [JsonPropertyName("mailboxId")]
    [SwaggerSchema("The id of the mailbox.", Nullable = false)]
    public required Guid MailboxId { get; init; }

    [JsonPropertyName("messageId")]
    [SwaggerSchema("The id of the message.", Nullable = false)]
    public required Guid MessageId { get; init; }

    [JsonInclude]
    [JsonPropertyName("deletedDate")]
    public required DateTime DeletedDate { get; init; }
}
