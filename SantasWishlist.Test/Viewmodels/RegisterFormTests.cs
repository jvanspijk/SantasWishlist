using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SantasWishlist.Context;
using SantasWishlistWeb.Viewmodels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SantasWishlist.Test.Viewmodels
{
    [TestClass()]
    public class RegisterFormTests
    {
        private RegisterForm _testForm;
        private List<string> _expectedNames;
        private ValidationContext _mockContext;


        [TestInitialize]
        public void Setup()
        {
            _testForm = new RegisterForm();
            _expectedNames = new List<string>()
            {
                "bob",
                "jan",
                "kees",
                "willem"
            };
            var mockExistingUser = new SantasWishlistUser { UserName = "pietje" };
            var mockUserManager = new Mock<UserManager<SantasWishlistUser>>(
                Mock.Of<IUserStore<SantasWishlistUser>>(), null, null, null, null, null, null, null, null);
            mockUserManager
                .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((string name) =>
                 name == mockExistingUser.UserName ? mockExistingUser : null);
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(UserManager<SantasWishlistUser>))).Returns(mockUserManager.Object);
            _mockContext = new ValidationContext(new object(), mockServiceProvider.Object, null);
        }


        [TestMethod()]
        public void GetNamesList_CommaSeperatedInputReturnsListOfNames_ReturnsTrue()
        {
            _testForm.NamesInput = "Bob, Jan, Kees, Willem";

            bool namesListsAreEqual = _testForm.GetNamesList().SequenceEqual(_expectedNames);

            Assert.IsTrue(namesListsAreEqual);
        }

        [TestMethod()]
        public void GetNamesList_SpaceSeperatedInputReturnsListOfNames_ReturnsTrue()
        {
            _testForm.NamesInput = "Bob Jan Kees Willem";

            bool namesListsAreEqual = _testForm.GetNamesList().SequenceEqual(_expectedNames);

            Assert.IsTrue(namesListsAreEqual);
        }

        [TestMethod()]
        public void GetNamesList_InvalidInputReturnsListOfNames_ReturnsFalse()
        {
            _testForm.NamesInput = "BobJanKeesWillem";

            bool namesListsAreEqual = _testForm.GetNamesList().SequenceEqual(_expectedNames);

            Assert.IsFalse(namesListsAreEqual);
        }

        [TestMethod()]
        public void GetNamesList_SingleNameReturnsListOfOneName_ReturnsTrue()
        {
            _testForm.NamesInput = "Bob";

            bool namesListsAreEqual = _testForm.GetNamesList().SequenceEqual(new List<string> { "bob" });

            Assert.IsTrue(namesListsAreEqual);
        }        

        [TestMethod]
        public void Validate_ReturnsNoError_WhenNoDuplicateOrExistingUserNames()
        {                    
            var form = new RegisterForm();
            form.NamesInput = "Bob, Jan, Piet";

            var results = form.Validate(_mockContext).ToList();

            Assert.IsFalse(results.Any());
        }
        

        [TestMethod]
        public void Validate_ReturnsError_WhenDuplicateUserNames()
        {            
            var form = new RegisterForm();
            form.NamesInput = "Bob, Jan, Bob";

            var results = form.Validate(_mockContext).ToList();

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Je hebt dezelfde persoon meerdere keren in de lijst staan.", results[0].ErrorMessage);
        }
        [TestMethod]
        public void Validate_ReturnsError_WhenExistingUserNames()
        {                            
            var form = new RegisterForm();
            form.NamesInput = "Jan, Bob, Pietje";
           
            var results = form.Validate(_mockContext).ToList();
            
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("pietje heeft al een account.", results[0].ErrorMessage);
        }
    }
}