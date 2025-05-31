using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Department { get; set; }

        public string? PhotoPath { get; set; }
    }
}
