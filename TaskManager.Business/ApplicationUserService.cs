using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using TaskManager.Business.IServices;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Business
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ApplicationUserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IEnumerable<ApplicationUser>> GetAllUsersInOrganizationAsync(int orgId)
        {
            return await _context.Users.Where(u => u.OrganizationId == orgId).ToListAsync();
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
