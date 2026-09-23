using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Business.IServices
{
    public interface ITaskItemService
    {
        Task<TaskItem> CreateTaskAsync(TaskItem task, int organizationId);

        Task<bool> DeleteTaskAsync(int taskId, int organizationId);

        Task<IEnumerable<TaskItem>> GetAllTasksAssignedToUserAsync(string userId);

        Task<IEnumerable<TaskItem>> GetAllTasksInOrganization(ApplicationUser user);

        Task<TaskItem?> GetTaskByIdAsync(int taskId);

        Task<TaskItem> UpdateTaskAsync(TaskItem task);
    }
}
