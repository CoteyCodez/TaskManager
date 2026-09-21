using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManager.Models;
using TaskManager.Utilities;

namespace TaskManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<Organization> Organizations { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Organization>().HasData(
                new Organization { Id = 1, Name = "Microsoft" }
                );
            modelBuilder.Entity<TaskItem>().HasData(
                new TaskItem
                {
                    Id = 1,
                    Title = "Sample Task",
                    Description = "This is a sample task",
                    OrganizationId = 1,
                    Status = SD.TaskAssigned,
                    AssignedToUserId = "e89b78c6-a35d-4c84-a40c-09ddd190f366",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    DueDate = new DateTime(2026, 1, 8, 0, 0, 0, DateTimeKind.Utc)
                }
                );

        }
    }
}

