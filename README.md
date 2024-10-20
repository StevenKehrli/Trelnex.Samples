# Trelnex.Samples

These samples demonstrate the usage of [Trelnex.Core](https://github.com/StevenKehrli/Trelnex.Core).

These sample integrate to create a Message, read a Message, update a Message, delete a Message, or query Messages within a Mailbox for a User or Group.

## Trelnex.Users.Api

`Trelnex.Users.Api` is a REST API to create a User and get a User.

[http://localhost:5185/swagger](http://localhost:5185/swagger)

## Trelnex.Users.Client

`Trelnex.Users.Client` is an HTTP client for the above `Trelnex.Users.Api` REST API to create a user and get a User.

This client is integrated into `Trelnex.Mailboxes.Api` to validate a User.

## Trelnex.Groups.Api

`Trelnex.Groups.Api` is a REST API to create a Group and get a Group.

[http://localhost:5186/swagger](http://localhost:5186/swagger)

## Trelnex.Groups.Client

`Trelnex.Groups.Client` is an HTTP client for the above `Trelnex.Groups.Api` REST API to create a Group and get a Group.

This client is integrated into `Trelnex.Mailboxes.Api` to validate a Group.

## Trelnex.Mailboxes.Api

Each User and Group owns a Mailbox.

`Trelnex.Mailboxes.Api` is a REST API to get a Mailbox, by mailbox id, user id, or group id.

[http://localhost:5187/swagger](http://localhost:5187/swagger)

## Trelnex.Mailboxes.Client

`Trelnex.Mailboxes.Client` is an HTTP client for the above `Trelnex.Mailboxes.Api` REST API to get a Mailbox, by mailbox id, user id, or group id.

This client is integrated into `Trelnex.Messages.Api` to validate a Mailbox.

## Trelnex.Messages.Api

A Message may be stored in a Mailbox.

`Trelnex.Messages.Api` is a REST API to create a Message, read a Message, update a Message, delete a Message, or query Messages.

[http://localhost:5188/swagger](http://localhost:5188/swagger)

## Trelnex.Messages.Client

`Trelnex.Messages.Client` is an HTTP client for the above `Trelnex.Messages.Api` REST API to create a Message, read a Message, update a Message, delete a Message, or query Messages.
