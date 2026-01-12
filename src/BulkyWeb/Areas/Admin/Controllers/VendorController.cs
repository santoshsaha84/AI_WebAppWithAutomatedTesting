using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VendorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public VendorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Vendor> objVendorList = _unitOfWork.Vendor.GetAll().ToList();
            return View(objVendorList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Vendor.Add(vendor);
                _unitOfWork.Save();
                TempData["success"] = "Vendor created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var vendor = _unitOfWork.Vendor.Get(u => u.Id == id);
            if (vendor == null)
                return NotFound();

            return View(vendor);
        }

        [HttpPost]
        public IActionResult Edit(Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Vendor.Update(vendor);
                _unitOfWork.Save();
                TempData["success"] = "Vendor updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var vendor = _unitOfWork.Vendor.Get(u => u.Id == id);
            if (vendor == null)
                return NotFound();

            return View(vendor);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            var obj = _unitOfWork.Vendor.Get(u => u.Id == id);
            if (obj == null)
                return NotFound();

            _unitOfWork.Vendor.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Vendor deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
