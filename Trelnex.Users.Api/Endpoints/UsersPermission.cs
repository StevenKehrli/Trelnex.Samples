using Trelnex.Core.Api.Authentication;

namespace Trelnex.Users.Api.Endpoints;

internal class UsersPermission : MicrosoftIdentityPermission
{
    protected override string ConfigSectionName => "Auth:trelnex-api-users";

    public override string JwtBearerScheme => "Bearer.trelnex-api-users";

    public override void AddAuthorization(
        IPoliciesBuilder policiesBuilder)
    {
        policiesBuilder
            .AddPolicy<UsersCreatePolicy>()
            .AddPolicy<UsersReadPolicy>();
    }

    public class UsersCreatePolicy : IPermissionPolicy
    {
        public string[] RequiredRoles => ["users.create"];
    }

    public class UsersReadPolicy : IPermissionPolicy
    {
        public string[] RequiredRoles => ["users.read"];
    }
}
