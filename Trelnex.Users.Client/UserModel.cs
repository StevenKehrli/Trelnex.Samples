using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace Trelnex.Users.Client;

public record UserModel
{
    /// <summary>
    /// The id of the user.
    /// </summary>
    [JsonPropertyName("id")]
    [SwaggerSchema("The id of the user.", Nullable = false)]
    public required Guid Id { get; init; }

    /// <summary>
    /// The name of this user.
    /// </summary>
    [JsonPropertyName("userName")]
    [SwaggerSchema("The name of the user.", Nullable = false)]
    public required string UserName { get; init; }
}
