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
        public async Task<bool> JoinOrganizationWithRoleMember(string userId, string orgJoinKey)
        {
            var user = await _applicationUserService.GetUserByIdAsync(userId);

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

            // Join organization and assign role to user 
            await _applicationUserService.JoinOrganizationByJoinCode(userId, targetOrg.JoinCode);
            await _userManager.AddToRoleAsync(user, SD.RoleMember);

            return true;
        }
        public async Task<Organization> CreateOrganizationWithRoleLeader(string userId, string orgName) // Can add sub-role between Creator and Member here later if you want
        {
            var organization = new Organization { Name = orgName };
            _context.Organizations.Add(organization);

            var user = await _applicationUserService.GetUserByIdAsync(userId);
            user.OrganizationId = organization.Id;
            await _context.SaveChangesAsync(); 

            await _userManager.AddToRoleAsync(user, SD.RoleLeader);
           
            return organization;
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
        public async Task<List<ApplicationUser>> GetAllUsersInOrganization(string userId)   //Only if admin 
        {
            var organization = await GetOrganizationByUserId(userId);
            List<ApplicationUser> organizationMemberList = await _context.Users.Where(o => o.OrganizationId == organization.Id).ToListAsync();
            return organizationMemberList;
        }

        public async Task<List<TaskItem>> GetAllTasksInOrganization(string userId)   //Only if admin 
        {
            var organization = await GetOrganizationByUserId(userId);
            List<TaskItem> organizationTaskList = await _context.TaskItems.Where(o => o.OrganizationId == organization.Id).ToListAsync();
            return organizationTaskList;
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

            foreach (var member in await GetAllUsersInOrganization(userId))
            {
                await LeaveOrganization(userId, member.Id);
            }

            // Delete all task items belonging to the organization to avoid FK violations

            foreach (var task in await GetAllTasksInOrganization(userId))
            {
                await _taskItemService.DeleteTaskAsync(task.Id, orgIdHolder);
            }

            // Delete organization after removing all users and their organization tasks

            _context.Remove(organization);

            await _context.SaveChangesAsync();

            return true;
        }
        // This method passes admin tasks back into the queue
        public async Task<bool> LeaveOrganization(string callerId, string? targetId)
        {
            var target = await _userManager.FindByIdAsync(targetId);
            if (target == null)
            {
                return false;
            }

            var caller = await _userManager.FindByIdAsync(callerId);
            if (caller == null)
            {
                return false;
            }

            bool isAdmin = await _userManager.IsInRoleAsync(caller, SD.RoleLeader);

            var organization = await GetOrganizationByUserId(callerId);

            if (isAdmin)                                // Admin removes another user
            {
                if (isAdmin && callerId != targetId) 
                {
                    target.OrganizationId = null;
                }
            }

            if (!isAdmin && callerId == targetId)      // Member removes self
            {
                caller.OrganizationId = null;   
            }

            if (isAdmin && callerId == targetId)      // Admin removes self
            {
                var leaderTasks = await _taskItemService.GetAllTasksAssignedToUserAsync(callerId);
                var nextUser = await _context.Users.FirstOrDefaultAsync(u => u.OrganizationId == organization.Id && u.Id != callerId);
                
                if (nextUser != null)
                {
                    foreach (var task in leaderTasks)
                    {
                        task.AssignedToUserId = nextUser.Id;
                    }
                }

                else
                {
                    await RemoveAllUsersFromOrganization(callerId);
                    await DeleteOrganization(organization.Id);
                }
            }

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
