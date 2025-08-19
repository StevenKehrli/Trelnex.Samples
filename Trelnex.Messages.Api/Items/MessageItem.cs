using System.Text.Json.Serialization;
using FluentValidation;
using Trelnex.Core.Data;

namespace Trelnex.Messages.Api.Items;

internal record MessageItem : BaseItem
{
    [Track]
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
