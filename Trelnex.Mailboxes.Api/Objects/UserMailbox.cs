using System.Text.Json.Serialization;
using FluentValidation;
using Trelnex.Core.Data;
using Trelnex.Mailboxes.Client;

namespace Trelnex.Mailboxes.Api.Objects;

internal interface IUserMailbox : IBaseItem
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

internal class UserMailbox : BaseItem, IUserMailbox
{
    [JsonPropertyName("ownerType")]
    public MailboxOwnerType OwnerType { get; set; }

    [JsonPropertyName("mailboxId")]
    public Guid MailboxId { get; set; }

    public static AbstractValidator<UserMailbox> Validator { get; } = new UserMailboxValidator();

    private class UserMailboxValidator : AbstractValidator<UserMailbox>
    {
        public UserMailboxValidator()
        {
            RuleFor(k => k.OwnerType)
                .Equal(MailboxOwnerType.User)
                .WithMessage($"OwnerType is not '{MailboxOwnerType.User}'.");

            RuleFor(k => k.MailboxId)
                .NotDefault()
                .WithMessage("MailboxId is not valid.");
        }
    }
}
