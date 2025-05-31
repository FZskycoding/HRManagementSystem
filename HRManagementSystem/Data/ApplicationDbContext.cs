using HRManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HRManagementSystem.Data
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
    }
}
