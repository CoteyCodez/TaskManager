using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using System.Security.Claims;
using TaskManager.Business.IServices;
using TaskManager.Models;
using TaskManager.Models.ViewModels;
using TaskManager.Utilities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TaskManager.Areas.User.Controllers
{
    [Area("User")]
    public class TaskItemController : Controller
    {
        private readonly ITaskItemService _taskItemService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUserService _applicationUserService;

        public TaskItemController(UserManager<ApplicationUser> userManager, 
            ITaskItemService taskItemService,
            IApplicationUserService applicationUserService)
        {
            _userManager = userManager;
            _taskItemService = taskItemService;
            _applicationUserService = applicationUserService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var allTasksAssignedToUser = (await _taskItemService.GetAllTasksAssignedToUserAsync(user.Id)).ToList();
            return View(allTasksAssignedToUser);
        }

        public async Task<IActionResult> IndexAll()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }
            var tasksInOrganization = await _taskItemService.GetAllTasksInOrganization(user);
            return View(tasksInOrganization);
        }

        public async Task<IActionResult> Create()
        {
            return View(); 
        }

        [HttpPost]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePOST(TaskItem newTask)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            newTask.AssignedToUserId = user.Id;
            await _taskItemService.CreateTaskAsync(newTask, user.OrganizationId ?? 0);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var model = new TaskItemVM();

            model.TaskStatusList =
                [
                    new SelectListItem { Text = SD.TaskAssigned, Value = SD.TaskAssigned },
                    new SelectListItem { Text = SD.TaskCompleted, Value = SD.TaskCompleted }    
                ];

            var membersInOrganization = await _applicationUserService.GetAllUsersInOrganizationAsync(user.OrganizationId ?? 0);

            model.OrganizationMemberList = new SelectList(membersInOrganization, "Id", "UserName");

            model.Id = id;

            var currentTask = await _taskItemService.GetTaskByIdAsync(id);
            model.Title = currentTask.Title;
            model.Description = currentTask.Description;
            model.Status = currentTask.Status;
            model.OrganizationMemberId = currentTask.AssignedToUserId;
            model.CreatedAt = currentTask.CreatedAt;
            model.DueDate = currentTask.DueDate;

            model.OrganizationMemberUsername = (await _applicationUserService.GetUserByIdAsync(currentTask.AssignedToUserId))?.UserName;           

            return View(model);
        }

        [HttpPost]
        [ActionName("Update")]
        public async Task<IActionResult> UpdatePOST(TaskItemVM taskItemVM)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var existingTask = await _taskItemService.GetTaskByIdAsync(taskItemVM.Id);

            existingTask.Title = taskItemVM.Title;
            existingTask.Description = taskItemVM.Description;
            existingTask.Status = taskItemVM.Status;
            existingTask.CreatedAt = taskItemVM.CreatedAt;
            existingTask.DueDate = taskItemVM.DueDate;

            await _taskItemService.UpdateTaskAsync(existingTask);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var task = await _taskItemService.GetTaskByIdAsync(id);

            if (task.AssignedToUserId != user.Id || task.OrganizationId != user.OrganizationId)
            {
                return Forbid();
            }

            if (task == null || id < 0)
            {
                return NotFound();
            }

            await _taskItemService.DeleteTaskAsync(task.Id, task.OrganizationId ?? 0);
            return RedirectToAction("Index");
        }
    }
}
