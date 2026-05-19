using Dapper;
using Microsoft.Extensions.Configuration;
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
    public async Task<GetAllTicketsQueryResponse> GetAllAsync(int page, int pageSize, string sortBy = "CreatedDate", string sortDirection = "desc", string? status = null, string search = "")
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var connectionString = configuration.GetConnectionString("PostgresSql")
            ?? throw new InvalidOperationException("Connection string 'PostgresSql' was not found.");

        await using var connection = new NpgsqlConnection(connectionString);

        // Parametreler ve WHERE clause oluştur
        var parameters = new DynamicParameters();
        var whereConditions = new List<string>();

        if (!string.IsNullOrWhiteSpace(status))
        {
            whereConditions.Add("\"Status\" = @Status");
            parameters.Add("Status", status);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            whereConditions.Add("(\"Title\" ILIKE @Search OR \"Description\" ILIKE @Search)");
            parameters.Add("Search", $"%{search}%");
        }

        var whereClause = whereConditions.Count > 0 ? "WHERE " + string.Join(" AND ", whereConditions) : "";

        // Toplam kayıt sayısı
        var countSql = $"""
            SELECT COUNT(1)
            FROM "Tickets"
            {whereClause}
            """;

        var totalCount = await connection.QuerySingleAsync<int>(countSql, parameters);

        // Sort validation (SQL injection koruması)
        var sortColumn = sortBy?.ToLowerInvariant() switch
        {
            "title" => "\"Title\"",
            "status" => "\"Status\"",
            "createddate" => "\"CreatedDate\"",
            _ => "\"CreatedDate\""
        };

        var direction = sortDirection?.ToLowerInvariant() == "asc" ? "ASC" : "DESC";

        // Sayfalama parametreleri
        var offset = (page - 1) * pageSize;
        parameters.Add("PageSize", pageSize);
        parameters.Add("Offset", offset);

        var sql = $"""
            SELECT "Id", "Title", "Status", "CreatedDate"
            FROM "Tickets"
            {whereClause}
            ORDER BY {sortColumn} {direction}
            LIMIT @PageSize OFFSET @Offset
            """;

        var rows = await connection.QueryAsync<TicketRow>(sql, parameters);

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

    public async Task<GetOpenTicketsQueryResponse> GetOpenTicketsAsync(int page, int pageSize, string sortBy = "CreatedDate", string sortDirection = "desc",
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
        
        var sortColumn = sortBy?.ToLowerInvariant() switch
        {
            "title" => "\"Title\"",
            "status" => "\"Status\"",
            "createddate" => "\"CreatedDate\"",
            _ => "\"CreatedDate\""
        };
        var direction = sortDirection?.ToLowerInvariant() == "asc" ? "ASC" : "DESC";
        
        var sql = $"""
            SELECT "Id", "Title", "Status", "CreatedDate"
            FROM "Tickets"
            WHERE "Status" = @Status
            ORDER BY {sortColumn} {direction}
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