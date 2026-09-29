using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Business;
using TaskManager.Business.IServices;
using TaskManager.Models;

namespace TaskManager.Areas.User.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly IOrganizationService _organizationService;
        private readonly UserManager<ApplicationUser> _userManager;
        public OrganizationController(
            IOrganizationService organizationService,
            UserManager<ApplicationUser> userManager)
        {
            _organizationService = organizationService;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePOST(string orgName)
        {
            var user = _userManager.GetUserAsync(User).Result;

            _organizationService.CreateOrganizationWithRoleLeader(orgName);

            return RedirectToAction("Index", "Home", new { area = "User" });
        }

        public ActionResult Join()
        {
            return View(); 
        }

        [HttpPost]
        [ActionName("Join")]
        [ValidateAntiForgeryToken]
        public ActionResult JoinPOST(string orgJoinKey)
        {
            var user = _userManager.GetUserAsync(User).Result;


            if (orgJoinKey == null)
            {
                return NotFound();        //May need to fix this
            }

            _organizationService.JoinOrganizationWithRoleMember(user.Id, orgJoinKey);

            return RedirectToAction("Index", "Home", new { area = "User" });
        }
    }
}
