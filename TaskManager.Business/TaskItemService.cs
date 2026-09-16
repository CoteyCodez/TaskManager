using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TaskManager.Business.IServices;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Business
{
    public class TaskItemService : ITaskItemService
    {
        private readonly ApplicationDbContext _context;
        public TaskItemService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            var targetTask = await _context.TaskItems.FindAsync(taskId);
            if (targetTask is null)
            {
                return false;
            }

            _context.TaskItems.Remove(targetTask);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAssignedToUserAsync(string userId)
        {
            return await _context.TaskItems
                .Where(t => t.AssignedToUserId == userId)
                .ToListAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int taskId)
        {
            return await _context.TaskItems.FindAsync(taskId);
        }

        public async Task<TaskItem> UpdateTaskAsync(TaskItem task)
        {
            _context.TaskItems.Update(task);
            await _context.SaveChangesAsync();
            return task;

        }
    }
}
