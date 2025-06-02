using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace TodoApi.Controllers
{
    [Route("todos")] // Explicitly set route to "todos"
    [ApiController]
    public class TodosController : ControllerBase
    {
        private static readonly List<TodoItem> _todos = new List<TodoItem>();
        private static long _nextId = 1;

        // GET: /todos
        [HttpGet]
        public ActionResult<IEnumerable<TodoItem>> GetTodoItems()
        {
            if (!_todos.Any())
            {
                // Return empty list directly, which is fine for the frontend
                return Ok(new List<TodoItem>());
            }
            return Ok(_todos);
        }

        // POST: /todos
        [HttpPost]
        public ActionResult<TodoItem> PostTodoItem(TodoItemInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Task))
            {
                return BadRequest(new { error = "Task is required" });
            }

            var todoItem = new TodoItem
            {
                Id = _nextId++,
                Task = input.Task,
                IsComplete = false
            };

            _todos.Add(todoItem);
            // The frontend expects the newly created todo item back, matching the Python version.
            return StatusCode(201, todoItem);
        }

        // GET: /todos/{id} - Helper for potential future use or direct API testing
        [HttpGet("{id}")]
        public ActionResult<TodoItem> GetTodoItemById(long id)
        {
            var todoItem = _todos.FirstOrDefault(t => t.Id == id);
            if (todoItem == null)
            {
                return NotFound(new { error = "Todo not found" });
            }
            return Ok(todoItem);
        }

        // PUT: /todos/{id}
        [HttpPut("{id}")]
        public IActionResult PutTodoItem(long id, TodoItemUpdateInput input)
        {
            var todoItem = _todos.FirstOrDefault(t => t.Id == id);
            if (todoItem == null)
            {
                return NotFound(new { error = "Todo not found" });
            }

            // The input DTO already maps "completed" JSON property to the Completed C# property
            todoItem.IsComplete = input.Completed;

            return Ok(todoItem); // Return the updated item as per frontend expectation
        }
    }

    // DTO for POST input
    public class TodoItemInput
    {
        [JsonPropertyName("task")]
        public string? Task { get; set; }
    }

    // DTO for PUT input
    public class TodoItemUpdateInput
    {
        // This property name must match the JSON sent by the frontend.
        // The frontend sends: { "completed": true/false }
        [JsonPropertyName("completed")]
        public bool Completed { get; set; }
    }
}
