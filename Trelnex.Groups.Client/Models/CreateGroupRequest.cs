using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace Trelnex.Groups.Client;

public record CreateGroupRequest
{
    [JsonPropertyName("groupName")]
    [SwaggerSchema("The name of the group.", Nullable = false)]
    public required string GroupName { get; init; }
}
