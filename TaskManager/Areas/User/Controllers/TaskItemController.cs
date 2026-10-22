using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManager.Business.IServices;
using TaskManager.Data;
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
        private readonly IApplicationUserService _applicationUserService;
        private readonly IOrganizationService _organizationService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private ApplicationDbContext _context; 

        private readonly ILocalFileService _localFileService;
        public TaskItemController(UserManager<ApplicationUser> userManager,
            ITaskItemService taskItemService,
            IApplicationUserService applicationUserService,
            IOrganizationService organizationService,
            IWebHostEnvironment webHostEnvironment,
            ILocalFileService localFileService,
            ApplicationDbContext context
            )
        {
            _userManager = userManager;
            _taskItemService = taskItemService;
            _applicationUserService = applicationUserService;
            _localFileService = localFileService;
            _organizationService = organizationService;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            //var user = await _userManager.GetUserAsync(User);

            //if (user == null)
            //{
            //    return NotFound();
            //}

            //var allTasksAssignedToUser = (await _taskItemService.GetAllPrivateUserTasks(user.Id)).ToList();
            //return View(allTasksAssignedToUser);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.Name = "Your tasks";
            return View();
        }

        public async Task<IActionResult> IndexAll()
        {
            ViewBag.Name = "Team Tasks";
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            
            return View();
        }
        public async Task<IActionResult> CreateTask()
        {
            ViewBag.Name = "Create Task";
            var user = await _userManager.GetUserAsync(User);
            var org = await _organizationService.GetOrganizationByUserId(user.Id);

            if (user == null || org == null)
            {
                throw new Exception("Either user or organization does not exist");
            }

            TaskItemVM taskItemVM = new TaskItemVM(); 


            var usersInOrg = await _organizationService.GetAllUsersInOrganization(user.Id);
            taskItemVM.OrganizationMemberList = usersInOrg.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = u.UserName,

            });
            taskItemVM.OrganizationId = org.Id;
            return View(taskItemVM);

            //taskItemVM.OrganizationId = null;
            //taskItemVM.OrganizationMemberList = null; 
            //taskItemVM.PrivateTaskTargetId = user.PrivateTaskTargetId; 
        }

        [HttpPost]
        [ActionName("CreateTask")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateTaskPOST(TaskItemVM newTask, IFormFile? file)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            // var membersInOrganization = await _applicationUserService.GetAllUsersInOrganizationAsync(user.OrganizationId ?? 0);
            
            var model = new TaskItem
            {
                Id = newTask.Id, 
                Title = newTask.Title,
                Description = newTask.Description,
                //PrivateTaskTargetId = newTask.PrivateTaskTargetId, 
                OrganizationId = newTask.OrganizationId,
                Status = newTask.Status,
                AssignedToUserId = newTask.OrganizationMemberId,
                CreatedAt = newTask.CreatedAt,
                DueDate = newTask.DueDate
                
            };

            var fileName = await _localFileService.SaveAttachmentToLocal(file);
            model.AttachmentUrl = fileName;

            // snippet pulled from here

            await _taskItemService.CreateTaskAsync(model, user.Id);
            return model.OrganizationId == null ? RedirectToAction("Index") : RedirectToAction("IndexAll");
        }

        public async Task<IActionResult> Update(int id)
        {
            ViewBag.Name = "Update Task";
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

            if (currentTask.AttachmentUrl != null)
                model.AttachmentUrl = currentTask.AttachmentUrl;

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

            if (task.OrganizationId != user.OrganizationId)
            {
                return Forbid();
            }

            if (task == null || taskId < 0)
            {
                return NotFound();
            }

            await _taskItemService.DeleteTaskAsync(task.Id, task.OrganizationId ?? 0);
            return task.OrganizationId == null ? RedirectToAction("Index") : RedirectToAction("IndexAll");
        }

        public IActionResult Download(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return NotFound();
            }
            var fileBytes = _localFileService.DownloadAttachmentInBytes(filename).Result;
            if (fileBytes == null)
            {
                return NotFound();
            }
            return File(fileBytes, "application/octet-stream", filename);
        }

        #region API Calls
        public async Task<IActionResult> GetAllTasksAssignedToUserInOrganization()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var allTasksAssignedToUser = (await _taskItemService.GetAllTasksAssignedToUserAsync(userId)).ToList();

            return Json(new { data = allTasksAssignedToUser });
        }
        public async Task<IActionResult> GetAllOrgTasks()
        {
            string? userId = null;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!User.IsInRole(SD.RoleLeader) && !User.IsInRole(SD.RoleMember))
            {            
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
            }

            var tasks = await _organizationService.GetAllTasksInOrganization(userId);
            return Json(new { data = tasks });
        }
        #endregion
    }
}
