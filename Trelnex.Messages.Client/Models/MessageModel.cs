using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace Trelnex.Messages.Client;

public record MessageModel
{
    [JsonPropertyName("userId")]
    [SwaggerSchema("The id of the user.", Nullable = false)]
    public required Guid UserId { get; init; }

    [JsonPropertyName("messageId")]
    [SwaggerSchema("The id of the message.", Nullable = false)]
    public required Guid MessageId { get; init; }

    [JsonPropertyName("contents")]
    [SwaggerSchema("The contents of the message.", Nullable = false)]
    public required string? Contents { get; init; }
}
