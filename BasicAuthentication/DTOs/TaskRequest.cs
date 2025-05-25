using BasicAuthentication.Enums;

namespace BasicAuthentication.DTOs
{
    public class TaskRequest
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public TodoTaskStatus TaskStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? UserId { get; set; }
    }
}
