using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Minigames.Application;
using Minigames.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

// Inject infrastructure services
builder.Services
    .AddHttpContextAccessor()
    .AddAuthorization()
    .AddInfrastructureServices(builder.Configuration);

// Inject application services
builder.Services.AddApplicationServices();

// Cors
const string CORS_POLICY = "localhost_rule";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CORS_POLICY,
    policy =>
    {
        policy.WithOrigins("https://localhost:3000/", "https://localhost:3000", "https://127.0.0.1:3000/", "https://127.0.0.1:3000").AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Migrate infrastructure
await MinigamesContext.Migrate(app.Services);

// Exception handler
app.UseExceptionHandler(a => a.Run(async context =>
{
    var feature = context.Features.Get<IExceptionHandlerPathFeature>();
    var exception = feature!.Error;
    var result = JsonSerializer.Serialize(new { message = exception.Message, stackTrace = app.Environment.IsDevelopment() ? exception.ToString() : null });
    context.Response.ContentType = "application/json";
    if (exception is AppException appException)
    {
        context.Response.StatusCode = (int)appException.Code;
    }
    await context.Response.WriteAsync(result);
}));

app.UseHttpsRedirection();

app.UseCors(CORS_POLICY);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
