﻿namespace BasicAuthentication.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public string? Email { get; set; }

        public ICollection<TodoTask> AssignedTasks{ get; set; } = new List<TodoTask>();
    }
}
