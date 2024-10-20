using System.Text.Json.Serialization;
using FluentValidation;
using Trelnex.Core.Data;
using Trelnex.Mailboxes.Client;

namespace Trelnex.Mailboxes.Api.Objects;

internal interface IGroupMailbox : IBaseItem
{
    /// <summary>
    /// The type of owner.
    /// </summary>
    MailboxOwnerType OwnerType { get; set; }

    /// <summary>
    /// The mailbox id for this owner.
    /// </summary>
    Guid MailboxId { get; set; }
}

internal class GroupMailbox : BaseItem, IGroupMailbox
{
    [JsonPropertyName("ownerType")]
    public MailboxOwnerType OwnerType { get; set; }

    [JsonPropertyName("mailboxId")]
    public Guid MailboxId { get; set; }

    public static AbstractValidator<GroupMailbox> Validator { get; } = new GroupMailboxValidator();

    private class GroupMailboxValidator : AbstractValidator<GroupMailbox>
    {
        public GroupMailboxValidator()
        {
            RuleFor(k => k.OwnerType)
                .Equal(MailboxOwnerType.Group)
                .WithMessage($"OwnerType is not '{MailboxOwnerType.Group}'.");

            RuleFor(k => k.MailboxId)
                .NotDefault()
                .WithMessage("MailboxId is not valid.");
        }
    }
}
