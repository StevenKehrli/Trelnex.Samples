Feature: Messages

@users
@mailboxes
@messages
Scenario: User Message is created
	Given User Doris exists
	And UserMailbox for Doris exists
	When Message with contents 'first message' in UserMailbox for Doris is created
	Then Message with contents 'first message' in UserMailbox for Doris is valid

@users
@mailboxes
@messages
Scenario: User Messages are empty
	Given User Doris exists
	And UserMailbox for Doris exists
	Then Messages in UserMailbox for Doris are valid
		| Contents       |

@users
@mailboxes
@messages
Scenario: User Messages are valid
	Given User Doris exists
	And UserMailbox for Doris exists
	When Message with contents 'first message' in UserMailbox for Doris is created
	And Message with contents 'second message' in UserMailbox for Doris is created
	And Message with contents 'third message' in UserMailbox for Doris is created
	Then Messages in UserMailbox for Doris are valid
		| Contents       |
		| first message  |
		| second message |
		| third message  |

@users
@mailboxes
@messages
Scenario: User Message is updated
	Given User Doris exists
	And UserMailbox for Doris exists
	And Message with contents 'first message' in UserMailbox for Doris exists
	When Message with contents 'first message' in UserMailbox for Doris is updated to contents 'first message - updated'
	Then Message with contents 'first message - updated' in UserMailbox for Doris is valid

@users
@mailboxes
@messages
Scenario: User Message is deleted
	Given User Doris exists
	And UserMailbox for Doris exists
	And Message with contents 'first message' in UserMailbox for Doris exists
	When Message with contents 'first message' in UserMailbox for Doris is deleted
	Then Message with contents 'first message' in UserMailbox for Doris is not found

@groups
@mailboxes
@messages
Scenario: Group Message is created
	Given Group Testers exists
	And GroupMailbox for Testers exists
	When Message with contents 'first message' in GroupMailbox for Testers is created
	Then Message with contents 'first message' in GroupMailbox for Testers is valid

@groups
@mailboxes
@messages
Scenario: Group Messages are empty
	Given Group Testers exists
	And GroupMailbox for Testers exists
	Then Messages in GroupMailbox for Testers are valid
		| Contents       |

@groups
@mailboxes
@messages
Scenario: Group Messages are valid
	Given Group Testers exists
	And GroupMailbox for Testers exists
	When Message with contents 'first message' in GroupMailbox for Testers is created
	And Message with contents 'second message' in GroupMailbox for Testers is created
	And Message with contents 'third message' in GroupMailbox for Testers is created
	Then Messages in GroupMailbox for Testers are valid
		| Contents       |
		| first message  |
		| second message |
		| third message  |

@groups
@mailboxes
@messages
Scenario: Group Message is updated
	Given Group Testers exists
	And GroupMailbox for Testers exists
	And Message with contents 'first message' in GroupMailbox for Testers exists
	When Message with contents 'first message' in GroupMailbox for Testers is updated to contents 'first message - updated'
	Then Message with contents 'first message - updated' in GroupMailbox for Testers is valid

@groups
@mailboxes
@messages
Scenario: Group Message is deleted
	Given Group Testers exists
	And GroupMailbox for Testers exists
	And Message with contents 'first message' in GroupMailbox for Testers exists
	When Message with contents 'first message' in GroupMailbox for Testers is deleted
	Then Message with contents 'first message' in GroupMailbox for Testers is not found

@messages
Scenario: CreateMessage with bad mailboxId throws BadRequest
	Then CreateMessage with bad mailboxId throws BadRequest

@messages
Scenario: UpdateMessage with bad mailboxId throws BadRequest
	Then UpdateMessage with bad mailboxId throws BadRequest

@messages
Scenario: UpdateMessage with bad messageId throws BadRequest
	Then UpdateMessage with bad messageId throws BadRequest

@messages
Scenario: UpdateMessage not found
	Then UpdateMessage not found

@messages
Scenario: DeleteMessage not found
	Then DeleteMessage not found
