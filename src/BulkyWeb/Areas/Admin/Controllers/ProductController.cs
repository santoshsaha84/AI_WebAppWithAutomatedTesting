using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.IO;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        IUnitOfWork unitOfWork;
        IWebHostEnvironment webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            this.unitOfWork = unitOfWork;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> list = unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return View(list);
        }

        public IActionResult Upsert(int? id)
        {
            
            IEnumerable<SelectListItem> categoryList = unitOfWork.Category.GetAll().Select(u => new SelectListItem() { Text = u.Name, Value = u.Id.ToString() });
            // ViewBag.Category = categoryList;
            //ViewData["Category"] = categoryList;
            ProductVM productVM = new ProductVM()
            {
                CategoryList = categoryList
            };
            if (id == null || id == 0)
            {
                productVM.Product = new Product();

            }
            else
            {

                productVM.Product = unitOfWork.Product.Get(u => u.Id == id);
            }
            return View(productVM);
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM obj,IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string rootpath = webHostEnvironment.WebRootPath;

                if (!string.IsNullOrEmpty(obj.Product.ImageUrl))
                {
                    
                    var path = Path.Combine(rootpath , obj.Product.ImageUrl.TrimStart('\\'));
                    if(System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }                  

                }
                if (file != null)
                {

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var productPath = Path.Combine(rootpath, @"images\product");
                    using (var filestream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    obj.Product.ImageUrl = @"\images\product\" + fileName;
                }
                if (obj.Product.ImageUrl == null ) 
                    obj.Product.ImageUrl = string.Empty;

                if (obj.Product.Id == 0)
                {

                    unitOfWork.Product.Add(obj.Product);
                }else
                {
                    unitOfWork.Product.Update(obj.Product);
                }
                unitOfWork.Save();
                TempData["Success"] = "Product Created Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                obj.CategoryList = unitOfWork.Category.GetAll().Select(u => new SelectListItem() { Text = u.Name, Value = u.Id.ToString() });
                return View(obj);
            }
           

        }

        



      

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            var obj = unitOfWork.Product.Get(u => u.Id == id);
            if (obj != null)
            {
                unitOfWork.Product.Remove(obj);
                unitOfWork.Save();
                TempData["success"] = "Record deleted successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }

        }



        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var Product = unitOfWork.Product.Get(u => u.Id == id);
            if (Product == null)
            {
                return NotFound();
            }

            return View(Product);

        }

        public IActionResult GetAllData()
        {
            List<Product> list = unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = list });
        }
    }
}
