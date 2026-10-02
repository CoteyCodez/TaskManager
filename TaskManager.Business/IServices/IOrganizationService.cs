using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Models;

namespace TaskManager.Business.IServices
{
    public interface IOrganizationService
    {
        Task<bool> JoinOrganizationWithRoleMember(string userId, string orgJoinKey);
        Task<Organization> CreateOrganizationWithRoleLeader(string userId, string orgName);
        Task<bool> DeleteOrganization(int orgId);
        Task<List<ApplicationUser>> GetAllUsersInOrganization(string userId);
        Task<List<TaskItem>> GetAllTasksInOrganization(string userId);

        Task<Organization> GetOrganizationByJoinKey(string orgJoinKey);
        Task<Organization> GetOrganizationByUserId(string userId);

        Task<bool> RemoveFromOrganization(string userId);
        Task<bool> RemoveAllUsersFromOrganization(string userId);
    }
}
