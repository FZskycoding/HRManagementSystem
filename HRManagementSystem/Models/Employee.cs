using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "請輸入姓名")]
        [StringLength(20, ErrorMessage = "姓名長度不可超過 20 字")]
        [Display(Name = "名稱")]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "請選擇部門")]
        [Display(Name = "部門")]
        public string Department { get; set; }

        [Display(Name = "照片")]
        public string? Photo { get; set; }
    }
}
