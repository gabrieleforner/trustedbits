using api_server.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);

ServicesBuilder.SetupServices(builder);

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.Services.GetRequiredService<IConnectionMultiplexer>();  // Force initialization of Redis connection

app.MapControllers();
app.MapHealthChecks("/health");
app.Run();