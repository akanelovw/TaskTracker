namespace TestWebApp.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CustomerCompany {  get; set; }
        public string ExecutorCompany { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Priority { get; set; }

        public string? ManagerId { get; set; }
        public AppUser Manager { get; set; }
        public ICollection<Objective> Objectives { get; set; } = new List<Objective>();
        public ICollection<AppUser> Employees { get; set; } = new List<AppUser>();
        public ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();
        public ICollection<Document> Documents { get; set; } = new List<Document>();

    }
}
