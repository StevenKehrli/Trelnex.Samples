using System.Text.Json.Serialization;

namespace Trelnex.Mailboxes.Client;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MailboxOwnerType
{
    User = 1,
    Group = 2,
}
