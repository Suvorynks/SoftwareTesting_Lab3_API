Feature: Restful Booker API CRUD Operations
  As an API tester
  I want to test the Create, Read, Update, and Delete operations
  So that I can ensure the hotel booking system works correctly

Scenario: Perform full CRUD on a hotel booking
  Given I authenticate and get a secure token
  When I create a new booking for "Iryna" "Suvorova"
  Then the booking should be created successfully
  
  When I retrieve the booking details
  Then the details should match "Iryna" "Suvorova"
  
  When I update the booking checkout date
  Then the booking should be updated successfully
  
  When I delete the booking
  Then the booking should not exist anymore