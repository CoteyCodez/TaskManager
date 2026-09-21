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
                    OrganizationId = 1, 
                    
                    // Name = "Caleb Otey",
                    // PhoneNumber = "1112223333",
                }, "Admin123*");

                if (result.Succeeded)
                {
                    user = await _userManager.FindByEmailAsync("admintester@gmail.com");
                    await _userManager.AddToRoleAsync(user, SD.RoleLeader);
                }

            }
        }
    }
}
