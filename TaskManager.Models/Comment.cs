using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TaskManager.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow; 
        [Required]
        public string Content { get; set; } = string.Empty;

        // Foreign Keys
        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public Organization? Organization { get; set; }

        public int TaskItemId { get; set; }
        [ForeignKey("TaskItemId")]
        public TaskItem? TaskItem { get; set; }

        public string? AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public ApplicationUser? Author { get; set; }
        
    }
}
