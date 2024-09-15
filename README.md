# GeneralPolls
An Application for creating polls for elections

Packages Used:
EntityFramework: ORM and identity
Brevo/SendingBlue: Email

Tables:
    CandiateTable:
        string Id
        string ElectionId
        int VoteCount
        string CandidateName
        string Email
        string CandidatePicturePath
    PollsTable:
        string Id
        string ElectionName
        int CanidateCount
    RegisteredVotersTable:
        string Id
        string ElectionId
        string UserId
        int Vote
    ApplicationUser:
        inherits from idenitityUser
        string FirstName
        string LastName
        string Email
        string File_Location (location of profile picture)
        IFormFile ProfilePicture

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
- Catch the exception that arises when an unregisterd voter tries to vote. done
- Give roles and restric certain methods to the Admins. done

Friday 2 August 2024
Added Email Confirmation
    ActionResult ConfirmEmail()
    ActionResult ConfirmationComplete
    bool EmailConfirmationStatus(): Application and Repository
    bool SendConfirmationLink(): Application Layer
Added IsAuthenticated
Added UserProfilePicture
    ActionResult Settings() Post/Get
    settings page for users to change profile picture
Added FrontEnd Design Using Bootstrap Admin from boostrapious
Modified ViewPolls
    added CandidatePicturePath
    represented the list of candidate in a table 
Changes Made:
Added a "Delete Candidate" feature:
	[HttpGet]
	RemoveCandidate(string candidateId) 

	[HttpPost]
	DeleteCandidate(string candidateId)
		Service Class: DeleteCandidate(string candidateId)
			Repository Class: DeleteCandidate(string candidateId)
Had localdb(MssqlDb) connection issues:
	made new dbserver
	deleted old migrations folder, made new migrations
Fixed Duplicate candidate issues

Updated the PollsDBTable:
	Added string UserId (Id of the user that created the poll)
	Removed string CandidateCount
Mon 05 08 2024
Making checks to see if current poll belongs to current user? allow delete candidate : don't allow
Successfully implemented the "Delete candidate" feature

Thursday 8 august
Updated Confirmation:
	Added a "WaitForResend" page that counts down for 30 secs before allowing user to resend confirmation link
Added a "UpdatesToCome" page containing a list of updates to be added to the project as it has been deployed "http://generalpoll.somee.com"
Published a deployed a first draft to "somee.com"

30th August
Add CompletedPollsModel
PollsDbModel
	add new property DateTime EndDate{get;set;}
	use datetime-local in createPoll.cshtml page
use hangfire to secheduler method PollEnding() to run when EndDate has reached
PollEnding(string pollId)
	Send Email to creator and participants that poll has expired
		RegisteredVoters registeredVoters = Get All RegisterVoters under current ElectionId
		If my userId exits in registeredVoters
		BackgroundJob.Sechedule SendEmail
	adds Ended Poll to the CompletedPollsTable
	removes Ended Poll from the PollsDbTable(Finally completed, Took me way too long to add this like 5 days :_) 6th September)
ShowWinners(string pollId)
	Get CompletedPoll from CompletedPollTable
	gets list of Candidates
	organize them in descending order of vote count
	return list
[HttpGet]CompletedElections()

View
	on the PollsPage
	<a href="CompletedElection">Show List of Completed Election</a>
	
	on the CompletedElection's page
	shows a list of compleleted Election using datatables
	column showWinners redirects to ShowWinners page
	shows top candidates in descending order
	link show all candidate redirects to show all candidates
