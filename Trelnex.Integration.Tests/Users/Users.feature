Feature: Users

@users
Scenario: User is created
	When User Doris is created
	Then User Doris is valid

@users
Scenario: User is not found
	Then User 2f7cc4e1-78db-47a4-8c01-0af89ab8cd86 is not found
