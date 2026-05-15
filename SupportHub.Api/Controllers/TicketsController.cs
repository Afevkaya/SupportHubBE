using Microsoft.AspNetCore.Mvc;
using SupportHub.Api.Models.Requests;
using SupportHub.Api.Services.Tickets;

namespace SupportHub.Api.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketsController(ITicketService ticketService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            var response = await ticketService.GetTicketsAsync();
            return Ok(response);
        }
        
        [HttpGet("open")]
        public async Task<IActionResult> GetOpenTickets()
        {
            var response = await ticketService.GetOpenTicketsAsync();
            return Ok(response);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] RequestCreateTicket request)
        {
            var response = await ticketService.CreateTicketAsync(request);
            return Ok(response);
        }
        
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateTicketStatus(Guid id, [FromBody] RequestUpdateTicketStatus request)
        {
            var response = await ticketService.UpdateTicketStatusAsync(id, request);
            return Ok(response);
        }
    }
}
