﻿using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SantasWishlist.Context;
using SantasWishlistWeb.Viewmodels;

namespace SantasWishlistWeb.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<SantasWishlistUser> _userManager;
        private SignInManager<SantasWishlistUser> _signInManager;

        public AccountController(UserManager<SantasWishlistUser> userManager, SignInManager<SantasWishlistUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginForm loginForm)
        {
            if(loginForm.UserName != "Kerstman")
            {
                loginForm.Password = loginForm.Password.ToLower();
            }
            var correctCredentials = await CheckCredentialsCorrect(loginForm);
            
            if (correctCredentials)
            {
                try
                {
                    var user = _userManager.FindByNameAsync(loginForm.UserName).Result;
                    if(user.SentWishlist)
                    {
                        ModelState.AddModelError("","Je hebt al een verlanglijstje ingevuld.");
                        await _signInManager.SignOutAsync();
                        return View(loginForm);
                    }
                }
                catch
                {
                    return View(loginForm);
                }
                return Redirect("/Home");
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/Home");
        }

        private async Task<bool> CheckCredentialsCorrect(LoginForm loginForm)
        {
            var result = await _signInManager.PasswordSignInAsync(loginForm.UserName,
                   loginForm.Password, true, false);
            
            if (result.Succeeded)
            {
                return true;
            }
            return false;
        }
    }
}
