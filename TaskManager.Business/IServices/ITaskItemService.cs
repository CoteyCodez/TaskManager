using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Models;

namespace TaskManager.Business.IServices
{
    public interface ITaskItemService
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAssignedToUserAsync(string userId);
        Task<TaskItem?> GetTaskByIdAsync(int taskId);
        Task<TaskItem> CreateTaskAsync(TaskItem task);
        Task<TaskItem> UpdateTaskAsync(TaskItem task);
        Task<bool> DeleteTaskAsync(int taskId);
    }
}
