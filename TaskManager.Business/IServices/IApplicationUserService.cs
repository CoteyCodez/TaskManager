using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Models;

namespace TaskManager.Business.IServices
{
    public interface IApplicationUserService
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsersInOrganizationAsync(int orgId);
        Task<ApplicationUser?> GetUserByIdAsync(string userId); 
    }
}
