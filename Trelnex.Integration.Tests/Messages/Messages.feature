Feature: Messages

@users
@messages
Scenario: User Message is created
	Given User Doris exists
	When Message for User Doris with contents 'first message' is created
	Then Message for User Doris with contents 'first message' is valid

@users
@messages
Scenario: User Messages are empty
	Given User Doris exists
	Then Messages for User Doris are valid
		| Contents       |

@users
@messages
Scenario: User Messages are valid
	Given User Doris exists
	When Message for User Doris with contents 'first message' is created
	And Message for User Doris with contents 'second message' is created
	And Message for User Doris with contents 'third message' is created
	Then Messages for User Doris are valid
		| Contents       |
		| first message  |
		| second message |
		| third message  |

@users
@messages
Scenario: User Message is updated
	Given User Doris exists
	And Message for User Doris with contents 'first message' exists
	When Message for User Doris with contents 'first message' is updated to contents 'first message - updated'
	Then Message for User Doris with contents 'first message - updated' is valid

@users
@messages
Scenario: User Message is deleted
	Given User Doris exists
	And Message for User Doris with contents 'first message' exists
	When Message for User Doris with contents 'first message' is deleted
	Then Message for User Doris with contents 'first message' is not found

@users
@messages
Scenario: UpdateMessage with bad messageId throws NotFound
	Given User Doris exists
	Then UpdateMessage for User Doris with bad messageId throws NotFound

@users
@messages
Scenario: DeleteMessage with bad messageId throws NotFound
	Given User Doris exists
	Then DeleteMessage for User Doris with bad messageId throws NotFound
