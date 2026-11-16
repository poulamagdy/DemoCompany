using System.ComponentModel.DataAnnotations;

namespace Demo.PL.Models
{
	public class ResetPasswordViewModel
	{
		[Required(ErrorMessage = "Password Required")]
		[MaxLength(10)]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]

		public string Password { get; set; }
		[Required(ErrorMessage = "Password Required")]
		[MaxLength(10)]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]
		[Compare(nameof(Password), ErrorMessage ="Password Not Match")]
		public string ConfirmPassword { get; set; }
    }
}
