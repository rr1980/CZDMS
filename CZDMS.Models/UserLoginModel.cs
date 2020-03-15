using System.ComponentModel.DataAnnotations;

namespace CZDMS.Models
{

    public class UserLoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
