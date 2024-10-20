using Trelnex.Core.Api.Authentication;

namespace Trelnex.Mailboxes.Api.Endpoints;

internal class MailboxesPermission : MicrosoftIdentityPermission
{
    protected override string ConfigSectionName => "Auth:trelnex-api-mailboxes";
    public override string JwtBearerScheme => "Bearer.trelnex-api-mailboxes";

    public override void AddAuthorization(
        IPoliciesBuilder policiesBuilder)
    {
        policiesBuilder
            .AddPolicy<MailboxesReadPolicy>();
    }

    public class MailboxesReadPolicy : IPermissionPolicy
    {
        public string[] RequiredRoles => ["mailboxes.read"];
    }
}
