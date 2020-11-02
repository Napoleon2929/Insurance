using System.ComponentModel.DataAnnotations;

namespace Identity.Models
{
    public class LoginViewModel
    {
        private string _email;

        [Required]
        [EmailAddress]
        public string Email
        {
            get => _email;
            set => _email = value?.Trim();
        }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
