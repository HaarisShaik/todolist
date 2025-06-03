using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(new TodoRepository());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/", () => "Hello Shaik!");
app.MapGet("/michael", () => "Hello Micky!");

app.UseCors(a => a.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";
});

app.MapControllers();
app.Run();


public struct TodoItem
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

    public void CompleteTodo(string text)
    {
        Todos.Remove(new TodoItem() { Text = text });
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

    [HttpDelete]
    [Route("todo")]
    public ActionResult DeleteTodo([FromBody] TodoItem todoItem)
    {
        TodoRepository.CompleteTodo(todoItem.Text);
        return Ok();
    }
}