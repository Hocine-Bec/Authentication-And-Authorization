using BasicAuthentication.Enums;

namespace BasicAuthentication.Entities
{
    public class TodoTask
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public TodoTaskStatus TaskStatus { get; set; }
        public TimeSlot? DateRange { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }
    }

    public class TimeSlot
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public override string ToString()
        {
            var start = StartDate?.ToString("yyyy/MM/dd") ?? "Not started";
            var end = EndDate?.ToString("yyyy/MM/dd") ?? "No end date";

            return $"{start} -- {end}";
        }
    }
}
