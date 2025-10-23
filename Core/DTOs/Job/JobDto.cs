namespace Core.DTOs.Job
{
    public class JobDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public string Location { get; set; }
        public string SalaryRange { get; set; }
        public string JobType { get; set; }
        public bool IsActive { get; set; }
        public string EmployerName { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
