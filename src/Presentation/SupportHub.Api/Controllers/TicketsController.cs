using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportHub.Application.Authorization;
using SupportHub.Application.Features.Tickets.Commands.AssignTicket;
using SupportHub.Application.Features.Tickets.Commands.CreateTicket;
using SupportHub.Application.Features.Tickets.Commands.CreateTicketComment;
using SupportHub.Application.Features.Tickets.Commands.UpdateTicketStatus;
using SupportHub.Application.Features.Tickets.Queries.GetAllTickets;
using SupportHub.Application.Features.Tickets.Queries.GetTicketComments;
using SupportHub.Application.Features.Tickets.Queries.GetTicketDetail;

namespace SupportHub.Api.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketsController(IMediator mediator) : ControllerBase
    {
        [Authorize(policy: Permissions.Tickets.View)]
        [HttpGet]
        public async Task<IActionResult> GetTickets([FromQuery] GetAllTicketsQuery request)
        {
            var response = await mediator.Send(request);
            return Ok(response);
        }

        [Authorize(policy: Permissions.Tickets.View)]
        [HttpGet("{ticketId:guid}")]
        public async Task<IActionResult> GetTicketDetail([FromRoute] Guid ticketId)
        {
            var response = await mediator.Send(new GetTicketDetailQuery(ticketId));
            return Ok(response);
        }

        [Authorize(policy: Permissions.Tickets.Create)]
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketCommand request)
        {
            var response = await mediator.Send(request);
            return Ok(response);
        }

        [Authorize(policy: Permissions.Tickets.Update)]
        [HttpPatch("{ticketId:guid}/status")]
        public async Task<IActionResult> UpdateTicketStatus([FromBody] UpdateTicketStatusCommand request, [FromRoute] Guid ticketId)
        {
            var command = request with { Id = ticketId };
            var response = await mediator.Send(command);
            return Ok(response);
        }
        
        [Authorize(policy: Permissions.Tickets.Assign)]
        [HttpPatch("{ticketId:guid}/assign")]
        public async Task<IActionResult> AssignTicket([FromBody] AssignTicketCommand request, [FromRoute] Guid ticketId)
        {
            var command = request with { TicketId = ticketId };
            var response = await mediator.Send(command);
            return Ok(response);
        }

        [Authorize(policy: Permissions.Tickets.Comment)]
        [HttpPost("{ticketId:guid}/comments")]
        public async Task<IActionResult> CreateComments([FromBody] CreateTicketCommentCommand request, [FromRoute] Guid ticketId)
        {
            var command = request with { TicketId = ticketId };
            var response = await mediator.Send(command);
            return Ok(response);
        }

        [Authorize(policy: Permissions.Tickets.View)]
        [HttpGet("{ticketId:guid}/comments")]
        public async Task<IActionResult> GetComments([FromRoute] Guid ticketId)
        {
            var response = await mediator.Send(new GetTicketCommentsQuery(ticketId));
            return Ok(response);
        }
    }
}
