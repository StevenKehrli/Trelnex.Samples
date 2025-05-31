using System.Text.Json.Serialization;
using FluentValidation;
using Trelnex.Core.Data;

namespace Trelnex.Messages.Api.Items;

internal interface IMessageItem : IBaseItem
{
    /// <summary>
    /// The contents of the message.
    /// </summary>
    string? Contents { get; set; }
}

internal class MessageItem : BaseItem, IMessageItem
{
    [TrackChange]
    [JsonPropertyName("contents")]
    public string? Contents { get; set; }

    public static AbstractValidator<MessageItem> Validator { get; } = new MessageItemValidator();

    private class MessageItemValidator : AbstractValidator<MessageItem>
    {
        public MessageItemValidator()
        {
            RuleFor(k => k.Contents)
                .NotEmpty()
                .WithMessage("contents is null.");
        }
    }
}
