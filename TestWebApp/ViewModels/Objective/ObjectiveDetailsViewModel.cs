using TaskTracker.Models;

namespace TaskTracker.ViewModels.Objectives
{
    public class ObjectiveDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public AppUser AssignedEmployee { get; set; }
        public AppUser Author { get; set; }
        public ObjectiveStatus Status { get; set; }
        public string Comment { get; set; }
        public int Priority { get; set; }
        public int ProjectId { get; set; }
    }
}
