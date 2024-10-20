Feature: Groups

@groups
Scenario: Group is created
	When Group Testers is created
	Then Group Testers is valid

@groups
Scenario: Group is not found
	Then Group 80a2dafe-ceec-44ca-8b9c-af66a38cc852 is not found
