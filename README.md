# Trelnex.Samples

These samples demonstrate the usage of [Trelnex.Core](https://github.com/StevenKehrli/Trelnex.Core).

These sample integrate to create a Message, read a Message, update a Message, delete a Message, or query Messages within a Mailbox for a User or Group.

## CosmosDB

These samples use CosmosDB NoSQL as a backing store.

appsettings.json must be configured with the valid TenantId and EndpointUri.

The CosmosDB account must be configured with the necessary data plane role assignments. See [https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/security/reference-data-plane-roles](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/security/reference-data-plane-roles) for more information.

### Trelnex.Users.Api

`Trelnex.Users.Api` requires a `trelnex-users` database and `users` container.

### Trelnex.Groups.Api

`Trelnex.Groups.Api` requires a `trelnex-groups` database and `groups` container.

### Trelnex.Mailboxes.Api

`Trelnex.Mailboxes.Api` requires a `trelnex-mailboxes` database and `mailboxes`, `user-mailboxes`, and `group-mailboxes` containers.

### Trelnex.Messages.Api

`Trelnex.Messages.Api` requires a `trelnex-messages` database and `messages` container.

## Authentication and Authorization

These samples use Microsoft Identity for authentication and authorization.

See [https://learn.microsoft.com/en-us/entra/identity-platform/quickstart-register-app?tabs=certificate](https://learn.microsoft.com/en-us/entra/identity-platform/quickstart-register-app?tabs=certificate) for more information.

appsettings.json must be configured with the valid Instance, TenantId, ClientId, and IdentifierURI. IdentifierURI is the Application ID URI for the app registration.

### Trelnex.Users.Api

`Trelnex.Users.Api` requires `users.create` and `users.read` roles.

### Trelnex.Groups.Api

`Trelnex.Groups.Api` requires `groups.create` and `groups.read` roles.

### Trelnex.Mailboxes.Api

`Trelnex.Mailboxes.Api` requires `mailboxes.read` role.

### Trelnex.Messages.Api

`Trelnex.Messages.Api` requires `messages.create`, `messages.read`, `messages.update`, and `messages.delete` role.

## Projects

### Trelnex.Users.Api

`Trelnex.Users.Api` is a REST API to create a User and get a User.

[http://localhost:5185/swagger](http://localhost:5185/swagger)

### Trelnex.Users.Client

`Trelnex.Users.Client` is an HTTP client for the above `Trelnex.Users.Api` REST API to create a user and get a User.

This client is integrated into `Trelnex.Mailboxes.Api` to validate a User.

### Trelnex.Groups.Api

`Trelnex.Groups.Api` is a REST API to create a Group and get a Group.

[http://localhost:5186/swagger](http://localhost:5186/swagger)

### Trelnex.Groups.Client

`Trelnex.Groups.Client` is an HTTP client for the above `Trelnex.Groups.Api` REST API to create a Group and get a Group.

This client is integrated into `Trelnex.Mailboxes.Api` to validate a Group.

### Trelnex.Mailboxes.Api

Each User and Group owns a Mailbox.

`Trelnex.Mailboxes.Api` is a REST API to get a Mailbox, by mailbox id, user id, or group id.

[http://localhost:5187/swagger](http://localhost:5187/swagger)

### Trelnex.Mailboxes.Client

`Trelnex.Mailboxes.Client` is an HTTP client for the above `Trelnex.Mailboxes.Api` REST API to get a Mailbox, by mailbox id, user id, or group id.

This client is integrated into `Trelnex.Messages.Api` to validate a Mailbox.

### Trelnex.Messages.Api

A Message may be stored in a Mailbox.

`Trelnex.Messages.Api` is a REST API to create a Message, read a Message, update a Message, delete a Message, or query Messages.

[http://localhost:5188/swagger](http://localhost:5188/swagger)

### Trelnex.Messages.Client

`Trelnex.Messages.Client` is an HTTP client for the above `Trelnex.Messages.Api` REST API to create a Message, read a Message, update a Message, delete a Message, or query Messages.

## Trelnex.Integration.Tests

`Trelnex.Integration.Tests` are [SpecFlow](https://specflow.org/) tests.

### ICommandProvider<TInterface, TItem>

The `Trelnex.Core.Data.Emulators` library exposes the `InMemoryCommandProvider<TInterface, TItem>` implementation of `ICommandProvider<TInterface, TItem>`.  We can use this to assist development and testing of their business logic.

For example, this code creates the `ICommandProvider<TInterface, TItem>` for `IUser`.

```csharp
    // create the users command providers
    var inMemoryCommandProviders = InMemoryCommandProviders.Create(
        options => options.AddUsersCommandProviders());
```

This is same logic as the `Trelnex.Users.Api` startup logic, but using the `InMemoryCommandProvider<TInterface, TItem>` instead of `CosmosCommandProvider<TInterface, TItem>`.

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
