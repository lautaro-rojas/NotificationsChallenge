namespace NotificationsChallenge.Application.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DateActivationFormated { get; set; } = string.Empty;
        // We can add extra properties ony for the view
    }
}