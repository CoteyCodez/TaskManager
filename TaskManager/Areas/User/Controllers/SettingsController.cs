using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Business.IServices;
using TaskManager.Models;

namespace TaskManager.Areas.User.Controllers
{
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
        public ActionResult Index()
        {
            var user = _userManager.GetUserAsync(User).Result;

            if (user.OrganizationId == null)
            {
                return NotFound();  // Change this to a view where they can make one
            }

            // This should return a view where it just shows that they already have one, maybe i can move the if logic into the view

            return View(); 

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IndexPOST(string userId, string orgJoinKey, string userRole)
        {
            var user = _userManager.GetUserAsync(User).Result;


            if (orgJoinKey == null || user.OrganizationId == null)
            {
                return null;        //May need to fix this
            }

            _organizationService.AddUserToOrganizationWithRole(user.Id, orgJoinKey, userRole);

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
