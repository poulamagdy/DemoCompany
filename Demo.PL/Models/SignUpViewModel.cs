using System.ComponentModel.DataAnnotations;

namespace Demo.PL.Models
{
	public class SignUpViewModel
	{
        [Required(ErrorMessage = "Email Required")]
		[RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$",
		ErrorMessage = "Please enter a valid email address.")]
		public string Email { get; set; }
		[Required(ErrorMessage = "Password Required")]
		[MaxLength(10)]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
		ErrorMessage = "Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one digit, and one special character from @$!%*?&.")]
		public string Password { get; set; }
		[Required(ErrorMessage = "Password Required")]
		[MaxLength(10)]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]
		[Compare(nameof(Password),ErrorMessage = "Passord Not Match")]
		public string ConfirmPassword { get; set; }
		[Required]
        public bool IsActive { get; set; }
    }
}
