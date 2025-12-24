using System.ComponentModel.DataAnnotations;

namespace TestWebApp.ViewModels.Project
{
    public class ProjectUpdateViewModel : IValidatableObject
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CustomerCompany { get; set; }
        public string ExecutorCompany { get; set; }
        public string ManagerId { get; set; }
        public ICollection<String> EmployeesId { get; set; } = new List<string>();
        [Required(ErrorMessage = "The start date is required")]
        [Display(Name = "Start Date:")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime StartTime { get; set; }
        [Required(ErrorMessage = "The end date is required")]
        [Display(Name = "End Date:")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime EndTime { get; set; }
        [Required(ErrorMessage = "The priority is required")]
        [Display(Name = "Priority in range from 1 to 3:")]
        [Range(1, 3, ErrorMessage = "Please enter valid number")]
        public int Priority { get; set; }
        public IFormFileCollection Documents { get; set; }

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            if (EndTime < StartTime)
            {
                yield return new ValidationResult("EndDate must be greater than StartDate");
            }
        }
    }
}
