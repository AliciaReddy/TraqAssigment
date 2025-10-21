using System.ComponentModel.DataAnnotations;

namespace TraqBankingApp.ViewModels
{
    public class LoginInput
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
