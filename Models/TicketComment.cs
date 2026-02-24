using System.ComponentModel.DataAnnotations;

namespace HelpdeskApp.Models
{
    public class TicketComment
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        [Required(ErrorMessage = "Comment text is required")]
        [Display(Name = "Comment")]
        public string CommentText { get; set; } = "";

        // Who wrote this comment (FK to Users)
        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Not stored in DB, just used for display
        public string? CreatedByName { get; set; }
    }
}