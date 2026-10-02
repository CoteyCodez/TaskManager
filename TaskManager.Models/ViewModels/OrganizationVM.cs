using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace TaskManager.Models.ViewModels
{
    public class OrganizationVM
    {
        public string? Name { get; set; }
        [ValidateNever]
        public string SelectedUserId { get; set; } = string.Empty; 

        [ValidateNever]
        public IEnumerable<SelectListItem>? MembersList { get; set; }
    }
}
