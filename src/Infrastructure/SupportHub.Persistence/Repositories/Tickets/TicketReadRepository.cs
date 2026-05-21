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
    public async Task<GetAllTicketsQueryResponse> GetAllAsync(int page, int pageSize, string sortBy = "CreatedDate",
        string sortDirection = "desc", string? status = null, string? priority = null, string search = "")
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var connectionString = configuration.GetConnectionString("PostgresSql")
            ?? throw new InvalidOperationException("Connection string 'PostgresSql' was not found.");

        await using var connection = new NpgsqlConnection(connectionString);

        // Parametreler ve WHERE clause oluştur
        var parameters = new DynamicParameters();
        var whereConditions = new List<string>();

        // Status: int değerini enum'a çevir, sonra string'e dönüştür
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (int.TryParse(status, out var statusValue))
            {
                try
                {
                    var statusEnum = (TicketStatusType)statusValue;
                    whereConditions.Add("\"Status\" = @Status");
                    parameters.Add("Status", statusEnum.ToString());
                }
                catch
                {
                    // Geçersiz enum değeri, filtre uygulanmaz
                }
            }
        }
        
        // Priority: int değerini enum'a çevir, sonra string'e dönüştür
        if (!string.IsNullOrWhiteSpace(priority))
        {
            if (int.TryParse(priority, out var priorityValue))
            {
                try
                {
                    var priorityEnum = (TicketPriorityType)priorityValue;
                    whereConditions.Add("\"Priority\" = @Priority");
                    parameters.Add("Priority", priorityEnum.ToString());
                }
                catch
                {
                    // Geçersiz enum değeri, filtre uygulanmaz
                }
            }
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
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        if (totalPages > 0 && page > totalPages)
        {
            page = totalPages;
        }

        // Sort validation (SQL injection koruması)
        var sortColumn = sortBy?.ToLowerInvariant() switch
        {
            "title" => "\"Title\"",
            "status" => "\"Status\"",
            "createddate" => "\"CreatedDate\"",
            "priority" => "\"Priority\"",
            _ => "\"CreatedDate\""
        };

        var direction = sortDirection?.ToLowerInvariant() == "asc" ? "ASC" : "DESC";

        // Sayfalama parametreleri
        var offset = (page - 1) * pageSize;
        parameters.Add("PageSize", pageSize);
        parameters.Add("Offset", offset);

        var sql = $"""
            SELECT "Id", "Title", "Status", "Priority", "CreatedDate"
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
                x.Priority,
                x.CreatedDate))
            .ToList();


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
            SELECT "Id", "Title", "Status", "Priority", "CreatedDate"
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
                x.Priority,
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
            SELECT
                t."Id", t."Title", t."Description", t."Status", t."Priority", t."CreatedDate", t."UpdatedDate",
                c."Id", c."Message", c."AuthorName", c."TicketId", c."CreatedDate",
                a."Id", a."ActivityType", a."Description", a."TicketId", a."CreatedDate"
            FROM "Tickets" t
            LEFT JOIN "TicketComments" c ON c."TicketId" = t."Id"
            LEFT JOIN "TicketActivities" a ON a."TicketId" = t."Id"
            WHERE t."Id" = @Id
            ORDER BY c."CreatedDate" ASC, a."CreatedDate" ASC
            """;

        var ticketLookup = new Dictionary<Guid, Ticket>();

        await connection.QueryAsync<Ticket, TicketComment, TicketActivity, Ticket>(
            sql,
            (ticket, comment, activity) =>
            {
                if (!ticketLookup.TryGetValue(ticket.Id, out var ticketEntry))
                {
                    ticketEntry = ticket;
                    ticketEntry.TicketComments = new List<TicketComment>();
                    ticketEntry.TicketActivities = new List<TicketActivity>();
                    ticketLookup.Add(ticketEntry.Id, ticketEntry);
                }

                if (comment.Id != Guid.Empty && ticketEntry.TicketComments.All(c => c.Id != comment.Id))
                {
                    ticketEntry.TicketComments.Add(comment);
                }

                if (activity.Id != Guid.Empty && ticketEntry.TicketActivities.All(a => a.Id != activity.Id))
                {
                    ticketEntry.TicketActivities.Add(activity);
                }

                return ticketEntry;
            },
            new { Id = id },
            splitOn: "Id,Id");

        return ticketLookup.Values.FirstOrDefault();
    }

    public async Task<bool> GetByIdAsync(Guid id)
    {
        var connectionString = configuration.GetConnectionString("PostgresSql")
            ?? throw new InvalidOperationException("Connection string 'PostgresSql' was not found.");

        await using var connection = new NpgsqlConnection(connectionString);

        const string sql = """
            SELECT EXISTS(
                SELECT 1
                FROM "Tickets"
                WHERE "Id" = @Id
            )
            """;

        return await connection.QuerySingleAsync<bool>(sql, new { Id = id });
    }

    private sealed class TicketRow
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public string Priority { get; init; } = string.Empty;
        public DateTime CreatedDate { get; init; }
    }
}