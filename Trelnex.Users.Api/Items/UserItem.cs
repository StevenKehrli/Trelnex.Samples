using System.Text.Json.Serialization;
using FluentValidation;
using Trelnex.Core.Data;

namespace Trelnex.Users.Api.Items;

internal interface IUserItem : IBaseItem
{
    /// <summary>
    /// The name of this user.
    /// </summary>
    string UserName { get; set; }
}

internal class UserItem : BaseItem, IUserItem
{
    [TrackChange]
    [JsonPropertyName("userName")]
    public string UserName { get; set; } = null!;

    public static AbstractValidator<UserItem> Validator { get; } = new UserItemValidator();

    private class UserItemValidator : AbstractValidator<UserItem>
    {
        public UserItemValidator()
        {
            RuleFor(k => k.UserName)
                .NotEmpty()
                .WithMessage("userName is null.");
        }
    }
}
