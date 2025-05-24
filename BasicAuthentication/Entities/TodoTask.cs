using BasicAuthentication.Enums;

namespace BasicAuthentication.Entities
{
    public class TodoTask
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public Status TaskStatus { get; set; }
        public TimeSlot? DateRange { get; set; }
    }

    public class TimeSlot
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public override string ToString()
        {
            return $"{StartDate.ToString("yyyy/MM/dd")} -- {EndDate.ToString("yyyy/MM/dd")}";
        }
    }
}
