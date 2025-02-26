using Microsoft.EntityFrameworkCore;
using Rirais.Task.Grpc.Services;
using Rirais.Task.GrpcServer.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

/// add context to project because it's not real project i use memory instead of real sql server.
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMemoryDb"));

/// Add services to the container.
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});

/// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/myapp.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(); /// Use Serilog for logging

var app = builder.Build();

/// Configure the HTTP request pipeline.
app.MapGrpcService<PersonService>();
app.MapGet("/", () => "Use a gRPC client to communicate with the server.");

app.Run();
