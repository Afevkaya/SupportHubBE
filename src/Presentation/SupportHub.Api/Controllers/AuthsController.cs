using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupportHub.Application.Features.Auths.Commands.RegisterUser;

namespace SupportHub.Api.Controllers
{
    [Route("api/auths")]
    [ApiController]
    public class AuthsController(IMediator mediator) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand request)
        {
            await mediator.Send(request);
            return Ok();
        }
    }
}
