using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace TaskManager.Models.ViewModels
{
    public class UpdateVM
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> TaskStatusList { get; set; }
    }
}
