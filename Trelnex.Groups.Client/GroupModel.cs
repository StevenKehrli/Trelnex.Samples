using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace Trelnex.Groups.Client;

public record GroupModel
{
    /// <summary>
    /// The id of the group.
    /// </summary>
    [JsonPropertyName("id")]
    [SwaggerSchema("The id of the group.", Nullable = false)]
    public required Guid Id { get; init; }

    /// <summary>
    /// The name of this group.
    /// </summary>
    [JsonPropertyName("groupName")]
    [SwaggerSchema("The name of the group.", Nullable = false)]
    public required string GroupName { get; init; }
}
