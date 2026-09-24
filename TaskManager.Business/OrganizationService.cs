using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Business.IServices;
using TaskManager.Data;
using TaskManager.Models;
using Microsoft.EntityFrameworkCore;


namespace TaskManager.Business.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public OrganizationService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Task<ApplicationUser?> AddUserToOrganizationWithRole(string userId, string userRole)
        {
            throw new NotImplementedException();
        }

        //public Task<ApplicationUser?> CreateOrganizationWithAdmin(string userId, string orgName)
        //{
        //    throw new NotImplementedException();
        //}

        // add user to oranization with role and then update it 

        public async Task<Organization> GetOrganizationByJoinKey(string orgJoinKey)
        {
            var organization = await _context.Organizations.FirstOrDefaultAsync(o => o.JoinCode == orgJoinKey);

            if (organization == null)
            {
                return null; 
            }

            return organization;
        }
        public async Task<ApplicationUser> AddUserToOrganizationWithRole(string userId, string orgJoinKey, string userRole)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) 
            {
                return null;
            }

            // Find organization by join key

            Organization targetOrg = await GetOrganizationByJoinKey(orgJoinKey);

            if (targetOrg == null)
            {
                return null;
            }

            // assign role to user 

            user.OrganizationId = targetOrg.Id;

            return user;
        }
        public async Task<Organization> CreateOrganizationWithRole(string userId, string orgName, string userRole)
        {
            var organization = new Organization { Name = orgName };
            _context.Organizations.Add(organization);
            await _context.SaveChangesAsync();

            // assign role to user
            await AddUserToOrganizationWithRole(userId, organization.JoinCode, userRole);

            return organization;
        }
        public async Task<ApplicationUser?> RemoveUserFromCurrentOrganization(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return null; 
            }
            user.OrganizationId = null;

            await _context.SaveChangesAsync();

            return user; 
        }
    }
}
