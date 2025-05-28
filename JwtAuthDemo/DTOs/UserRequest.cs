using JwtAuthDemo.Enums;

namespace JwtAuthDemo.DTOs
{
    public class UserResponse
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public string? Email { get; set; }
    }

    public class UserWithTaskRequest
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public string? Email { get; set; }
        public List<TaskSummaryResponse> AssignedTasks { get; set; } = new List<TaskSummaryResponse>();
    }

    public class TaskSummaryResponse
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public TodoTaskStatus TaskStatus { get; set; }
    }

    public class CreateUserRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public string? Email { get; set; }
    }

   

    public class UpdateUserRequest
    {
        public required string Username { get; set; }
        public string? Email { get; set; }
    }
}
