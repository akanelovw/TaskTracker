using System.ComponentModel.DataAnnotations;
using TestWebApp.Models;

namespace TestWebApp.ViewModels.Project
{
    public class ProjectDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CustomerCompany { get; set; }
        public string ExecutorCompany { get; set; }
        public string ManagerId { get; set; }
        public AppUser Manager { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime StartTime { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime EndTime { get; set; }
        public int Priority { get; set; }
        public ICollection<Objective> Objectives { get; set; }
        public ICollection<AppUser> Employees { get; set; }
        public ICollection<Document> Documents { get; set; }
    }
}
