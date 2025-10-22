using System;

namespace Core.DTOs.Admin
{
    public class JobAdminViewDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}