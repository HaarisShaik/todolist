using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(c => c.UseSqlite("Data Source=TodoList.db"));

var app = builder.Build();

app.MapGet("/", () => "Hello Shaik!");
app.MapGet("/michael", () => "Hello Micky!");

using (var scope = app.Services.CreateScope()) {
    scope.ServiceProvider.GetService<DataContext>()!.Database.EnsureCreated();
}

app.UseCors(a => a.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";
});

app.MapControllers();
app.Run();


public class TodoItem
{
    public int Id { get; set; }
    public string Text { get; set; }
}

public class DataContext: DbContext
{
    public DataContext(DbContextOptions<DataContext> dbContextOptions) : base(dbContextOptions) {}
    public DbSet<TodoItem> TodoItems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>()
            .HasKey(e => e.Id);
        
        modelBuilder.Entity<TodoItem>()
            .Property(b => b.Id)
            .ValueGeneratedOnAdd();
    }
}

[ApiController]
public class TodoController : ControllerBase
{
    public TodoController(DataContext dataContext)
    {
        DataContext = dataContext;
    }

    private DataContext DataContext { get; }


    [HttpGet]
    [Route("todo")]
    public ActionResult<IList<TodoItem>> GetTodos()
    {
        return Ok(DataContext.TodoItems.ToList());
    }

    [HttpPost]
    [Route("todo")]
    public ActionResult PostTodo([FromBody] AddTodoDto todoItem, CancellationToken cancellationToken)
    {
        DataContext.TodoItems.Add(new TodoItem() { Text = todoItem.Text});
        DataContext.SaveChanges();
        return Ok();
    }

    [HttpDelete]
    [Route("todo")]
    public ActionResult DeleteTodo([FromBody] DeleteTodoDto todo)
    {
        var item = DataContext.TodoItems.SingleOrDefault(t => t.Id == todo.Id);
        if (item == null)
        {
            return NoContent();
        }
        DataContext.TodoItems.Remove(item);
        DataContext.SaveChanges();
        return Ok();
    }
}

public class AddTodoDto
{
    public string Text { get; set; }
}

public class DeleteTodoDto
{
    public int Id { get; set; }
}