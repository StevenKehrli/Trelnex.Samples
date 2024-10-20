using Trelnex.Mailboxes.Client;

namespace Trelnex.Mailboxes.Api.Objects;

internal static class UserMailboxExtensions
{
    public static MailboxModel ConvertToModel(
        this IUserMailbox userMailbox)
    {
        return new MailboxModel
        {
            Id = userMailbox.MailboxId,
            OwnerId = Guid.Parse(userMailbox.Id),
            OwnerType = userMailbox.OwnerType,
        };
    }
}
