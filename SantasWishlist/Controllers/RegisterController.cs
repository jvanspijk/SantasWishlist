using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SantasWishlist.Context;
using SantasWishlistWeb.Viewmodels;
using System.Data;

namespace SantasWishlistWeb.Controllers
{
    [Authorize(Roles = "Santa")]
    public class RegisterController : Controller
    {
        private UserManager<SantasWishlistUser> _userManager;
        
        public RegisterController(UserManager<SantasWishlistUser> userManager)
        {
            _userManager = userManager;            
        }      

        
        public IActionResult Create()
        {            
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RegisterForm form)
        {         
            if(!ModelState.IsValid)
            {
                return View(form);
            }

            try
            {
                CreateUsers(form);                
                return RedirectToAction("Success", form);
            }
            catch
            {
                ModelState.AddModelError("", "Er is iets mis gegaan bij het aanmaken van de accounts.");
                return View(form);
            }
        }

        
        public IActionResult Success(RegisterForm form)
        {
            return View(form);
        }    
        
        private void CreateUsers(RegisterForm form)
        {
            var hasher = new PasswordHasher<IdentityUser>();

            var userNames = form.GetNamesList();
            foreach (var name in userNames)
            {                
                SantasWishlistUser user = new();
                user.UserName = name;                       
                user.PasswordHash = hasher.HashPassword(user, form.Password.ToLower());
                user.WasGood = form.WereGood;
                user.SentWishlist = false;

                CreateUser(user);
            }  
            
        }

        private bool CreateUser(SantasWishlistUser user)
        {
            const string roleName = "Child";
            IdentityResult userResult = _userManager.CreateAsync(user).Result;
            if(userResult.Succeeded)
            {
                _userManager.AddToRoleAsync(user, roleName).Wait();
                return true;
            }
            return false;
        }
        
    }
}
