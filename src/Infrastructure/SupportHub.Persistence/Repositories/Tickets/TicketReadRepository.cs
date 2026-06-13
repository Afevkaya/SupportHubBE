using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.DTOs.Responses.Tickets;
using SupportHub.Application.Features.Tickets.Queries.GetAllTickets;
using SupportHub.Domain.Entities;
using SupportHub.Domain.Enums;

namespace Persistence.Repositories.Tickets;

public class TicketReadRepository(IConfiguration configuration) : ITicketReadRepository
{
    public async Task<GetAllTicketsQueryResponse> GetAllAsync(int page, int pageSize, Guid? createdByUserId,
        Guid? assignedAgentId,
        string? sortBy = "createdDate",
        string? sortDirection = "desc", string? status = null, string? priority = null, string? search = null)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var connectionString = configuration.GetConnectionString("PostgresSql")
            ?? throw new InvalidOperationException("Connection string 'PostgresSql' was not found.");

        await using var connection = new NpgsqlConnection(connectionString);

        // Parametreler ve WHERE clause oluştur
        var parameters = new DynamicParameters();
        var whereConditions = new List<string>();
        
        if (createdByUserId.HasValue)
        {
            whereConditions.Add("\"CreatedByUserId\" = @CreatedByUserId");
            parameters.Add("CreatedByUserId", createdByUserId.Value);
        }

        if (assignedAgentId.HasValue)
        {
            whereConditions.Add("\"AssignedAgentId\" = @AssignedAgentId");
            parameters.Add("AssignedAgentId", assignedAgentId.Value);
        }

        // Status değerini enum'a çevir, sonra string'e dönüştür
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (Enum.TryParse<TicketStatusType>(status, true, out var statusEnum) &&
                Enum.IsDefined(typeof(TicketStatusType), statusEnum))
            {
                whereConditions.Add("\"Status\" = @Status");
                parameters.Add("Status", statusEnum.ToString());
            }
        }
        
        // Priority değerini enum'a çevir, sonra string'e dönüştür
        if (!string.IsNullOrWhiteSpace(priority))
        {
            if (Enum.TryParse<TicketPriorityType>(priority, true, out var priorityEnum) &&
                Enum.IsDefined(typeof(TicketPriorityType), priorityEnum))
            {
                whereConditions.Add("\"Priority\" = @Priority");
                parameters.Add("Priority", priorityEnum.ToString());
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

    public async Task<Ticket?> GetTicketAsync(Guid id)
    {
        var connectionString = configuration.GetConnectionString("PostgresSql")
            ?? throw new InvalidOperationException("Connection string 'PostgresSql' was not found.");

        await using var connection = new NpgsqlConnection(connectionString);

        const string sql = """
            SELECT "Id", "CreatedByUserId", "AssignedAgentId"
            FROM "Tickets"
            WHERE "Id" = @Id
            """;

        return await connection.QuerySingleOrDefaultAsync<Ticket>(sql, new { Id = id });
    }
    public async Task<Ticket?> GetTicketDetail(Guid id, Guid? createdByUserId, Guid? assignedAgentId)
    {
        var connectionString = configuration.GetConnectionString("PostgresSql")
            ?? throw new InvalidOperationException("Connection string 'PostgresSql' was not found.");

        await using var connection = new NpgsqlConnection(connectionString);

        const string sql = """
            SELECT
                "Id", "Title", "Description", "Status", "Priority", "CreatedDate", "UpdatedDate"
            FROM "Tickets"
            WHERE "Id" = @Id
              AND (@CreatedByUserId IS NULL OR "CreatedByUserId" = @CreatedByUserId)
              AND (@AssignedAgentId IS NULL OR "AssignedAgentId" = @AssignedAgentId);

            SELECT
                "Id", "Message", "AuthorUserId", "TicketId", "CreatedDate"
            FROM "TicketComments"
            WHERE "TicketId" = @Id
            ORDER BY "CreatedDate" ASC;

            SELECT
                "Id", "ActivityType", "Description", "TicketId", "CreatedDate"
            FROM "TicketActivities"
            WHERE "TicketId" = @Id
            ORDER BY "CreatedDate" ASC;
            """;

        await using var result = await connection.QueryMultipleAsync(sql,
            new { Id = id, CreatedByUserId = createdByUserId, AssignedAgentId = assignedAgentId });

        var ticket = await result.ReadSingleOrDefaultAsync<Ticket>();
        if (ticket is null)
        {
            return null;
        }

        ticket.TicketComments = (await result.ReadAsync<TicketComment>()).ToList();
        ticket.TicketActivities = (await result.ReadAsync<TicketActivity>()).ToList();

        return ticket;
    }
    public async Task<bool> AnyTicketAsync(Guid id)
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
