using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Business;
using TaskManager.Models;
using System.Linq;
using TaskManager.Business.IServices;

namespace TaskManager.Controllers
{
    public class TaskItemController : Controller
    {
        private readonly ITaskItemService _taskItemService;
        private readonly UserManager<IdentityUser> _userManager;

        public TaskItemController(UserManager<IdentityUser> userManager, ITaskItemService taskItemService)
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
            await _taskItemService.CreateTaskAsync(newTask);
            return RedirectToAction("Index");
        }
    }
}
