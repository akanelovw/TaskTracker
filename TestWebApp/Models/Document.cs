namespace TaskTracker.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
