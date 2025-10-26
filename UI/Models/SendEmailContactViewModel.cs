using System.ComponentModel.DataAnnotations;

public class SendEmailContactViewModel
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Subject { get; set; }

    [Required]
    public string Message { get; set; }

    public bool SubscribeToNewsletter { get; set; }
}