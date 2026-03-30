using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace NotificationsChallenge.WebApi.Controllers
{   
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        // Inyectamos IConfiguration para poder leer nuestra clave secreta del appsettings.json
        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        // El DTO (Data Transfer Object) para recibir los datos del cliente
        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // 1. VALIDACIÓN (Aquí iría tu lógica real de base de datos)
            // Para el ejemplo, simulamos que validamos las credenciales
            if (request.Email == "usuario@test.com" && request.Password == "123456")
            {
                // 2. Si el usuario es válido, FABRICAMOS el token
                var tokenString = GenerateJwtToken(request.Email, "1"); // "1" sería el ID del usuario en tu DB

                // 3. Devolvemos el token al frontend
                return Ok(new { Token = tokenString });
            }

            // Si las credenciales son incorrectas, devolvemos un 401 Unauthorized
            return Unauthorized(new { Mensaje = "Credenciales incorrectas" });
        }

        // --- El motor que fabrica el JWT ---
        private string GenerateJwtToken(string email, string userId)
        {
            // A. Traemos los datos de configuración
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // B. Definimos los "Claims" (Las afirmaciones o datos que viajan DENTRO del token)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Un ID único para este token
                new Claim("Rol", "Administrador") // Puedes agregar claims personalizados
            };

            // C. Configuramos las propiedades del token (vencimiento, emisor, etc.)
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), // El token expira en 2 horas
                signingCredentials: credentials);

            // D. Lo empaquetamos y lo convertimos en un string (el famoso "eyJhbG...")
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}