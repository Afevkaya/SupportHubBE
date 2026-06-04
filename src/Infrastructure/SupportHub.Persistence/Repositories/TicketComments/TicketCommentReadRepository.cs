using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SupportHub.Application.Abstractions.Repositories.TicketComments;
using SupportHub.Domain.Entities;

namespace Persistence.Repositories.TicketComments;

public class TicketCommentReadRepository(IConfiguration configuration) : ITicketCommentReadRepository
{
    public async Task<List<TicketComment>> GetCommentsByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        var connectionString = configuration.GetConnectionString("PostgresSql") 
                               ?? throw new InvalidOperationException("Connection string 'PostgresSql' was not found.");
        await using var connection = new NpgsqlConnection(connectionString);
        
        var sql = @"SELECT ""Id"", ""TicketId"", ""Message"", ""AuthorUserId"", ""CreatedDate""
                    FROM ""TicketComments""
                    WHERE ""TicketId"" = @TicketId
                    ORDER BY ""CreatedDate"" ASC";
        
         var ticketComments = await connection.QueryAsync<TicketComment>(sql, new { TicketId = ticketId });
         return ticketComments.ToList();
    }
}