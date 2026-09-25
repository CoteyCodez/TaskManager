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
        Task<bool> AddUserToOrganizationWithRole(string userId, string orgJoinKey, string userRole);
        Task<bool> CreateOrganizationWithRole(string userId, string orgName, string userRole);
        Task<bool> DeleteOrganization(int orgId);
        Task<Organization> GetOrganizationByJoinKey(string orgJoinKey);
        Task<Organization> GetOrganizationByUserId(string userId);
        Task<bool> LeaveOrganization(string userId);
        Task<bool> RemoveAllUsersFromOrganization(string userId);
    }
}
