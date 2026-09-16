using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TaskManager.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }

        public string? CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public IdentityUser? CreatedBy { get; set; }

        public string? AssignedToUserId { get; set; }
        [ForeignKey("AssignedToUserId")]
        public IdentityUser? AssignedToUser { get; set; }

    }
}
