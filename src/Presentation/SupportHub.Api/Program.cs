using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.Repositories.Tickets;
using SupportHub.Api.Middlewares;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SupportHubDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresSql")));
builder.Services.AddApplicationServices();
builder.Services.AddScoped<ITicketReadRepository, TicketReadRepository>();
builder.Services.AddScoped<ITicketWriteRepository, TicketWriteRepository>();
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