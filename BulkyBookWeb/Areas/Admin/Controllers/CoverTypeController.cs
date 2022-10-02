using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<CoverType> coverTypes = unitOfWork.CoverType.GetAll();
            return View(coverTypes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(CoverType cover) {

            if (ModelState.IsValid)
            {
                if (cover.Id == 0)
                {
                    unitOfWork.CoverType.Add(cover);
                    TempData["message"] = "Save successfully";
                }
                else {
                    unitOfWork.CoverType.Update(cover);
                    TempData["message"] = "Update successfully";
                }
                unitOfWork.Save();

                return RedirectToAction("Index");
            }

            return View(cover);
        }

        public IActionResult Edit(int Id) {
            var cover = unitOfWork.CoverType.FirstOrDefault(x => x.Id == Id);
            
            if (Id == 0 || cover == null)
            {
                return NoContent();
            }

            return View("Create", cover);
        }

        public IActionResult Delete(int Id) {
            if (Id == 0) return NoContent();

            var cover = unitOfWork.CoverType.FirstOrDefault(x => x.Id == Id);
            
            if (cover == null)
                return NoContent();

            unitOfWork.CoverType.Remove(cover);
            unitOfWork.Save();

            return RedirectToAction("Index");
        }
    }
}
