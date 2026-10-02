using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace TaskManager.Models.ViewModels
{
    public class OrganizationVM
    {
        [ValidateNever]
        public string? SelectedUserId { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? MembersList { get; set; }
    }
}
