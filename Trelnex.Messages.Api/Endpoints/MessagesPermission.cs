using Trelnex.Core.Api.Authentication;

namespace Trelnex.Messages.Api.Endpoints;

internal class MessagesPermission : MicrosoftIdentityPermission
{
    protected override string ConfigSectionName => "Auth:trelnex-api-messages";

    public override string JwtBearerScheme => "Bearer.trelnex-api-messages";

    public override void AddAuthorization(
        IPoliciesBuilder policiesBuilder)
    {
        policiesBuilder
            .AddPolicy<MessagesCreatePolicy>()
            .AddPolicy<MessagesReadPolicy>()
            .AddPolicy<MessagesUpdatePolicy>()
            .AddPolicy<MessagesDeletePolicy>();
    }

    public class MessagesCreatePolicy : IPermissionPolicy
    {
        public string[] RequiredRoles => ["messages.create"];
    }

    public class MessagesReadPolicy : IPermissionPolicy
    {
        public string[] RequiredRoles => ["messages.read"];
    }

    public class MessagesUpdatePolicy : IPermissionPolicy
    {
        public string[] RequiredRoles => ["messages.update"];
    }

    public class MessagesDeletePolicy : IPermissionPolicy
    {
        public string[] RequiredRoles => ["messages.delete"];
    }
}
