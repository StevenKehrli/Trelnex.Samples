using System.Linq.Expressions;
using Trelnex.Integration.Tests.InMemory;
using Trelnex.Users.Api.Objects;
using Trelnex.Users.Client;

namespace Trelnex.Integration.Tests.Users;

public class UsersContext
{
    private IUsersClient _usersClient = null!;

    private readonly Dictionary<string, UserModel> _userModels = [];

    public UsersContext()
    {
        // create the users command providers
        var inMemoryCommandProviders = InMemoryCommandProviders.Create(
            options => options.AddUsersCommandProviders());

        // get the users command provider
        var userProvider = inMemoryCommandProviders.Get<IUser>();

        // create the users client
        _usersClient = new UsersClient(userProvider);
    }

    public IUsersClient Client => _usersClient;

    public void Add(
        UserModel userModel)
    {
        var key = $"{userModel.Id}";

        _userModels[key] = userModel;
    }

    public bool Exists(
        Expression<Func<UserModel, bool>> predicate)
    {
        return _userModels.Values.AsQueryable().Any(predicate);
    }

    public UserModel Single(
        Expression<Func<UserModel, bool>> predicate)
    {
        return _userModels.Values.AsQueryable().Single(predicate);
    }
}
