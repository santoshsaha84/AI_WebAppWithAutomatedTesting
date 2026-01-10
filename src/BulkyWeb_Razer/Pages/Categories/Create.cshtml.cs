using BulkyWeb_Razer.Data;
using BulkyWeb_Razer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWeb_Razer.Pages.Categories
{
    public class CreateModel : PageModel
    {
        public readonly ApplicationDbContext _db;

        [BindProperty]
        public Category category { get; set; }
        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            _db.Categories.Add(category);
            _db.SaveChanges();
            TempData["success"] = "Record created successfully";
            return  RedirectToPage("Index");
        }
    }
}
