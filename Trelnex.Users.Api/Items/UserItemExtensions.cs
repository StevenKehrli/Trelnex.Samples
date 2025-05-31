using Trelnex.Users.Client;

namespace Trelnex.Users.Api.Items;

internal static class UserExtensions
{
    public static UserModel ConvertToModel(
        this IUserItem userItem)
    {
        return new UserModel
        {
            Id = Guid.Parse(userItem.Id),
            UserName = userItem.UserName,
        };
    }
}
