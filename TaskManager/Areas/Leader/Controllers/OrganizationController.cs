using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query.Internal;
using TaskManager.Business;
using TaskManager.Business.IServices;
using TaskManager.Models;
using TaskManager.Models.ViewModels;
using TaskManager.Utilities;

namespace TaskManager.Areas.Leader.Controllers
{
    [Area("Leader")]
    [Authorize(Roles = SD.RoleLeader)]
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
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var org = await _organizationService.GetOrganizationByUserId(user.Id);
            if (user == null || org == null)
            {
                throw new Exception("Either user or organization does not exist");
            }

            var usersInOrg = await _organizationService.GetAllUsersInOrganization(user.Id);

            OrganizationVM organizationVM = new OrganizationVM
            {
                MembersList = usersInOrg.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.UserName
                }).Where(u => u.Value != user.Id),

                Name = org.Name

            }; 

            return View(organizationVM);
        }

        [HttpPost]
        [ActionName("Index")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IndexPOST(OrganizationVM organizationVM)
        {
            var user = await _userManager.GetUserAsync(User);

            var targetId = organizationVM.SelectedUserId;

            await _organizationService.LeaveOrganization(user.Id, targetId);
            
           return RedirectToAction("Index");
        }
    }
}
