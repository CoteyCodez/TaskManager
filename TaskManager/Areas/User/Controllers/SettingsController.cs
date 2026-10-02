using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Business.IServices;
using TaskManager.Business.Services;
using TaskManager.Models;
using TaskManager.Utilities;

namespace TaskManager.Areas.User.Controllers
{
    [Area("User")]
    public class SettingsController : Controller
    {
        private readonly ITaskItemService _taskItemService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IOrganizationService _organizationService;
        public SettingsController(
            ITaskItemService taskItemService, 
            UserManager<ApplicationUser> userManager, 
            IApplicationUserService applicationUserService, 
            IOrganizationService organizationService)
        {
            _taskItemService = taskItemService;
            _userManager = userManager;
            _applicationUserService = applicationUserService;
            _organizationService = organizationService;
        }

        public async Task<ActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            if(string.IsNullOrEmpty(userId))
            {
                return NotFound(); 
            }

            //Need to include organization in the query with lazy loading to access it in the view
            var user = await _userManager.Users
                .Include(u => u.Organization)
                .FirstOrDefaultAsync(u => u.Id == userId);


            if (user.OrganizationId.HasValue)
            {
                return RedirectToAction("Leave");
            }

            // This should return a view where it just shows that they already have one, maybe i can move the if logic into the view

            return RedirectToAction("JoinOrCreate");

        }


        public IActionResult Leave()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Leave")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LeavePOST()
        {
            var user = await _userManager.GetUserAsync(User);
            var idHolder = user.Id;
            var organization = await _organizationService.GetOrganizationByUserId(idHolder);

            if (user == null)
            {
                return BadRequest("You are not part of an organization.");
            }

            if (User.IsInRole(SD.RoleLeader))
            {
                await _organizationService.RemoveAllUsersFromOrganization(idHolder);
                await _organizationService.DeleteOrganization(organization.Id);
            }

            await _organizationService.LeaveOrganization(user.Id, null);

            return RedirectToAction("Index", "Home", new { area = "User" });
        }

        //JoinOrCreate
        public IActionResult JoinOrCreate()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Join")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> JoinPOST(string orgJoinKey)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("You are not part of an organization.");
            }

            var idHolder = user.Id;            

            if (User.IsInRole(SD.RoleLeader) || User.IsInRole(SD.RoleMember))
            {
                return BadRequest("You are already part of an organization.");
            }

            var joinResult = await _organizationService.JoinOrganizationWithRoleMember(user.Id, orgJoinKey);
            
            if (!joinResult) //gonna need to fix this
            {
                return BadRequest("You are already part of an organization.");
            }

            return RedirectToAction("Index", "Home", new { area = "User" });
        }
    }
}
