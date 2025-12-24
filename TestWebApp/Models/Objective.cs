using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models
{
    public class Objective
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorId { get; set; }
        public AppUser Author { get; set; }
        public string? AssignedEmployeeId { get; set; }
        public AppUser AssignedEmployee { get; set; }
        [Required]
        public ObjectiveStatus Status { get; set; }
        public string Comment { get; set; }
        public int Priority { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
    public enum ObjectiveStatus
    {
        ToDo,
        InProgress,
        Done
    }
}
