using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace Trelnex.Messages.Client;

public record CreateMessageRequest
{
    [JsonPropertyName("contents")]
    [SwaggerSchema("The contents of the message.", Nullable = false)]
    public required string? Contents { get; init; }
}
