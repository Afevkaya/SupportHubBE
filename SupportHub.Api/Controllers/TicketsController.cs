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
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] RequestCreateTicket request)
        {
            var response = await ticketService.CreateTicketAsync(request);
            return Ok(response);
        }
    }
}
