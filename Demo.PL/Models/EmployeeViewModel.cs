using Demo.DAL.Entities;
using System.ComponentModel.DataAnnotations;

namespace Demo.PL.Models
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name Is Required")]
        [MaxLength(50)]
        public string Name { get; set; }
        public string Address { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Currency)]
        public double Salary { get; set; }
        public bool IsActive { get; set; }
        public DateTime HireDate { get; set; } = DateTime.Now;
        public Department Department { get; set; }
        public int DepartmentId { get; set; }
        public string? ImageName { get; set; }
        public IFormFile Image { get; set; }
    }
}
