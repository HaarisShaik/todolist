using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(new TodoRepository());
builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/", () => "Hello Shaik!");
app.MapGet("/michael", () => "Hello Micky!");

app.MapControllers();
app.Run();


public class TodoItem
{
    public string Text { get; set; }
}

public class TodoRepository
{
    private IList<TodoItem> Todos { get; set; } = new List<TodoItem>();

    public IList<TodoItem> GetTodos()
    {
        return Todos.ToList();
    }

    public void AddTodo(TodoItem todo)
    {
        Todos.Add(todo);
    }
}

[ApiController]
public class TodoController : ControllerBase
{
    public TodoController(TodoRepository todoRepository)
    {
        TodoRepository = todoRepository;
    }

    private TodoRepository TodoRepository { get; }

    [HttpGet]
    [Route("todo")]
    public ActionResult<IList<TodoItem>> GetTodos()
    {
        return Ok(TodoRepository.GetTodos());
    }

    [HttpPost]
    [Route("todo")]
    public ActionResult PostTodo([FromBody] TodoItem todoItem, CancellationToken cancellationToken)
    {
        TodoRepository.AddTodo(todoItem);
        return Ok();
    }
}