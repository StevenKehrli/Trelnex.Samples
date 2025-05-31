# Trelnex.Samples

These samples demonstrate the usage of [Trelnex.Core](https://github.com/StevenKehrli/Trelnex.Core).

These samples integrate to create a User, read a User, create a Message, read a Message, update a Message, delete a Message, or query Messages.

## CosmosDB

These samples use CosmosDB NoSQL as a backing store.

appsettings.json must be configured with the valid TenantId and EndpointUri.

The CosmosDB account must be configured with the necessary data plane role assignments. See [https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/security/reference-data-plane-roles](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/security/reference-data-plane-roles) for more information.

### Trelnex.Users.Api

`Trelnex.Users.Api` requires a `trelnex-users` database and `users` container.

### Trelnex.Messages.Api

`Trelnex.Messages.Api` requires a `trelnex-messages` database and `messages` container.

## Authentication and Authorization

These samples use Microsoft Identity for authentication and authorization.

See [https://learn.microsoft.com/en-us/entra/identity-platform/quickstart-register-app?tabs=certificate](https://learn.microsoft.com/en-us/entra/identity-platform/quickstart-register-app?tabs=certificate) for more information.

appsettings.json must be configured with the valid Instance, TenantId, ClientId, and IdentifierURI. IdentifierURI is the Application ID URI for the app registration.

### Trelnex.Users.Api

`Trelnex.Users.Api` requires `users.create` and `users.read` roles.

### Trelnex.Messages.Api

`Trelnex.Messages.Api` requires `messages.create`, `messages.read`, `messages.update`, and `messages.delete` roles.

## Projects

### Trelnex.Users.Api

`Trelnex.Users.Api` is a REST API to create a User and get a User.

[http://localhost:5185/swagger](http://localhost:5185/swagger)

### Trelnex.Users.Client

`Trelnex.Users.Client` is an HTTP client for the above `Trelnex.Users.Api` REST API to create a User and get a User.

### Trelnex.Messages.Api

`Trelnex.Messages.Api` is a REST API to create a Message, read a Message, update a Message, delete a Message, or query Messages.

[http://localhost:5188/swagger](http://localhost:5188/swagger)

### Trelnex.Messages.Client

`Trelnex.Messages.Client` is an HTTP client for the above `Trelnex.Messages.Api` REST API to create a Message, read a Message, update a Message, delete a Message, or query Messages.

## Trelnex.Integration.Tests

`Trelnex.Integration.Tests` are [SpecFlow](https://specflow.org/) tests.

### ICommandProvider<TInterface, TItem>

The `Trelnex.Core.Data.Emulators` library exposes the `InMemoryCommandProvider<TInterface, TItem>` implementation of `ICommandProvider<TInterface, TItem>`. We can use this to assist development and testing of business logic.

For example, this code creates the `ICommandProvider<TInterface, TItem>` for `IUser` and `IMessage`.

```csharp
    // create the command providers
    var inMemoryCommandProviders = InMemoryCommandProviders.Create(
        options => options
            .AddUsersCommandProviders()
            .AddMessagesCommandProviders());
```

This is the same logic as the API startup logic, but using the `InMemoryCommandProvider<TInterface, TItem>` instead of `CosmosCommandProvider<TInterface, TItem>`.

### Client

Each REST API has a `HandleRequest` method for its business logic and data access. We can construct an HTTP client for each REST API that invokes the `HandleRequest` method instead of the endpoint.

For example, the `IUsersClient` `GetUser` method looks like this:

```csharp
    public async Task<UserModel> GetUser(
        Guid userId)
    {
        // format the request arguments
        var parameters = new GetUserEndpoint.RequestParameters()
        {
            UserId = userId
        };

        // invoke
        return await GetUserEndpoint.HandleRequest(
            userProvider: userProvider,
            parameters: parameters);
    }
```

### Context

Each test maintains its own Context object for its `ICommandProvider<TInterface, TItem>` and Client.

Its step definitions references this Context object to invoke the Client method which will invoke the `HandleRequest` method.
