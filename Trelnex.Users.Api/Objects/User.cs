using System.Text.Json.Serialization;
using FluentValidation;
using Trelnex.Core.Data;

namespace Trelnex.Users.Api.Objects;

internal interface IUser : IBaseItem
{
    /// <summary>
    /// The name of this user.
    /// </summary>
    string UserName { get; set; }
}

internal class User : BaseItem, IUser
{
    [TrackChange]
    [JsonPropertyName("userName")]
    public string UserName { get; set; } = null!;

    public static AbstractValidator<User> Validator { get; } = new UserValidator();

    private class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(k => k.UserName)
                .NotEmpty()
                .WithMessage("userName is null.");
        }
    }
}
