using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace Trelnex.Messages.Client;

public record DeleteMessageResponse
{
    [JsonPropertyName("userId")]
    [SwaggerSchema("The id of the user.", Nullable = false)]
    public required Guid UserId { get; init; }

    [JsonPropertyName("messageId")]
    [SwaggerSchema("The id of the message.", Nullable = false)]
    public required Guid MessageId { get; init; }

    [JsonInclude]
    [JsonPropertyName("deletedDateTimeOffset")]
    public required DateTimeOffset DeletedDateTimeOffset { get; init; }
}
