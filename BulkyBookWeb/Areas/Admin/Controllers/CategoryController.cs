using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categories = unitOfWork.Category.GetAll();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Name.Equals(category.DisplayOrder.ToString()))
                {
                    ModelState.AddModelError("name", "Name cannot be the same with display order");
                    return View(category);
                }
                else
                {
                    if (category.Id != 0)
                    {
                        unitOfWork.Category.Update(category);
                        TempData["message"] = "Update successfully";
                    }
                    else
                    {
                        category.CreatedDateTime = DateTime.Now;
                        unitOfWork.Category.Add(category);
                        TempData["message"] = "Save successfully";
                    }

                    unitOfWork.Save();

                    return RedirectToAction("Index");
                }
            }
            else
            {
                //this will show in validation summary because keyname is not model properties
                ModelState.AddModelError("KeyName", "Please fix these following errors");
            }

            return View(category);
        }

        public IActionResult Edit(int Id)
        {
            if (Id == 0) return NoContent();

            var category = unitOfWork.Category.FirstOrDefault(x => x.Id == Id);

            if (category == null)
            {
                return NoContent();
            }

            return View("Create", category);
        }


        //Custom Action Name to invoke this action
        //[ActionName("DeletePost")]
        public IActionResult Delete(int Id)
        {

            var category = unitOfWork.Category.FirstOrDefault(x => x.Id == Id);

            if (category == null)
            {
                return NotFound();
            }

            unitOfWork.Category.Remove(category);
            unitOfWork.Save();

            TempData["message"] = "Deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
