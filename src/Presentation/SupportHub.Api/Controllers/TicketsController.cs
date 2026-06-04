using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportHub.Application.Constants;
using SupportHub.Application.Features.Tickets.Commands.AssignTicket;
using SupportHub.Application.Features.Tickets.Commands.CreateTicket;
using SupportHub.Application.Features.Tickets.Commands.CreateTicketComment;
using SupportHub.Application.Features.Tickets.Commands.UpdateTicketStatus;
using SupportHub.Application.Features.Tickets.Queries.Tickets.GetAllTickets;
using SupportHub.Application.Features.Tickets.Queries.Tickets.GetMyAssigned;
using SupportHub.Application.Features.Tickets.Queries.Tickets.GetOpenTickets;
using SupportHub.Application.Features.Tickets.Queries.Tickets.GetTicketComments;
using SupportHub.Application.Features.Tickets.Queries.Tickets.GetTicketDetail;

namespace SupportHub.Api.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketsController(IMediator mediator) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetTickets([FromQuery] GetAllTicketsQuery request)
        {
            var response = await mediator.Send(request);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("open")]
        public async Task<IActionResult> GetOpenTickets([FromQuery] GetOpenTicketsQuery request)
        {
            var response = await mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("{Id:guid}")]
        public async Task<IActionResult> GetTicketDetail([FromRoute] GetTicketDetailQuery request)
        {
            var response = await mediator.Send(request);
            return Ok(response);
        }

        [Authorize(Roles = Roles.Customer + "," + Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketCommand request)
        {
            var response = await mediator.Send(request);
            return Ok(response);
        }

        [Authorize]
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateTicketStatus([FromBody] UpdateTicketStatusCommand request, Guid id)
        {
            var command = request with { Id = id };
            var response = await mediator.Send(command);
            return Ok(response);
        }
        
        [Authorize(Roles = Roles.SupportAgent + "," + Roles.Admin)]
        [HttpPatch("{ticketId}/assign")]
        public async Task<IActionResult> AssignTicket([FromRoute] Guid ticketId, [FromBody] AssignTicketCommand request)
        {
            var command = request with { TicketId = ticketId };
            var response = await mediator.Send(command);
            return Ok(response);
        }

        [Authorize(Roles = Roles.SupportAgent + "," + Roles.Admin)]
        [HttpGet("my-assigned")]
        public async Task<IActionResult> GetMyAssignedTickets()
        {
            var response = await mediator.Send(new GetMyAssignedTicketsQuery());
            return Ok(response);
        }
        
        [Authorize]
        [HttpPost("{ticketId:guid}/comments")]
        public async Task<IActionResult> CreateComments([FromBody] CreateTicketCommentCommand request, [FromRoute] Guid ticketId)
        {
            var command = request with { TicketId = ticketId };
            var response = await mediator.Send(command);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("{ticketId:guid}/comments")]
        public async Task<IActionResult> GetComments([FromRoute] Guid ticketId)
        {
            var response = await mediator.Send(new GetTicketCommentsQuery(ticketId));
            return Ok(response);
        }
    }
}
