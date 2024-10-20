using System.Text.Json.Serialization;
using FluentValidation;
using Trelnex.Core.Data;

namespace Trelnex.Messages.Api.Objects;

internal interface IMessage : IBaseItem
{
    /// <summary>
    /// The contents of the message.
    /// </summary>
    string? Contents { get; set; }
}

internal class Message : BaseItem, IMessage
{
    [TrackChange]
    [JsonPropertyName("contents")]
    public string? Contents { get; set; }

    public static AbstractValidator<Message> Validator { get; } = new MessageValidator();

    private class MessageValidator : AbstractValidator<Message>
    {
        public MessageValidator()
        {
            RuleFor(k => k.Contents)
                .NotEmpty()
                .WithMessage("contents is null.");
        }
    }
}
