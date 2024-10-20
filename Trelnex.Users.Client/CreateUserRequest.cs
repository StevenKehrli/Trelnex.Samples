using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace Trelnex.Users.Client;

public record CreateUserRequest
{
    [JsonPropertyName("userName")]
    [SwaggerSchema("The name of the user.", Nullable = false)]
    public required string UserName { get; init; }
}
