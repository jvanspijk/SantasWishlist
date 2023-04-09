using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SantasWishlistWeb.Viewmodels;
using System;
using System.Collections.Generic;
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


        [TestInitialize]
        public void Setup()
        {
            _testForm = new RegisterForm();
            _expectedNames = new List<string>()
            {
                "Bob",
                "Jan",
                "Kees",
                "Willem"
            };
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
        public void GetNamesList_InvalidInputReturnsEmptyList_ReturnsTrue()
        {
            _testForm.NamesInput = "BobJanKeesWillem";

            bool namesListIsEmpty = _testForm.GetNamesList().Count == 0;

            Assert.IsTrue(namesListIsEmpty);
        }
    }
}