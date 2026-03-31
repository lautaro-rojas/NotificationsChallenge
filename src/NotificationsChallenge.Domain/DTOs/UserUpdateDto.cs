using System.ComponentModel.DataAnnotations;

namespace NotificationsChallenge.Domain.DTOs
{
    public class UserUpdateDto
    {
        // Este objeto se usa SOLO para recibir datos de "afuera" cuando queremos ACTUALIZAR un usuario.

        [Required(ErrorMessage = "Id is required")]    
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "The password must be at least 6 characters long")]
        public string Password { get; set; } = string.Empty;
    }
}