using System.ComponentModel.DataAnnotations;

namespace TaskTracker.ViewModels.Project
{
    public class ProjectViewModel
    {
        public string Title { get; set; }
        public string CustomerCompany { get; set; }
        public string ExecutorCompany { get; set; }
        public string ManagerId { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime StartTime { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime EndTime { get; set; }
        public int Priority { get; set; }
    }
}
