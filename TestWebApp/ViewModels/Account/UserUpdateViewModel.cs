using System.ComponentModel.DataAnnotations;

namespace TaskTracker.ViewModels.Account
{
    public class UserUpdateViewModel
    {
        public string Id { get; set; }
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email is required")]
        public string EmailAddress { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
    }
}
