using Microsoft.AspNetCore.Identity;

namespace TestWebApp.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? MiddleName { get; set; }
        public string FullName => $"{FirstName} {LastName} {MiddleName}";
        public string Email { get; set; }
        public ICollection<Project> ManagedProjects { get; set; } = new List<Project>();
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<Objective> Objectives { get; set; } = new List<Objective>();
        public ICollection<Objective> AssignedObjectives { get; set; } = new List<Objective>();
        public ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();

    }
}
