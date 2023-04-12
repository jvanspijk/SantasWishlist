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

        [Authorize(Roles = "Santa")]
        public IActionResult Create()
        {            
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Santa")]
        public IActionResult Create(RegisterForm form)
        {         
            if(!ModelState.IsValid)
            {
                return View(form);
            }         

            try
            {
                bool createSuccess = CreateUsers(form);
                if(createSuccess)
                {
                    return RedirectToAction("Success", form);
                }
                else
                {
                    ModelState.AddModelError("", "Accounts creëren mislukt.");
                    return View(form);
                }
            }
            catch
            {
                ModelState.AddModelError("", "Er is iets mis gegaan bij het aanmaken van de accounts.");
                return View(form);
            }
        }

        [Authorize(Roles = "Santa")]
        public IActionResult Success(RegisterForm form)
        {
            return View(form);
        }
        
        private bool CreateUsers(RegisterForm form)
        {
            var userNames = form.GetNamesList();
            if(userNames.Count == 0) 
            {
                return false;
            }
            foreach (var name in userNames)
            {                
                SantasWishlistUser user = new();
                user.UserName = name;           
                user.WasGood = form.WereGood;
                user.SentWishlist = false;
                
                if(CreateUser(user, form.Password.ToLower()) == false)
                {
                    return false;
                }
            }
            return true;            
        }

        private bool CreateUser(SantasWishlistUser user, string password)
        {
            const string roleName = "Child";
            if (_userManager.FindByNameAsync(user.UserName).Result != null)
            {
                return false;
            }
            try
            {
                IdentityResult userResult = _userManager.CreateAsync(user, password).Result;
                if (userResult.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, roleName).Wait();
                    return true;
                }
            }
            catch
            {
                return false;
            }
           
            return false;
        }
        
    }
}
