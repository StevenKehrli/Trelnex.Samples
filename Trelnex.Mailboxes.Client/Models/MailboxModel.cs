using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace Trelnex.Mailboxes.Client;

public record MailboxModel
{
    /// <summary>
    /// The id of the mailbox.
    /// </summary>
    [JsonPropertyName("id")]
    [SwaggerSchema("The id of the mailbox.", Nullable = false)]
    public required Guid Id { get; init; }

    /// <summary>
    /// The id of the owner for this mailbox.
    /// </summary>
    [JsonPropertyName("ownerId")]
    [SwaggerSchema("The owner id of the mailbox.", Nullable = false)]
    public required Guid OwnerId { get; init; }

    /// <summary>
    /// The type of owner of this mailbox.
    /// </summary>
    [JsonPropertyName("ownerType")]
    [SwaggerSchema("The owner type of the mailbox.", Nullable = false)]
    public required MailboxOwnerType OwnerType { get; set; }
}
