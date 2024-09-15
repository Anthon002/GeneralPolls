namespace GeneralPolls.Core.DTOs
{
    public class CompletedPollsViewModel
    {
        public string Id { get; set; }
        public string ElectionName { get; set; }
        public string UserId{get;set;}
        public DateTime DateCreated{get; set;}
        public DateTime EndDate {get; set;}
    }
}