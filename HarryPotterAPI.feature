Feature: Public API Testing (Harry Potter Spells)
  As a tester
  I want to fetch a list of Harry Potter spells in Ukrainian
  So that I can verify the public API works and returns data

Scenario: Get Harry Potter spells successfully
  Given I connect to the Harry Potter API
  When I request the list of spells
  Then the API response should be successful
  And the response should contain a list of spells