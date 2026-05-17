using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.Repositories.Tickets;
using SupportHub.Api.Middlewares;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Application.Services.Tickets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SupportHubDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresSql")));
builder.Services.AddScoped<ITicketCommandService, TicketCommandService>();
builder.Services.AddScoped<ITicketQueryService, TicketQueryService>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Global exception middleware should be early in the pipeline so it can catch exceptions
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();