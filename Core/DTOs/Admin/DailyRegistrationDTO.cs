using System;

namespace Core.DTOs.Admin
{
    public class DailyRegistrationDTO
    {
        public DateTime Date { get; set; }
        public int RegistrationCount { get; set; }
    }
}