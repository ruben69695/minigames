using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Minigames.Application;
using Minigames.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddHttpContextAccessor()
    .AddAuthorization()
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Configure logging
builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSeq(builder.Configuration.GetSection("Seq")));

// Inject infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// Inject application services
builder.Services.AddApplication();

// Cors
const string CORS_POLICY = "localhost_rule";
builder.Services.AddCors(options => 
{
    options.AddPolicy(name: CORS_POLICY,
    policy => 
    { 
        policy.WithOrigins("https://localhost:3000/", "https://localhost:3000","https://127.0.0.1:3000/", "https://127.0.0.1:3000").AllowAnyHeader().AllowAnyMethod();
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
await Minigames.Infrastructure.Extensions.Migrate(app.Services);

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
