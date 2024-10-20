using System.Linq.Expressions;
using Trelnex.Groups.Api.Objects;
using Trelnex.Groups.Client;
using Trelnex.Integration.Tests.InMemory;

namespace Trelnex.Integration.Tests.Groups;

public class GroupsContext
{
    private IGroupsClient _groupsClient = null!;

    private readonly Dictionary<string, GroupModel> _groupModels = [];

    public GroupsContext()
    {
        // create the groups command providers
        var inMemoryCommandProviders = InMemoryCommandProviders.Create(
            options => options.AddGroupsCommandProviders());

        // get the groups command provider
        var groupProvider = inMemoryCommandProviders.Get<IGroup>();

        // create the groups client
        _groupsClient = new GroupsClient(groupProvider);
    }

    public IGroupsClient Client => _groupsClient;

    public void Add(
        GroupModel groupModel)
    {
        var key = $"{groupModel.Id}";

        _groupModels[key] = groupModel;
    }

    public bool Exists(
        Expression<Func<GroupModel, bool>> predicate)
    {
        return _groupModels.Values.AsQueryable().Any(predicate);

    }

    public GroupModel Single(
        Expression<Func<GroupModel, bool>> predicate)
    {
        return _groupModels.Values.AsQueryable().Single(predicate);
    }
}
