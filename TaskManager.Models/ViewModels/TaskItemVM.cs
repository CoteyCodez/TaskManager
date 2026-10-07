using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TaskManager.Models.ViewModels
{
    public class TaskItemVM
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty; 
        public string? Description { get; set; }
        public string? Status { get; set; }
        public int? OrganizationId { get; set; }
        public string? OrganizationMemberId { get; set; }
        public string? OrganizationMemberUsername { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public string? CreatedById { get; set; }
        public string? NewComment { get; set; }
        public IEnumerable<Comment>? Comments { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> TaskStatusList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> OrganizationMemberList { get; set; }
    }
}
