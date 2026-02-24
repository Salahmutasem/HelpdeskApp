using Microsoft.AspNetCore.Mvc;
using HelpdeskApp.Data;
using HelpdeskApp.Models;

namespace HelpdeskApp.Controllers
{
    public class TicketsController : BaseController
    {
        private readonly TicketDb _ticketDb;
        private readonly CategoryDb _categoryDb;
        private const int PageSize = 5;

        public TicketsController(TicketDb ticketDb, CategoryDb categoryDb)
        {
            _ticketDb = ticketDb;
            _categoryDb = categoryDb;
        }

        public IActionResult Index(string? search, string? status, int page = 1)
        {
            var (tickets, totalCount) = _ticketDb.GetTickets(search, status, page, PageSize);
            int totalPages = (int)Math.Ceiling((double)totalCount / PageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(tickets);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _categoryDb.GetActiveCategories();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Ticket ticket)
        {
            ModelState.Remove("CreatedBy");
            ModelState.Remove("Status");
            ModelState.Remove("CategoryName");
            ModelState.Remove("CreatedByName");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _categoryDb.GetActiveCategories();
                return View(ticket);
            }

            ticket.CreatedBy = HttpContext.Session.GetInt32("UserId")!.Value;
            _ticketDb.CreateTicket(ticket);

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var ticket = _ticketDb.GetTicketById(id);
            if (ticket == null || ticket.IsDeleted)
                return NotFound();

            return View(ticket);
        }

        [HttpPost]
        public IActionResult AddComment(int ticketId, string commentText)
        {
            var ticket = _ticketDb.GetTicketById(ticketId);
            if (ticket == null)
                return NotFound();

            if (ticket.Status == "Closed")
            {
                TempData["Error"] = "Cannot add comments to a closed ticket.";
                return RedirectToAction("Details", new { id = ticketId });
            }

            if (string.IsNullOrWhiteSpace(commentText))
            {
                TempData["Error"] = "Comment text is required.";
                return RedirectToAction("Details", new { id = ticketId });
            }

            var comment = new TicketComment
            {
                TicketId = ticketId,
                CommentText = commentText,
                CreatedBy = HttpContext.Session.GetInt32("UserId")!.Value
            };

            _ticketDb.AddComment(comment);
            return RedirectToAction("Details", new { id = ticketId });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _ticketDb.SoftDeleteTicket(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            _ticketDb.UpdateTicketStatus(id, status);
            return RedirectToAction("Details", new { id = id });
        }
    }
}