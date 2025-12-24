namespace TaskTracker.Models
{
    public class ProjectEmployee
    {

        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public string EmployeeId { get; set; }
        public AppUser Employee { get; set; }
    }
}
