using Microsoft.AspNetCore.Mvc;
using HelpdeskApp.Data;
using HelpdeskApp.Models;

namespace HelpdeskApp.Controllers
{
    public class CategoriesController : BaseController
    {
        private readonly CategoryDb _categoryDb;

        public CategoriesController(CategoryDb categoryDb)
        {
            _categoryDb = categoryDb;
        }

        public IActionResult Index()
        {
            var categories = _categoryDb.GetAllCategories();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            bool created = _categoryDb.CreateCategory(category);
            if (!created)
            {
                ModelState.AddModelError("Name", "A category with this name already exists.");
                return View(category);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ToggleActive(int id)
        {
            _categoryDb.ToggleCategoryActive(id);
            return RedirectToAction("Index");
        }
    }
}