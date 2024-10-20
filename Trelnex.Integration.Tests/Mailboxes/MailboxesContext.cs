using System.Linq.Expressions;
using Trelnex.Integration.Tests.Groups;
using Trelnex.Integration.Tests.InMemory;
using Trelnex.Integration.Tests.Users;
using Trelnex.Mailboxes.Api.Objects;
using Trelnex.Mailboxes.Client;

namespace Trelnex.Integration.Tests.Mailboxes;

public class MailboxesContext
{
    private IMailboxesClient _mailboxesClient = null!;

    private readonly Dictionary<string, MailboxModel> _mailboxModels = [];

    public MailboxesContext(
        UsersContext usersContext,
        GroupsContext groupsContext)
    {
        // create the mailboxes command providers
        var inMemoryCommandProviders = InMemoryCommandProviders.Create(
            options => options.AddMailboxesCommandProviders());

        // get the mailboxes command provider
        var mailboxProvider = inMemoryCommandProviders.Get<IMailbox>();
        var userMailboxProvider = inMemoryCommandProviders.Get<IUserMailbox>();
        var groupMailboxProvider = inMemoryCommandProviders.Get<IGroupMailbox>();

        // create the mailboxes client
        _mailboxesClient = new MailboxesClient(
            usersClient: usersContext.Client,
            groupsClient: groupsContext.Client,
            mailboxProvider: mailboxProvider,
            userMailboxProvider: userMailboxProvider,
            groupMailboxProvider: groupMailboxProvider);
    }

    public IMailboxesClient Client => _mailboxesClient;

    public void Add(
        MailboxModel mailboxModel)
    {
        var key = $"{mailboxModel.Id}";

        _mailboxModels[key] = mailboxModel;
    }

    public bool Exists(
        Expression<Func<MailboxModel, bool>> predicate)
    {
        return _mailboxModels.Values.AsQueryable().Any(predicate);
    }

    public MailboxModel Single(
        Expression<Func<MailboxModel, bool>> predicate)
    {
        return _mailboxModels.Values.AsQueryable().Single(predicate);
    }
}
