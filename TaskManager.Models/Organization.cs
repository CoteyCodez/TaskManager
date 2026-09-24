using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Models
{
    public class Organization
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        // Unique code shared with prospective members so they can join this organization at signup.
        public string JoinCode { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();
    }
}
