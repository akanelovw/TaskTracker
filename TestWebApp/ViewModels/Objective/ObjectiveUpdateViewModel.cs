using TestWebApp.Models;

namespace TestWebApp.ViewModels.Objectives
{
    public class ObjectiveUpdateViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? AssignedEmployeeId { get; set; }
        public ObjectiveStatus Status { get; set; }
        public string Comment { get; set; }
        public int Priority { get; set; }
    }
}
