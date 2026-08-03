using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project_3.src.Application.Models;
using Project_3.src.Infrastructure.identity;

namespace Project_3.src.Infrastructure.Data.Context
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<EmployeeLeaveBalance> EmployeeLeaveBalances { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                           .HasIndex(u => u.Email)
                           .IsUnique();

            modelBuilder.Entity<EmployeeLeaveBalance>()
                           .HasIndex(e => new { e.UserId, e.LeaveTypeId })
                           .IsUnique();
                           
            modelBuilder.Entity<LeaveRequest>()
                           .Property(lr => lr.Status)
                           .HasConversion<string>();

        }
    }
    }
