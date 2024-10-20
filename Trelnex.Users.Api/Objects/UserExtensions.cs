using Trelnex.Users.Client;

namespace Trelnex.Users.Api.Objects;

internal static class UserExtensions
{
    public static UserModel ConvertToModel(
        this IUser user)
    {
        return new UserModel
        {
            Id = Guid.Parse(user.Id),
            UserName = user.UserName,
        };
    }

}
