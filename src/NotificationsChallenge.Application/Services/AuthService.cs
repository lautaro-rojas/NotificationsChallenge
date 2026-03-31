using Microsoft.EntityFrameworkCore;
using NotificationsChallenge.Domain.Entities;
using NotificationsChallenge.Domain.Interfaces;
using NotificationsChallenge.Domain.DTOs;
using NotificationsChallenge.Infrastructure.Data;

namespace NotificationsChallenge.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        public AuthService(ApplicationDbContext db) => _db = db;

        public async Task<UserDto?> LoginAsync(LoginDto dto)
        {
            var user = _db.USER.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null) return null;

            // Aquí deberías comparar el password hasheado, pero para el ejemplo lo dejamos simple
            if (user.Password != dto.Password)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                DateActivation = user.DateActivation
            };
        }

        public async Task<int> RegisterAsync(UserCreationDto dto)
        {
            throw new NotImplementedException();
        }
    }
}