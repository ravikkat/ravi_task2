using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QueueTaskConsumeAPI.Models;

using System.Text;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace QueueTaskConsumeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskConsumesController : ControllerBase
    {
        private readonly TaskConsumeDBContext _context;
        

        private IConnection _connection;
        private IModel _channel;
        public TaskConsumesController(TaskConsumeDBContext context)
        {
            _context = context;
        }

        // GET: api/TaskConsumes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskConsume>>> GetTodoItems()
        {
           
            return await _context.TodoItems.ToListAsync();
        }

        // GET: api/TaskConsumes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskConsume>> GetTaskConsume(int id)
        {
           
            var taskConsume = await _context.TodoItems.FindAsync(id);

            if (taskConsume == null)
            {
                return NotFound();
            }

            return taskConsume;
        }

        // PUT: api/TaskConsumes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskConsume(int id, TaskConsume taskConsume)
        {
            if (id != taskConsume.TaskId)
            {
                return BadRequest();
            }

            _context.Entry(taskConsume).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskConsumeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TaskConsumes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TaskConsume>> PostTaskConsume(TaskConsume taskConsume)
        {
            _context.TodoItems.Add(taskConsume);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTaskConsume", new { id = taskConsume.TaskId }, taskConsume);
        }

        // DELETE: api/TaskConsumes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskConsume(int id)
        {
            var taskConsume = await _context.TodoItems.FindAsync(id);
            if (taskConsume == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(taskConsume);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskConsumeExists(int id)
        {
            return _context.TodoItems.Any(e => e.TaskId == id);
        }

      

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }
        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
    }
}
