using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            this._unitOfWork = unitOfWork;
            this._webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAllProducts() {
            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = productList });
        }

        public IActionResult Upsert(int? Id)
        {
            ProductViewModel productViewModel = new()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                })
            };

            if (Id != null && Id != 0)
            {
                productViewModel.Product = _unitOfWork.Product.FirstOrDefault(x => x.Id == Id) ?? new();
            }

            return View(productViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Upsert(ProductViewModel productViewModel, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null) {
                    string fileName = file.FileName;
                    string uploadDir = Path.Combine(wwwRootPath, @"images\product");
                    //string extension = Path.GetExtension(file.FileName);

                    if (productViewModel.Product.ImageUrl != null) {
                        var oldImage = Path.Combine(wwwRootPath, productViewModel.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImage)) {
                            System.IO.File.Delete(oldImage);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploadDir, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }

                    productViewModel.Product.ImageUrl = @"\images\product\" + fileName;
                }
                
                if (productViewModel.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productViewModel.Product);
                    TempData["message"] = "Save successfully";
                }
                else
                {
                    _unitOfWork.Product.Update(productViewModel.Product);
                    TempData["message"] = "Update successfully";
                }
                _unitOfWork.Save();

                return RedirectToAction("Index");
            }

            return View(productViewModel);
        }

        public IActionResult Edit(int Id)
        {
            var product = _unitOfWork.Product.FirstOrDefault(x => x.Id == Id);

            if (Id == 0 || product == null)
            {
                return NoContent();
            }

            return View("Upsert", product);
        }


        [HttpDelete]
        public IActionResult Delete(int Id)
        {
            if (Id == 0) return NoContent();

            var product = _unitOfWork.Product.FirstOrDefault(x => x.Id == Id);

            if (product == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImage = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImage))
            {
                System.IO.File.Delete(oldImage);
            }

            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful"});
        }
    }
}
