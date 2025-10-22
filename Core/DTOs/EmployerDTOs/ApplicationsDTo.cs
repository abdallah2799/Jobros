using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.EmployerDTOs
{
    public class ApplicationsDTo
    {
        public int Id { get; set; }                  // رقم الطلب
        public string JobTitle { get; set; }         // اسم الوظيفة
        public string ApplicantName { get; set; }    // اسم الشخص المتقدم
        public string ApplicantEmail { get; set; }   // الإيميل (لو محتاجه)
        public DateTime AppliedAt { get; set; }      // تاريخ التقديم
        public string Status { get; set; }           // Pending - Accepted - Rejected
    }
}
