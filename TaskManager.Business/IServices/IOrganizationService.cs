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
        Task<ApplicationUser?> AddUserToOrganizationWithRole(string userId, string userRole);


        Task<Organization> GetOrganizationByJoinKey(string orgJoinKey);
        Task<ApplicationUser> AddUserToOrganizationWithRole(string userId, string orgJoinKey, string userRole);
        Task<Organization> CreateOrganizationWithRole(string userId, string orgName, string userRole);
        Task<ApplicationUser?> RemoveUserFromCurrentOrganization(string userId);

        // Task<ApplicationUser?> CreateOrganizationWithAdmin(string userId, string orgName);


    }
}
