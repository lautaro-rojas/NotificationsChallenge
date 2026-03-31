using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using NotificationsChallenge.Domain.DTOs;
using NotificationsChallenge.Application.Services;

namespace NotificationsChallenge.WebApi.Controllers
{   
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly JwtService _jwtService;

        public AuthController(AuthService authService, JwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            UserDto? user = await _authService.LoginAsync(loginDto);

            if (user != null) 
            {
                var tokenString = _jwtService.GenerateJwtToken(user.Id.ToString(), user.Email, user.Name);

                return Ok(new { Token = tokenString }); // Code 200: OK
            }

            return Unauthorized(new { Mensaje = "Incorrect credentials" }); // Code 401: Unauthorized
        }
    }
}