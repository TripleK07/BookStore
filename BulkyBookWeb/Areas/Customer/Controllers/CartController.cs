using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewModel shoppingCart { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCart = new ShoppingCartViewModel()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product")
            };

            foreach (var item in shoppingCart.ListCart)
            {
                var product = _unitOfWork.Product.FirstOrDefault(x => x.Id == item.ProductId);
                item.Price = getPriceBasedOnQuantity(product, item.Count);
                shoppingCart.CartTotal += (item.Price * item.Count);
            }

            return View(shoppingCart);
        }

        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefault(x => x.Id == cartId);    
            _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));  
        }

        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefault(x => x.Id == cartId);
            if (cart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
            }
            else
            {
                _unitOfWork.ShoppingCart.DecrementCount(cart, 1);
            }

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }


        private double getPriceBasedOnQuantity(Product product, int quantity) {
            if (quantity <= 50)
            {
                return product.Price;
            }
            else {
                if (quantity <= 100)
                {
                    return product.Price50;
                }
                return product.Price100;
            }
        }
    }
}
