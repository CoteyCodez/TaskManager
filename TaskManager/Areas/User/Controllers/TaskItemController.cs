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

namespace TaskManager.Areas.User.Controllers
{
    [Area("User")]
    public class TaskItemController : Controller
    {
        private readonly ITaskItemService _taskItemService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TaskItemController(UserManager<ApplicationUser> userManager, ITaskItemService taskItemService)
        {
            _userManager = userManager;
            _taskItemService = taskItemService;
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
            var model = new UpdateVM();

            model.TaskStatusList =
                [
                    new SelectListItem { Text = SD.TaskAssigned, Value = SD.TaskAssigned },
                    new SelectListItem { Text = SD.TaskCompleted, Value = SD.TaskCompleted }    
                ];

            model.Id = id;

            return View(model);
        }

        [HttpPost]
        [ActionName("Update")]
        public async Task<IActionResult> UpdatePOST(UpdateVM updateVM)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var existingTask = await _taskItemService.GetTaskByIdAsync(updateVM.Id);

            existingTask.Title = updateVM.Title;
            existingTask.Description = updateVM.Description;
            existingTask.Status = updateVM.Status;
            existingTask.CreatedAt = updateVM.CreatedAt;
            existingTask.DueDate = updateVM.DueDate;

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
