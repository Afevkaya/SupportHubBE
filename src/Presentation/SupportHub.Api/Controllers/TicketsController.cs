using Microsoft.AspNetCore.Mvc;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Application.DTOs.Requests;

namespace SupportHub.Api.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketsController(ITicketCommandService ticketCommandService, ITicketQueryService ticketQueryService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            var response = await ticketQueryService.GetTicketsAsync();
            return Ok(response);
        }
        
        [HttpGet("open")]
        public async Task<IActionResult> GetOpenTickets()
        {
            var response = await ticketQueryService.GetOpenTicketsAsync();
            return Ok(response);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] RequestCreateTicket request)
        {
            var response = await ticketCommandService.CreateTicketAsync(request);
            return Ok(response);
        }
        
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateTicketStatus(Guid id, [FromBody] RequestUpdateTicketStatus request)
        {
            var response = await ticketCommandService.UpdateTicketStatusAsync(id, request);
            return Ok(response);
        }
    }
}
