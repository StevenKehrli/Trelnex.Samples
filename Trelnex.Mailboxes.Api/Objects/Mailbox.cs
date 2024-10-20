using System.Text.Json.Serialization;
using FluentValidation;
using Trelnex.Core.Data;
using Trelnex.Mailboxes.Client;

namespace Trelnex.Mailboxes.Api.Objects;

internal interface IMailbox : IBaseItem
{
    /// <summary>
    /// The ID of the owner for this mailbox.
    /// </summary>
    Guid OwnerId { get; set; }

    /// <summary>
    /// The type of owner of this mailbox.
    /// </summary>
    MailboxOwnerType OwnerType { get; set; }
}

internal class Mailbox : BaseItem, IMailbox
{
    [JsonPropertyName("ownerId")]
    public Guid OwnerId { get; set; }

    [JsonPropertyName("ownerType")]
    public MailboxOwnerType OwnerType { get; set; }

    public static AbstractValidator<Mailbox> Validator { get; } = new MailboxValidator();

    private class MailboxValidator : AbstractValidator<Mailbox>
    {
        public MailboxValidator()
        {
            RuleFor(k => k.OwnerId)
                .NotEmpty()
                .WithMessage("OwnerId is null or empty.");
        }
    }
}
