using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupportHub.Application.Features.Tickets.Commands.CreateTicket;
using SupportHub.Application.Features.Tickets.Commands.UpdateTicketStatus;
using SupportHub.Application.Features.Tickets.Queries.Tickets.GetAllTickets;
using SupportHub.Application.Features.Tickets.Queries.Tickets.GetOpenTickets;

namespace SupportHub.Api.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketsController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            var response = await mediator.Send(new GetAllTicketsQuery());
            return Ok(response);
        }
        
        [HttpGet("open")]
        public async Task<IActionResult> GetOpenTickets()
        {
            var response = await mediator.Send(new GetOpenTicketsQuery());
            return Ok(response);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketCommand request)
        {
            var response = await mediator.Send(request);
            return Ok(response);
        }
        
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateTicketStatus(Guid id, [FromBody] UpdateTicketStatusCommand request)
        {
            var command = request with { Id = id };
            var response = await mediator.Send(command);
            return Ok(response);
        }
    }
}
