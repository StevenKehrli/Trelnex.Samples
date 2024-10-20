using Trelnex.Core.Api.Authentication;

namespace Trelnex.Groups.Api.Endpoints;

internal class GroupsPermission : MicrosoftIdentityPermission
{
    protected override string ConfigSectionName => "Auth:trelnex-api-groups";

    public override string JwtBearerScheme => "Bearer.trelnex-api-groups";

    public override void AddAuthorization(
        IPoliciesBuilder policiesBuilder)
    {
        policiesBuilder
            .AddPolicy<GroupsCreatePolicy>()
            .AddPolicy<GroupsReadPolicy>();
    }

    public class GroupsCreatePolicy : IPermissionPolicy
    {
        public string[] RequiredRoles => ["groups.create"];
    }

    public class GroupsReadPolicy : IPermissionPolicy
    {
        public string[] RequiredRoles => ["groups.read"];
    }
}
