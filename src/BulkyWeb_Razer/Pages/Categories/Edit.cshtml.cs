using BulkyWeb_Razer.Data;
using BulkyWeb_Razer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWeb_Razer.Pages.Categories
{
    public class EditModel : PageModel
    {
        public readonly ApplicationDbContext _db;

        [BindProperty]
        public Category category { get; set; }

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int? id)
        {
            category= _db.Categories.SingleOrDefault(c => c.Id==id);
        }

        public IActionResult OnPost()
        {
            _db.Categories.Update(category);
            _db.SaveChanges();
            TempData["success"] = "Record updated successfully";
            return RedirectToPage("Index");
        }
    }
}
