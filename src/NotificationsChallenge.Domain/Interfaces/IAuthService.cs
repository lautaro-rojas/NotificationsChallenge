using NotificationsChallenge.Domain.DTOs;

namespace NotificationsChallenge.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<int> RegisterAsync(UserCreationDto dto);

        Task<UserDto?> LoginAsync(LoginDto dto);
    }
}