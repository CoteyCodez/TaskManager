using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Models;

namespace TaskManager.Business.IServices
{
    public interface ITaskItemService
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAssignedToUserAsync(string userId);
        Task<IEnumerable<TaskItem>> GetAllTasksInOrganization(ApplicationUser user);

        Task<TaskItem?> GetTaskByIdAsync(int taskId);
        Task<TaskItem> CreateTaskAsync(TaskItem task, int organizationId);
        Task<TaskItem> UpdateTaskAsync(TaskItem task);
        Task<bool> DeleteTaskAsync(int taskId, int organizationId);
    }
}
