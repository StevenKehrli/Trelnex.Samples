using Trelnex.Mailboxes.Client;

namespace Trelnex.Mailboxes.Api.Objects;

internal static class MailboxExtensions
{
    public static MailboxModel ConvertToModel(
        this IMailbox mailbox)
    {
        return new MailboxModel
        {
            Id = Guid.Parse(mailbox.Id),
            OwnerId = mailbox.OwnerId,
            OwnerType = mailbox.OwnerType,
        };
    }
}
