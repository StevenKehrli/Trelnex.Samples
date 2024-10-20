using Trelnex.Mailboxes.Client;

namespace Trelnex.Mailboxes.Api.Objects;

internal static class GroupMailboxExtensions
{
    public static MailboxModel ConvertToModel(
        this IGroupMailbox groupMailbox)
    {
        return new MailboxModel
        {
            Id = groupMailbox.MailboxId,
            OwnerId = Guid.Parse(groupMailbox.Id),
            OwnerType = groupMailbox.OwnerType,
        };
    }
}
