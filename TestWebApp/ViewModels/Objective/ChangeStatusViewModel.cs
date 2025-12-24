using TaskTracker.Models;

namespace TaskTracker.ViewModels.Objectives
{
    public class ChangeStatusViewModel
    {
        public int Id { get; set; }
        public ObjectiveStatus Status { get; set; }
    }
}
