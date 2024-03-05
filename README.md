# GeneralPolls
An Application for creating polls for elections

Packages Used:


Features Added:
Registration and Sign in using Identity

Progress:
Already added the registration and sign up a while back, failed to track

Tuesday, 26 December 2023
Added a page to veiw store/ created Polls
Added PollsDBViewModel

Thursday, 28 December 2023
Added a form view and controller CreatePoll() for creating new polls

Thursday 29 February 2024
Added PollsPage :- Page that displays all the currently running elections
Added CreatePoll : (Note Should be authorized to only Admins)
Added ViewPoll : Dynamic page for any poll clicked
Added StoreCandidate: Method to store candidate selected by the Admings(Note Should be authorized to only Admins)
Added VotersRegistration: Method to register a user as a voter under a dynamic poll page
Added Vote: Method to enable voters to deduct their vote token and add the vote token to the selected candidate

Things to do:
- Catch the exception that arises when an unregisterd voter tries to vote
- Give roles and restric certain methods to the Admins
