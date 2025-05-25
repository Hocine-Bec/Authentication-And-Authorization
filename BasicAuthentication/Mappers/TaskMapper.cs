using BasicAuthentication.DTOs;
using BasicAuthentication.Entities;

namespace BasicAuthentication.Mappers
{
    public static class TaskMapper
    {
        public static TodoTask MapUpdate(this TodoTask task, TaskRequest request)
        {
            task.Title = request.Title;
            task.Description = request.Description;
            task.TaskStatus = request.TaskStatus;
            task.UserId = request.UserId;
            task.DateRange = new TimeSlot
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            return task;
        }

        public static TodoTask MapCreate(TaskRequest request)
        {
            return new TodoTask()
            {
                Title = request.Title,
                Description = request.Description,
                TaskStatus = request.TaskStatus,
                UserId = request.UserId,
                DateRange = new TimeSlot
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate
                }
            };
        }
    }
}
