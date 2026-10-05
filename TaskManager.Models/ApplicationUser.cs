using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TaskManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string PrivateTaskTargetId { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();

        public int? OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public Organization? Organization { get; set; }
    }
}
