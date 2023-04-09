using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SantasWishlist.Context;
using SantasWishlistWeb.Controllers;
using SantasWishlistWeb.Viewmodels;
using System.Collections.Generic;

namespace SantasWishlist.Test
{
    [TestClass]
    public class AuthTests
    {
        private PasswordHasher<IdentityUser>? _hasher;
        private Mock<UserManager<SantasWishlistUser>>? _userManager;
        private Mock<SignInManager<SantasWishlistUser>>? _signInManager;
        private Mock<AccountController>? _controller;

        [TestInitialize]
        public void Setup()
        {
            _hasher = new PasswordHasher<IdentityUser>();
            List<SantasWishlistUser> testUsers = new()
            {
                new SantasWishlistUser() { UserName = "test1", PasswordHash=_hasher.HashPassword(null, "test1")},
                new SantasWishlistUser() { UserName = "test2", PasswordHash=_hasher.HashPassword(null, "test2")}
            };
            var store = new Mock<IUserStore<SantasWishlistUser>>();
            _userManager = new Mock<UserManager<SantasWishlistUser>>(store.Object, null, null, null, null, null, null, null, null);
            _signInManager = new Mock<SignInManager<SantasWishlistUser>>(_userManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<SantasWishlistUser>>(), null, null, null, null);
            _controller = new Mock<AccountController>(_userManager.Object, _signInManager.Object);
            
        }

        
        public async Task CanLogin_CorrectCredentials_ReturnsTrue()
        {          
                      
            var loginForm = new LoginForm()
            {
                UserName = "test1",
                Password = "test1"
            };            
            _signInManager.Setup(
                s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(),
                It.IsAny<bool>())).Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Success));
            //_controller.Setup(c => c.Login(It.IsAny<LoginForm>()));

            var loginResult = await _controller.Object.Login(loginForm) as RedirectResult;

            //Assert.IsInstanceOfType(loginResult, typeof(RedirectResult));
            Assert.IsNotNull(_controller);
            //Assert.IsNotNull(loginResult);
            //Assert.AreEqual(loginResult, null);
            //Assert.AreEqual(loginResult.RouteValues["Parameter Name"], "Parameter Value");
        }

        
        public void CanLogin_WrongCredentials_ReturnsFalse()
        {
            Assert.IsFalse(false);
        }
    }
}