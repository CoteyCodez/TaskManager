using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Business.IServices;
using TaskManager.Data;
using TaskManager.Models;
using Microsoft.EntityFrameworkCore;
using TaskManager.Utilities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


namespace TaskManager.Business.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUserService _applicationUserService;
        private readonly ITaskItemService _taskItemService;
        public OrganizationService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IApplicationUserService applicationUserService,
            ITaskItemService taskItemService)
        {
            _context = context;
            _userManager = userManager;
            _applicationUserService = applicationUserService;
            _taskItemService = taskItemService;
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
        public async Task<bool> AddUserToOrganizationWithRole(string userId, string orgJoinKey, string userRole)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            // Find organization by join key
            Organization targetOrg = await GetOrganizationByJoinKey(orgJoinKey);

            if (targetOrg == null)
            {
                return false;
            }

            // Assign role to user 
            user.OrganizationId = targetOrg.Id;

            return true;
        }
        public async Task<bool> CreateOrganizationWithRole(string userId, string orgName, string userRole)
        {
            var organization = new Organization { Name = orgName };
            _context.Organizations.Add(organization);
            await _context.SaveChangesAsync();

            // Assign role to user
            await AddUserToOrganizationWithRole(userId, organization.JoinCode, userRole);

            return true;
        }

        public async Task<bool> DeleteOrganization(int orgId)
        {
            var organization = await _context.Organizations.FirstOrDefaultAsync(u => u.Id == orgId); // might need to change this

            if (organization == null)
            {
                return false;
            }

            _context.Remove(organization);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Organization> GetOrganizationByUserId(string userId)
        {
            var user = await _context.Users.Include(u => u.Organization).FirstOrDefaultAsync(u => u.Id == userId);
            var returnOrganization = user.Organization;

            if (user == null || returnOrganization == null)
            {
                return null;
            }

            return returnOrganization;
        }
        public async Task<bool> RemoveAllUsersFromOrganization(string userId)   //Only if admin 
        {
            var user = await _applicationUserService.GetUserByIdAsync(userId);
            var organization = await GetOrganizationByUserId(userId);
            int orgIdHolder = organization.Id; 
            if (user == null)
            {
                return false;
            }

            bool isAdmin = await _userManager.IsInRoleAsync(user, SD.RoleLeader);

            if (!isAdmin)
            {
                return false;
            }

            // Remove all users and related TaskItems from organization 
            List<ApplicationUser> organizationMemberList = await _context.Users.Where(o => o.OrganizationId == user.OrganizationId).ToListAsync();
            List<TaskItem> taskItemList = await _context.TaskItems.Where(o => o.OrganizationId == user.OrganizationId).ToListAsync();

            foreach (var member in organizationMemberList)
            {
                await LeaveOrganization(member.Id);
            }

            // Delete all task items belonging to the organization to avoid FK violations

            foreach (var task in taskItemList)
            {
                await _taskItemService.DeleteTaskAsync(task.Id, orgIdHolder);
            }



            // Delete organization after removing all users and their organization tasks

            _context.Remove(organization);

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> LeaveOrganization(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            bool isAdmin = await _userManager.IsInRoleAsync(user, SD.RoleLeader);

            if (user == null)
            {
                return false;
            }

            user.OrganizationId = null;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
