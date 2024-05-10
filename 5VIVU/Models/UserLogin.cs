using System.ComponentModel.DataAnnotations;

namespace _5VIVU.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage ="Mời nhập UserName")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Mời nhập Pasword")]

        public string Password { get; set; }

        public int Role { get; set; }

    }
}
