using System.ComponentModel.DataAnnotations;

namespace HelpdeskApp.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = "";

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = "";

        [Required(ErrorMessage = "Please select a category")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // Who created this ticket (FK to Users)
        public int CreatedBy { get; set; }

        // Status can be: Open, InProgress, Closed
        public string Status { get; set; } = "Open";

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;

        // These are not in the database table, we fill them when reading
        // so we can show the category name and user name in the list
        public string? CategoryName { get; set; }
        public string? CreatedByName { get; set; }

        // Comments for the details page
        public List<TicketComment> Comments { get; set; } = new List<TicketComment>();
    }
}