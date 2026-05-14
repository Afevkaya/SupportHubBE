using Microsoft.AspNetCore.Mvc;
using SupportHub.Api.Entities;
using SupportHub.Api.Enums;
using SupportHub.Api.Models.Requests;
using SupportHub.Api.Models.Responses;

namespace SupportHub.Api.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private static readonly List<Ticket> Tickets = [];
        
        [HttpGet]
        public IActionResult GetTickets()
        {
            var response = Tickets.Select(t => new ResponseGetTicket(
                t.Id,
                t.Title,
                t.Status,
                t.CreatedDate
            )).ToList();
            return Ok(response);
        }
        
        
        [HttpPost]
        public IActionResult CreateTicket([FromBody] RequestCreateTicket request)
        {
            if(request == null) return BadRequest("Request body is null.");
            
            if(string.IsNullOrEmpty(request.Title)) return BadRequest("Title is required.");
            
            if(string.IsNullOrEmpty(request.Description)) return BadRequest("Description is required.");
            
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Status = nameof(TicketStatusType.Open),
                CreatedDate = DateTime.UtcNow
            };
            Tickets.Add(ticket);
            var response = new ResponseCreateTicket(
                ticket.Id,
                ticket.Title,
                ticket.Description,
                ticket.Status,
                ticket.CreatedDate
            );
            return Ok(response);
        }
    }
}
