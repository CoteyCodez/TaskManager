using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Abstractions;
using Microsoft.VisualBasic;
using System.Security.Claims;
using System.Xml.Linq;
using TaskManager.Business.IServices;
using TaskManager.Business.Services;
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
        private readonly IOrganizationService _organizationService;

        public TaskItemController(UserManager<ApplicationUser> userManager, 
            ITaskItemService taskItemService,
            IApplicationUserService applicationUserService,
            IOrganizationService organizationService)
        {
            _userManager = userManager;
            _taskItemService = taskItemService;
            _applicationUserService = applicationUserService;
            _organizationService = organizationService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var allTasksAssignedToUser = (await _taskItemService.GetAllPrivateUserTasks(user.Id)).ToList();
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
            var user = await _userManager.GetUserAsync(User);
            var org = await _organizationService.GetOrganizationByUserId(user.Id);

            if (user == null || org == null)
            {
                throw new Exception("Either user or organization does not exist");
            }

            var usersInOrg = await _organizationService.GetAllUsersInOrganization(user.Id);

            TaskItemVM taskItemVM = new TaskItemVM
            {
                OrganizationMemberList = usersInOrg.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.UserName
                }),

                OrganizationId = org.Id

            };

            return View(taskItemVM);
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreatePOST(TaskItemVM newTask)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var membersInOrganization = await _applicationUserService.GetAllUsersInOrganizationAsync(user.OrganizationId ?? 0);
            var model = new TaskItem
            {
                Id = newTask.Id, 
                Title = newTask.Title,
                Description = newTask.Description,
                OrganizationId = newTask.OrganizationId,
                Status = newTask.Status,
                AssignedToUserId = newTask.OrganizationMemberId,
                CreatedAt = newTask.CreatedAt,
                DueDate = newTask.DueDate,
            };

            await _taskItemService.CreateTaskAsync(model, user.OrganizationId ?? 0);

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
            model.OrganizationId = currentTask.OrganizationId;
            model.Comments = currentTask.Comments;
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
            if (taskItemVM.NewComment != null)
            {
                existingTask.Comments.Add(new Comment {
                    Content = taskItemVM.NewComment,

                    // Foreign keys
                    AuthorId = user.Id,
                    TaskItemId = existingTask.Id,
                    OrganizationId = existingTask.OrganizationId ?? throw new Exception("This task is not attached to an organization."),

                });
            }
            existingTask.DueDate = taskItemVM.DueDate;

            await _taskItemService.UpdateTaskAsync(existingTask);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePOST(int taskId)
        {
            var user = await _userManager.GetUserAsync(User);

            var task = await _taskItemService.GetTaskByIdAsync(taskId);

            if (task.AssignedToUserId != user.Id || task.OrganizationId != user.OrganizationId || task.PrivateTaskTargetId != user.PrivateTaskTargetId)
            {
                return Forbid();
            }

            if (task == null || taskId < 0)
            {
                return NotFound();
            }

            await _taskItemService.DeleteTaskAsync(task.Id, task.OrganizationId ?? 0);
            return RedirectToAction("Index");
        }
    }
}
