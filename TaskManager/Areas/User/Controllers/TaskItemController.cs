using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TaskManager.Business;
using TaskManager.Business.IServices;
using TaskManager.Models;

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

           //FIXME
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

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var task = await _taskItemService.GetTaskByIdAsync(id, user.OrganizationId ?? 0);

            if (task == null || id < 0)
            {
                return NotFound();
            }

            await _taskItemService.DeleteTaskAsync(task.Id, task.OrganizationId ?? 0);
            return RedirectToAction("Index");
        }
    }
}
