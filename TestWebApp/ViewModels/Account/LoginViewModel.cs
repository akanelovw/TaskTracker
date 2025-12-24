using System.ComponentModel.DataAnnotations;

namespace TaskTracker.ViewModels.Account
{
    public class LoginViewModel
    {
        [Display(Name = "Email Address")]
        public string EmailAddress  { get; set; }
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
