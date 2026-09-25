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
    public class TaskItemService : ITaskItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public TaskItemService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<TaskItem> CreateTaskAsync(TaskItem task, int organizationId)
        {
            task.OrganizationId = organizationId; // Don't need to do this in the form, but you may want to move it tehre in a hidden field anyway
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

        public async Task<IEnumerable<TaskItem>> GetAllTasksAssignedToUserAsync(string userId)
        {
            return await _context.TaskItems
                .Where(t => t.AssignedToUserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksInOrganization(ApplicationUser user)
        {
            return await _context.TaskItems
                .Where(t => t.OrganizationId == user.OrganizationId)
                .ToListAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int taskId)
        {
            var task = await _context.TaskItems.FindAsync(taskId);

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
