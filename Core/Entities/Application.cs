using System;
using System.Collections.Generic;

namespace Core.Entities;

public partial class Application
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public int JobSeekerId { get; set; }
   
    public string Status { get; set; } = "Pending";
    public DateTime AppliedAt { get; set; } = DateTime.Now;
    public string CoverLetter { get; set; }

    public virtual Job Job { get; set; }
    public virtual JobSeeker JobSeeker { get; set; }
}