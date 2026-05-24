using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Application.Features.Auths.Commands.LoginUser;
using SupportHub.Application.Features.Auths.Commands.RegisterUser;

namespace SupportHub.Api.Controllers
{
    [Route("api/auths")]
    [ApiController]
    public class AuthsController(IMediator mediator, ICurrentService currentService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand request)
        {
            var response = await mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand request)
        {
            var response = await mediator.Send(request);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new
            {
                IsAuthenticated = currentService.IsAuthenticated,
                Email = currentService.Email,
                UserId = currentService.UserId
            });
        }
    }
}
