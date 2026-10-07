using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Models;

namespace TaskManager.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        [Required]
        public Guid JoinKey { get; set; } = Guid.NewGuid();
        [Required]
        public string Title { get; set; } = string.Empty; 
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }

        public ICollection<Comment>? Comments { get; set; }

        [ValidateNever]
        [Display(Name = "Product Image")]
        public string? ImageUrl { get; set; }

        public string? CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public ApplicationUser? CreatedBy { get; set; }

        public string? AssignedToUserId { get; set; }
        [ForeignKey("AssignedToUserId")]
        public ApplicationUser? AssignedToUser { get; set; }

        public int? OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public Organization? Organization { get; set; }

        // Only if private task 
        public string? PrivateTaskTargetId { get; set; }
        [ForeignKey("PrivateTaskTargetId")]
        public ApplicationUser? TargetUser { get; set; }


    }
}
