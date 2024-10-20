using Trelnex.Groups.Client;

namespace Trelnex.Groups.Api.Objects;

internal static class GroupExtensions
{
    public static GroupModel ConvertToModel(
        this IGroup group)
    {
        return new GroupModel
        {
            Id = Guid.Parse(group.Id),
            GroupName = group.GroupName,
        };
    }
}
