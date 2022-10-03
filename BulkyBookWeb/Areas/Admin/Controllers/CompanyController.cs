using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAllCompanies()
        {
            var company = _unitOfWork.Company.GetAll();
            return Json(new { data = company });
        }

        public IActionResult Upsert(int? Id)
        {
            Company company = new();
            
            if (Id != null && Id != 0)
            {
                company = _unitOfWork.Company.FirstOrDefault(x => x.Id == Id) ?? new();
            }

            return View(company);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                    TempData["message"] = "Save successfully";
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                    TempData["message"] = "Update successfully";
                }
                _unitOfWork.Save();

                return RedirectToAction("Index");
            }

            return View(company);
        }

        public IActionResult Edit(int Id)
        {
            var company = _unitOfWork.Company.FirstOrDefault(x => x.Id == Id);

            if (Id == 0 || company == null)
            {
                return NoContent();
            }

            return View("Upsert", company);
        }


        [HttpDelete]
        public IActionResult Delete(int Id)
        {
            if (Id == 0) return NoContent();

            var company = _unitOfWork.Company.FirstOrDefault(x => x.Id == Id);

            if (company == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Company.Remove(company);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }
    }
}
