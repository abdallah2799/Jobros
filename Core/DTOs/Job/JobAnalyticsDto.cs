namespace Core.DTOs.Job
{
    public class JobAnalyticsDto
    {
        public int TotalApplicants { get; set; }
        public int AcceptedCount { get; set; }
        public int RejectedCount { get; set; }
        public int PendingCount { get; set; }
    }
}