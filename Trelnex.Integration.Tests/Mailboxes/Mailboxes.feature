Feature: Mailboxes

@users
@mailboxes
Scenario: User Mailbox is valid
	Given User Doris exists
	And UserMailbox for Doris exists
	Then UserMailbox for Doris is valid

@groups
@mailboxes
Scenario: Group Mailbox is valid
	Given Group Testers exists
	And GroupMailbox for Testers exists
	Then GroupMailbox for Testers is valid

@mailboxes
Scenario: Mailbox is not found
	Then Mailbox for 84cf1068-f8ba-4393-97d0-e61679f3b5b3 is not found
