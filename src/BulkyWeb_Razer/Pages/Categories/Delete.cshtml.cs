using BulkyWeb_Razer.Data;
using BulkyWeb_Razer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWeb_Razer.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        public readonly ApplicationDbContext _db;

        [BindProperty]
        public Category category { get; set; }

        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet(int? id)
        {
            category = _db.Categories.SingleOrDefault(c => c.Id == id);
        }

        public IActionResult OnPost()
        {
            var cat = _db.Categories.SingleOrDefault(x => x.Id ==category.Id);
            _db.Categories.Remove(cat);
            _db.SaveChanges();
            TempData["success"] = "Record deleted successfully";
            return RedirectToPage("Index");
        }
    }
}
