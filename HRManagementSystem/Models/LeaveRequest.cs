using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagementSystem.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        [Required]
        [Display(Name = "請假類型")]
        public string LeaveType { get; set; } = string.Empty;

        [Required]
        [Display(Name = "開始日期")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "結束日期")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "請假原因")]
        public string? Reason { get; set; }

        [Display(Name = "狀態")]
        public string Status { get; set; } = "待審核"; // 預設是「待審核」
    }
}

