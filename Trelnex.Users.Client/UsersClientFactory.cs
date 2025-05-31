using Trelnex.Core.Client;
using Trelnex.Core.Identity;

namespace Trelnex.Users.Client;

public class UsersClientFactory : IClientFactory<IUsersClient>
{
    public string Name => typeof(UsersClient).Name;

    public IUsersClient Create(
        HttpClient httpClient,
        IAccessTokenProvider? accessTokenProvider)
    {
        ArgumentNullException.ThrowIfNull(accessTokenProvider);

        return new UsersClient(
            httpClient,
            accessTokenProvider);
    }
}
