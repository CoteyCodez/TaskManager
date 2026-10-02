using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Business;
using TaskManager.Business.IServices;
using TaskManager.Models;

namespace TaskManager.Areas.User.Controllers
{
    [Area("User")]
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
        public async Task<ActionResult> CreatePOST(string orgName)
        {
            var user = await _userManager.GetUserAsync(User);

            await _organizationService.CreateOrganizationWithRoleLeader(user.Id, orgName);

            return RedirectToAction("Index", "Home", new { area = "User" });
        }

        public ActionResult Join()
        {
            return View(); 
        }

        [HttpPost]
        [ActionName("Join")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> JoinPOST(string orgJoinKey)
        {
            var user = await _userManager.GetUserAsync(User);


            if (orgJoinKey == null)
            {
                return NotFound();        //May need to fix this
            }

            await _organizationService.JoinOrganizationWithRoleMember(user.Id, orgJoinKey);

            return RedirectToAction("Index", "Home", new { area = "User" });
        }
    }
}
