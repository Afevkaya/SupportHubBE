using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.DTOs.Responses;
using SupportHub.Domain.Enums;

namespace Persistence.Repositories.Tickets;

public class TicketReadRepository(IConfiguration configuration) : ITicketReadRepository
{
    public async Task<List<ResponseGetTicket>> GetAllAsync()
    {
        var connectionString = configuration.GetConnectionString("PostgresSql")
            ?? throw new InvalidOperationException("Connection string 'PostgresSql' was not found.");

        await using var connection = new NpgsqlConnection(connectionString);

        const string sql = """
            SELECT "Id", "Title", "Status", "CreatedDate"
            FROM "Tickets"
            ORDER BY "CreatedDate" DESC
            """;

        var rows = await connection.QueryAsync<TicketRow>(sql);

        return rows
            .Select(x => new ResponseGetTicket(
                x.Id,
                x.Title,
                x.Status,
                x.CreatedDate))
            .ToList();
    }

    public async Task<List<ResponseGetTicket>> GetOpenTicketsAsync()
    {
        var connectionString = configuration.GetConnectionString("PostgresSql")
            ?? throw new InvalidOperationException("Connection string 'PostgresSql' was not found.");

        await using var connection = new NpgsqlConnection(connectionString);

        const string sql = """
            SELECT "Id", "Title", "Status", "CreatedDate"
            FROM "Tickets"
            WHERE "Status" = @Status
            ORDER BY "CreatedDate" DESC
            """;

        var rows = await connection.QueryAsync<TicketRow>(sql, new
        {
            Status = TicketStatusType.Open.ToString()
        });

        return rows
            .Select(x => new ResponseGetTicket(
                x.Id,
                x.Title,
                x.Status,
                x.CreatedDate))
            .ToList();
    }

    private sealed class TicketRow
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedDate { get; init; }
    }
}