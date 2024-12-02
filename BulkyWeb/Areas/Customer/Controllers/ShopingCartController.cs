using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Model.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.LibraryModel;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShopingCartController : Controller
    {
        IUnitOfWork _unitOfWork;
        ShopingCartModel shopingCartModel { get; set; }
        public ShopingCartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            string? userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shopingCartModel = new()
            {
                ShopingCartList = _unitOfWork.shopingCart.GetAll(d => d.ApplicationUserId == userId,
                                    includeProperties: "Product")
            };
            foreach (var item in shopingCartModel.ShopingCartList)
            {
                item.Price = GetPriceBasedOnQuantity(item);
                shopingCartModel.OrderTotal += (item.Price * item.Count);
            }

            return View(shopingCartModel);
        }
        public IActionResult Plus(int cartId)
        {
            ShopingCart cart = _unitOfWork.shopingCart.Get(d => d.Id == cartId);
            if (cart != null)
            {
                cart.Count += 1;
                _unitOfWork.shopingCart.Update(cart);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Minus(int cartId)
        {
            ShopingCart cart = _unitOfWork.shopingCart.Get(d => d.Id == cartId);
            if (cart != null)
            {
                if (cart.Count <= 1)
                {
                    _unitOfWork.shopingCart.Remove(cart);
                }
                else
                {
                    cart.Count -= 1;
                    _unitOfWork.shopingCart.Update(cart);
                }
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Remove(int cartId)
        {
            ShopingCart cart = _unitOfWork.shopingCart.Get(d => d.Id == cartId);
            if (cart != null)
            {
                _unitOfWork.shopingCart.Remove(cart);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult OrderSummary(int cartId)
        {
            ShopingCart cart = _unitOfWork.shopingCart.Get(d => d.Id == cartId);
            if (cart != null)
            {
                _unitOfWork.shopingCart.Remove(cart);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));
        }
        public double GetPriceBasedOnQuantity(ShopingCart shopingCart)
        {
            if (shopingCart.Count <= 50)
            {
                return shopingCart.Product.Price50;
            }
            else if (shopingCart.Count <= 100)
            {
                return shopingCart.Product.Price100;
            }
            else
            {
                return shopingCart.Product.Price;
            }
        }
    }
}
