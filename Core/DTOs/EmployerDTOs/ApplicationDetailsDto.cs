using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.EmployerDTOs
{
    public class ApplicationDetailsDto
    {
        public int Id { get; set; }
        public string JobTitle { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantEmail { get; set; }
        public string CoverLetter { get; set; }
        public string CvFilePath { get; set; } // مكان الملف المرفوع لو موجود
        public DateTime AppliedAt { get; set; }
        public string Status { get; set; }
    }
}
