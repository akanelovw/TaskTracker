using System.ComponentModel.DataAnnotations;

namespace TaskTracker.ViewModels.Account
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
    }
}
