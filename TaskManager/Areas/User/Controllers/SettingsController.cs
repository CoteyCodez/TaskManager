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

        // GET: SettingsController
        public async Task<ActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            //Need to include organization in the query with lazy loading to access it in the view
            var user = await _userManager.Users
                .Include(u => u.Organization)
                .FirstOrDefaultAsync(u => u.Id == userId);


            if (user.OrganizationId == null)
            {
                return NotFound();  // Change this to a view where they can make one
            }

            // This should return a view where it just shows that they already have one, maybe i can move the if logic into the view

            return View(user); 

        }

        [HttpPost]
        [ActionName("Index")]
        [ValidateAntiForgeryToken]
        public ActionResult IndexPOST(string orgJoinKey, string userRole)
        {
            var user = _userManager.GetUserAsync(User).Result;


            if (orgJoinKey == null || user.OrganizationId == null)
            {
                return null;        //May need to fix this
            }

            // _organizationService.RemoveUserFromCurrentOrganization
            _organizationService.AddUserToOrganizationWithRole(user.Id, orgJoinKey, userRole);

            return RedirectToAction("Index", "Home", new { area = "User" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Remove()
        {
            var user = await _userManager.GetUserAsync(User);
            var idHodler = user.Id; 

            if (user == null)
            {
                return BadRequest("You are not part of an organization.");
            }
            if (User.IsInRole(SD.RoleLeader))
            {
                int idHolder = user.OrganizationId.Value;
                await _organizationService.RemoveAllUsersFromOrganization(user.Id);
                await _organizationService.DeleteOrganization(idHolder);
            }
            else
            {
                return Forbid(); 
            }

            await _organizationService.LeaveOrganization(user.Id);

            return RedirectToAction("Index", "Home", new { area = "User" });
        }

        // GET: SettingsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SettingsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SettingsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SettingsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SettingsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SettingsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SettingsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
