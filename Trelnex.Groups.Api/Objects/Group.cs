using System.Text.Json.Serialization;
using FluentValidation;
using Trelnex.Core.Data;

namespace Trelnex.Groups.Api.Objects;

internal interface IGroup : IBaseItem
{
    /// <summary>
    /// The name of this group.
    /// </summary>
    string GroupName { get; set; }
}

internal class Group : BaseItem, IGroup
{
    [TrackChange]
    [JsonPropertyName("groupName")]
    public string GroupName { get; set; } = null!;

    public static AbstractValidator<Group> Validator { get; } = new GroupValidator();

    private class GroupValidator : AbstractValidator<Group>
    {
        public GroupValidator()
        {
            RuleFor(k => k.GroupName)
                .NotEmpty()
                .WithMessage("groupName is null.");
        }
    }
}
