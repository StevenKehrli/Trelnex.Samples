using Trelnex.Core.Client;
using Trelnex.Core.Identity;

namespace Trelnex.Messages.Client;

public class MessagesClientFactory : IClientFactory<IMessagesClient>
{
    public string Name => typeof(MessagesClient).Name;

    public IMessagesClient Create(
        HttpClient httpClient,
        IAccessTokenProvider? accessTokenProvider)
    {
        ArgumentNullException.ThrowIfNull(accessTokenProvider);

        return new MessagesClient(
            httpClient,
            accessTokenProvider);
    }
}
