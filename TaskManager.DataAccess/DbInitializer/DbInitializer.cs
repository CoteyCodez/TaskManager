using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Utilities;

namespace TaskManager.Data.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;


        public DbInitializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }
        public async Task InitializeAsync()
        {
            try
            {
                if ((await _db.Database.GetPendingMigrationsAsync()).Any())
                {
                    await _db.Database.MigrateAsync();
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
