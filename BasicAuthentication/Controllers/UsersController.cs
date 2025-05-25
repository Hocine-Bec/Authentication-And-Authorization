using BasicAuthentication.Data;
using BasicAuthentication.DTOs;
using BasicAuthentication.Entities;
using BasicAuthentication.Enums;
using BasicAuthentication.Helper;
using BasicAuthentication.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasicAuthentication.Controllers
{
    [Route("Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
        {
            var response = await _context.Users
                .AsNoTracking()
                .Select(x => new UserResponse
                {
                    Id = x.Id,
                    Username = x.Username,
                    Email = x.Email
                })
                .ToListAsync();

            return Ok(response);
        }


        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserResponse>> GetById(int id)
        {
            if (id <= 0)
                return BadRequest("User ID must be greater than 0");

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (user == null)
                return NotFound($"User with ID {id} not found");

            var response = new UserResponse()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };
            return Ok(response);
        }


        [HttpGet]
        [Route("{id}/Tasks")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserWithTaskRequest>> GetUserWithTask(int id)
        {
            if (id <= 0)
                return BadRequest("User ID must be greater than 0");

            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.AssignedTasks)
                .FirstOrDefaultAsync(x => x.Id == id);
                
            if (user == null)
                return NotFound($"User with ID {id} not found");

            var response = new UserWithTaskRequest()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                AssignedTasks = user.AssignedTasks.Select(x => new TaskSummaryResponse()
                {
                    Id = x.Id,
                    Title = x.Title,
                    TaskStatus = x.TaskStatus
                }).ToList()
            };

            return Ok(response);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserResponse>> Create(CreateUserRequest request)
        {
            if (request == null)
                return BadRequest("User data is required");

            var user = new User()
            {
                Username = request.Username,
                Password = PasswordHelper.SetPassword(request.Password),
                Email = request.Email
            };

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            var response = new UserResponse()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            return CreatedAtAction(nameof(GetById), new { response.Id }, response);
        }


        [HttpPatch]
        [Route("{userId}/assign-tasks")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserWithTaskRequest>> AssignTasksToUsers(int userId, AssignTasksRequest request)
        {
            if (userId <= 0)
                return BadRequest("User ID must be greater than 0");

            if (!request.TaskIds.Any())
                return BadRequest("At least one task ID must be provided");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound($"User with ID {userId} not found");

            var availableTasks = await _context.Tasks
                .Where(t => request.TaskIds.Contains(t.Id) && t.UserId == null)
                .ToListAsync();

            if (!availableTasks.Any())
                return BadRequest("No unassigned tasks found from the provided task IDs");

            foreach (var task in availableTasks)
            {
                task.UserId = userId;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpPut]
        [Route("{Id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> Update(int id, UpdateUserRequest request)
        {
            if (id <= 0)
                return BadRequest("User ID must be greater than 0");

            if (request == null)
                return BadRequest("User data is required");

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound($"User with ID {id} not found");

            user.Username = request.Username;
            user.Email = request.Email;

            _context.Update(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }



        [HttpDelete]
        [Route("{Id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("User ID must be greater than 0");

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound($"User with ID {id} not found");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

}
