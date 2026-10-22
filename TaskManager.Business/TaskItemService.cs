using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.NativeInterop;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using TaskManager.Business.IServices;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Business
{
    public class TaskItemService : ITaskItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IApplicationUserService _applicationUserService;

        public TaskItemService(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            IApplicationUserService applicationUserService)
        {
            _context = context;
            _userManager = userManager;
            _applicationUserService = applicationUserService; 
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem task, string userId)
        {
            var user = await _applicationUserService.GetUserByIdAsync(userId);

            if (user.OrganizationId != null)
            {
                task.OrganizationId = user.OrganizationId;
            }

            else
            {
                throw new Exception("User does not belong to an organization.");
            }

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteTaskAsync(int taskId, int organizationId)
        {
            var targetTask = await _context.TaskItems.FindAsync(taskId);
            if (targetTask is null || targetTask.OrganizationId != organizationId)
            {
                return false;
            }

            _context.TaskItems.Remove(targetTask);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserTasksInOrganizationTaskAsync(int taskId, int organizationId)
        {
            var targetTask = await _context.TaskItems.Where(t => t.OrganizationId == organizationId).ToListAsync(); 
            
            if (targetTask is null)
            {
                return false;
            }

            foreach (var task in targetTask)
            {
                _context.TaskItems.Remove(task);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TaskItem>> GetAllPrivateUserTasks(string userId)
        {
            var privateUser = await _applicationUserService.GetUserByIdAsync(userId);
            return await _context.TaskItems
                .Where(t => t.PrivateTaskTargetId == privateUser.PrivateTaskTargetId)
                .ToListAsync();
        }
        
        // FIXME: Should probably move this to the organization controller to keep logic delineated well
        public async Task<IEnumerable<TaskItem>> GetAllTasksAssignedToUserAsync(string userId)
        {
            var user = await _applicationUserService.GetUserByIdAsync(userId);
            return await _context.TaskItems
                .Where(t => t.AssignedToUser == user)
                .Include(t => t.Comments)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksInOrganization(ApplicationUser user)
        {
            return await _context.TaskItems
                .Where(t => t.OrganizationId == user.OrganizationId)
                .Include(t => t.Comments)
                .ToListAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int taskId)
        {
            var task = await _context.TaskItems
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.Id == taskId); 

            if (task is null)
            {
                return null;
            }

            return task;
        }

        public async Task<TaskItem> UpdateTaskAsync(TaskItem task)
        {
            _context.TaskItems.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }


    }
}
