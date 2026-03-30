using Microsoft.EntityFrameworkCore;
using NotificationsChallenge.Domain.Entities;
using NotificationsChallenge.Domain.Interfaces;
using NotificationsChallenge.Domain.DTOs;
using NotificationsChallenge.Infrastructure.Data;

namespace NotificationsChallenge.Application.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;
        public UserService(ApplicationDbContext db) => _db = db;

        public async Task<int> AddAsync(UserCreationDto dto)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password, // Hask before save
                ActiveAccount = true,
                DateActivation = DateTime.UtcNow,
                DateModification = DateTime.UtcNow,
                DateDeactivation = DateTime.UtcNow
            };

            await _db.USER.AddAsync(user);
            await _db.SaveChangesAsync();
            return user.Id;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _db.USER.FindAsync(id);
            if (user == null) return false;

            _db.USER.Remove(user);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteLogicAsync(int id, UserCreationDto dto)
        {
            var user = await _db.USER.FindAsync(id);
            if (user == null) return false;

            user.ActiveAccount = false;
            user.DateDeactivation = DateTime.UtcNow;
            user.DateModification = DateTime.UtcNow;

            _db.USER.Update(user);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            return await _db.USER
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    ActiveAccount = u.ActiveAccount,
                    DateActivation = u.DateActivation,
                    DateModification = u.DateModification,
                    DateDeactivation = u.DateDeactivation
                })
                .ToListAsync();
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var u = await _db.USER.FindAsync(id);
            if (u == null) return null;

            return new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                ActiveAccount = u.ActiveAccount,
                DateActivation = u.DateActivation,
                DateModification = u.DateModification,
                DateDeactivation = u.DateDeactivation
            };
        }

        public async Task<bool> UpdateAsync(int id, UserUpdateDto dto)
        {
            var user = await _db.USER.FindAsync(id);
            if (user == null) return false;

            user.Name = dto.Name;
            user.Email = dto.Email;
            user.Password = dto.Password; // Hask before save
            user.DateModification = DateTime.UtcNow;

            _db.USER.Update(user);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}