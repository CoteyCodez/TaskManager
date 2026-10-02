using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Models;
using TaskManager.Utilities;


namespace TaskManager.Data.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        //private readonly ITaskItemService _taskItemService;

        public DbInitializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }
        public async Task InitializeAsync()
        {
            try
            {
                if ((await _context.Database.GetPendingMigrationsAsync()).Any())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception)

            {
                throw;
            }

            Organization organization = _context.Organizations.FirstOrDefault();

            if (!await _roleManager.RoleExistsAsync(SD.RoleLeader))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.RoleLeader));
            }

            if (!await _roleManager.RoleExistsAsync(SD.RoleMember))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.RoleMember));
            }


            ApplicationUser user = await _userManager.FindByEmailAsync("admintester@gmail.com");
            
            if (user == null)
            {
                var result = await _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admintester@gmail.com",
                    Email = "admintester@gmail.com",
                    EmailConfirmed = true,
                    OrganizationId = organization.Id, 

                }, "Admin123*");

                if (result.Succeeded)
                {
                    user = await _userManager.FindByEmailAsync("admintester@gmail.com");
                    await _userManager.AddToRoleAsync(user, SD.RoleLeader);
                }

            }

            else if (user.OrganizationId == null)
            {
                user.OrganizationId = organization.Id;
                await _context.SaveChangesAsync();

                if (!await _userManager.IsInRoleAsync(user, SD.RoleLeader))
                {
                    await _userManager.AddToRoleAsync(user, SD.RoleLeader);
                }
            }



            if (user != null && !_context.TaskItems.Any())
            {
                user.OrganizationId = organization.Id;

                _context.TaskItems.Add(new TaskItem
                {
                    Title = "Sample Task",
                    Description = "This is a sample task",
                    Status = "Assigned",
                    CreatedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(7),
                    OrganizationId = organization.Id,
                    AssignedToUserId = user.Id,
                    CreatedById = user.Id
                });

                await _context.SaveChangesAsync();
            }

            //user 2

            ApplicationUser user2 = await _userManager.FindByEmailAsync("usertester@gmail.com");

            if (user2 == null)
            {
                var result = await _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "usertester@gmail.com",
                    Email = "usertester@gmail.com",
                    EmailConfirmed = true,
                    OrganizationId = organization.Id,

                }, "User123*");

                if (result.Succeeded)
                {
                    user2 = await _userManager.FindByEmailAsync("usertester@gmail.com");
                    await _userManager.AddToRoleAsync(user, SD.RoleMember);
                }

            }

            else if (user2.OrganizationId == null)
            {
                user2.OrganizationId = organization.Id;
                await _context.SaveChangesAsync();
            }

            if (user2 != null && !_context.TaskItems.Any())
            {
                user2.OrganizationId = organization.Id;

                _context.TaskItems.Add(new TaskItem
                {
                    Title = "User Task",
                    Description = "This is a user task",
                    Status = "Assigned",
                    CreatedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(7),
                    OrganizationId = organization.Id,
                    AssignedToUserId = user.Id,
                    CreatedById = user.Id
                });

                await _context.SaveChangesAsync();
            }
        }
    }
}
