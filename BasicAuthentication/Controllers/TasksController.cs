using BasicAuthentication.Authorization;
using BasicAuthentication.Data;
using BasicAuthentication.DTOs;
using BasicAuthentication.Entities;
using BasicAuthentication.Enums;
using BasicAuthentication.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BasicAuthentication.Controllers
{
    [Route("Tasks")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [CheckPermission(Permission.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TodoTask>>> GetAll()
        {
            var tasks = await _context.Tasks
                .AsNoTracking()
                .ToListAsync();
            
            return Ok(tasks);
        }


        [HttpGet]
        [Route("{taskId}")]
        [Authorize(Policy = "AdminOrCanRead")]
        [CheckPermission(Permission.Read)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<TodoTask>> GetById(int taskId)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (taskId <= 0)
                return BadRequest("Task ID must be greater than 0");

            var task = await _context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == taskId);
            
            if (task == null)
                return NotFound($"Task with ID {taskId} not found");

            return Ok(task);
        }


        [HttpPost]
        [CheckPermission(Permission.Create)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TodoTask>> Create(TaskRequest request)
        {
            if (request == null)
                return BadRequest("Task data is required");

            var task = TaskMapper.MapCreate(request);

            await _context.AddAsync(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { taskId = task.Id }, task);
        }

      

        [HttpPut]
        [Route("{taskId}")]
        [CheckPermission(Permission.Update)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> Update(int taskId, TaskRequest request)
        {
            if (taskId <= 0)
                return BadRequest("Task ID must be greater than 0");

            if (request == null)
                return BadRequest("Task data is required");

            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
                return NotFound($"Task with ID {taskId} not found");

            task.MapUpdate(request);

            _context.Update(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }

      

        [HttpDelete]
        [Route("{taskId}")]
        [CheckPermission(Permission.Delete)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Delete(int taskId)
        {
            if (taskId <= 0)
                return BadRequest("Task ID must be greater than 0");

            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
                return NotFound($"Task with ID {taskId} not found");

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }



        [HttpPatch]
        [Route("{taskId}")]
        [CheckPermission(Permission.Update)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Complete(int taskId)
        {
            if (taskId <= 0)
                return BadRequest("Task ID must be greater than 0");

            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
                return NotFound($"Task with ID {taskId} not found");

            if (task.TaskStatus == TodoTaskStatus.Completed)
                return NoContent(); 

            task.TaskStatus = TodoTaskStatus.Completed;

            task.DateRange ??= new TimeSlot { StartDate = DateTime.UtcNow };
            task.DateRange.EndDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
