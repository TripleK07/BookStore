using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties: ("Category,CoverType"));
            return View(products);
        }

        public IActionResult Details(int productId) {

            var product = _unitOfWork.Product.FirstOrDefault(x => x.Id == productId, includeProperties: ("Category,CoverType"));

            ShoppingCart cart = new()
            {
                Count = 1,
                Product = product,
                ProductId = product.Id,
                Price = product.Price,
            };

            return View(cart);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            var dbShoppingCart = _unitOfWork.ShoppingCart.FirstOrDefault(x => x.ProductId == shoppingCart.ProductId && x.ApplicationUserId == shoppingCart.ApplicationUserId);

            if (dbShoppingCart == null)
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
            }
            else {
                _unitOfWork.ShoppingCart.IncrementCount(dbShoppingCart, shoppingCart.Count);
            }
            
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}