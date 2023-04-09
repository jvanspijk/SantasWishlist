using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SantasWishlist.Context;
using SantasWishlist.Domain;
using SantasWishlistWeb.Controllers;
using SantasWishlistWeb.Viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SantasWishlistWeb.Controllers.Tests
{
    [TestClass()]
    public class WishlistControllerTests
    {
        private Mock<ClaimsPrincipal> _mockUser;
        private WishlistController _controller;


        [TestInitialize]
        public void Setup()
        {
            var mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.FindFirst(It.IsAny<string>())).Returns(new Claim(ClaimTypes.NameIdentifier, "123"));


            var mockUserManager = new Mock<UserManager<SantasWishlistUser>>(
                Mock.Of<IUserStore<SantasWishlistUser>>(), null, null, null, null, null, null, null, null);

            var mockGiftRepository = new Mock<IGiftRepository>();
            mockGiftRepository.Setup(g => g.GetPossibleGifts()).Returns(new List<Gift>
            {
                new Gift { Name = "Speelgoed", Category = GiftCategory.WANT },
                new Gift { Name = "Boek", Category = GiftCategory.READ },
            });           

            _controller = new WishlistController(mockGiftRepository.Object, mockUserManager.Object);
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = mockPrincipal.Object }
            };
        }
        [TestMethod]
        public void AboutMe_ReturnsViewResultWithCorrectUserModel()
        {
            var result = _controller.AboutMe() as ViewResult;
            var model = result.ViewData.Model as UserModel;
           
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.AreEqual("123", model.Id);            
        }

        [TestMethod]
        public void ChooseGifts_ModelIsInvalid_ReturnsViewWithModel()
        {
            var wishlistModel = new WishlistModel { ChosenGiftNames = new(){ "Boek" } };
            _controller.ModelState.AddModelError("", "Test fout.");
           
            var result = _controller.ChooseGifts(wishlistModel) as ViewResult;
           
            Assert.IsNotNull(result);
            Assert.AreEqual(_controller.ModelState.IsValid, false);
            Assert.AreEqual(wishlistModel, result.Model);
        }

        [TestMethod]
        public void ChooseGifts_ModelIsValid_ReturnsSuccessViewWithModel()
        {           
            var wishlistModel = new WishlistModel
            {
                ChosenGiftNames = new List<string> { "Speelgoed" },
                ExtraGifts = "Boek",
                User = new UserModel { WasGood = true }
            };
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Headers["Referer"] = "http://localhost:5000/AboutMe";
            
            var result = _controller.ChooseGifts(wishlistModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Success", result.ViewName);
            Assert.AreEqual(wishlistModel, result.Model);
        }
    }
}