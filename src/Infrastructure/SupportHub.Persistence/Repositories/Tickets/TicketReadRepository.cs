using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.DTOs.Responses;
using SupportHub.Application.Features.Tickets.Queries.Tickets.GetAllTickets;
using SupportHub.Application.Features.Tickets.Queries.Tickets.GetOpenTickets;
using SupportHub.Domain.Entities;
using SupportHub.Domain.Enums;

namespace Persistence.Repositories.Tickets;

public class TicketReadRepository(IConfiguration configuration) : ITicketReadRepository
{
    public async Task<GetAllTicketsQueryResponse> GetAllAsync(int page, int pageSize)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var connectionString = configuration.GetConnectionString("PostgresSql")
            ?? throw new InvalidOperationException("Connection string 'PostgresSql' was not found.");

        await using var connection = new NpgsqlConnection(connectionString);

        // Toplam kayıt sayısı
        const string countSql = """
            SELECT COUNT(1)
            FROM "Tickets"
            """;

        var totalCount = await connection.QuerySingleAsync<int>(countSql);

        // Sayfalama parametreleri
        var offset = (page - 1) * pageSize;

        const string sql = """
            SELECT "Id", "Title", "Status", "CreatedDate"
            FROM "Tickets"
            ORDER BY "CreatedDate" DESC
            LIMIT @PageSize OFFSET @Offset
            """;

        var rows = await connection.QueryAsync<TicketRow>(sql, new
        {
            PageSize = pageSize,
            Offset = offset
        });

        var items = rows
            .Select(x => new ResponseGetTicket(
                x.Id,
                x.Title,
                x.Status,
                x.CreatedDate))
            .ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        
        return new GetAllTicketsQueryResponse(
            items,
            page,
            pageSize,
            totalCount,
            totalPages);
    }

    public async Task<GetOpenTicketsQueryResponse> GetOpenTicketsAsync(int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var connectionString = configuration.GetConnectionString("PostgresSql")
                               ?? throw new InvalidOperationException("Connection string 'PostgresSql' was not found.");

        await using var connection = new NpgsqlConnection(connectionString);

        // Toplam kayıt sayısı
        const string countSql = """
            SELECT COUNT(1)
            FROM "Tickets"
            WHERE "Status" = @Status
            """;

        var totalCount = await connection.QuerySingleAsync<int>(countSql, new { Status = TicketStatusType.Open.ToString() });

        // Sayfalama parametreleri
        var offset = (page - 1) * pageSize;
        
        const string sql = """
            SELECT "Id", "Title", "Status", "CreatedDate"
            FROM "Tickets"
            WHERE "Status" = @Status
            ORDER BY "CreatedDate" DESC
            LIMIT @PageSize OFFSET @Offset
            """;

        var rows = await connection.QueryAsync<TicketRow>(sql, new
        {
            Status = TicketStatusType.Open.ToString(),
            PageSize = pageSize,
            Offset = offset
        });

        var items = rows
            .Select(x => new ResponseGetTicket(
                x.Id,
                x.Title,
                x.Status,
                x.CreatedDate))
            .ToList();
        
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        return new GetOpenTicketsQueryResponse(
            items,
            page,
            pageSize,
            totalCount,
            totalPages);
    }

    public async Task<Ticket?> GetTicketDetail(Guid id)
    {
        var connectionString = configuration.GetConnectionString("PostgresSql")
            ?? throw new InvalidOperationException("Connection string 'PostgresSql' was not found.");

        await using var connection = new NpgsqlConnection(connectionString);

        const string sql = """
            SELECT "Id", "Title", "Description", "Status", "CreatedDate", "UpdatedDate"
            FROM "Tickets"
            WHERE "Id" = @Id
            """;

        var ticket = connection.QuerySingleOrDefault<Ticket>(sql, new { Id = id });

        return ticket;
    }

    private sealed class TicketRow
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedDate { get; init; }
    }
}