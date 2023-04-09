using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SantasWishlist.Context;
using SantasWishlistWeb.Viewmodels;

namespace SantasWishlistWeb.Controllers
{
    public class RegisterController : Controller
    {
        private UserManager<SantasWishlistUser> _userManager;
        private SignInManager<SantasWishlistUser> _signInManager;
        public RegisterController(UserManager<SantasWishlistUser> userManager, SignInManager<SantasWishlistUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }      

        
        public IActionResult Create()
        {            
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RegisterForm form)
        {            
            try
            {
                return RedirectToAction("Success", form);
            }
            catch
            {
                return View(form);
            }
        }

        
        public IActionResult Success(RegisterForm form)
        {
            return View(form);
        }     
        
    }
}
