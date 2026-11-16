using System.ComponentModel.DataAnnotations;

namespace Demo.PL.Models
{
	public class LoginViewModel
	{
        [Required(ErrorMessage = "Email Required")]
		[RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$",
		ErrorMessage = "Please enter a valid email address.")]
		public string Email { get; set; }
		[Required(ErrorMessage = "Password Required")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]
		public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
