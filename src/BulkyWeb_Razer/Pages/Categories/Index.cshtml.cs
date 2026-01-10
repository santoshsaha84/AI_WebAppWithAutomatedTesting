using BulkyWeb_Razer.Data;
using BulkyWeb_Razer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWeb_Razer.Pages.Categories
{
    public class IndexModel : PageModel
    {
        public readonly ApplicationDbContext _db;
        public List<Category> categories { get; set; }
        public IndexModel(ApplicationDbContext db)
        {
            _db= db;
        }
        public void OnGet()
        {
            categories = _db.Categories.ToList();
        }
    }
}
