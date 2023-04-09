using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SantasWishlist.Domain;
using SantasWishlistWeb.ValidationAttributes;
using SantasWishlistWeb.Viewmodels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SantasWishlist.Test.ValidationAttributes
{
    [TestClass()]
    public class CannotCombineAttributeTests
    {        
        private MockModel _model;
        private List<ValidationResult> _results;

        [TestInitialize]
        public void Setup()
        {
            _model = new MockModel();            
        }
        [TestMethod()]
        public void Wishlist_LegoAndKnex_ReturnsFalse()
        {           
            List<string> gifts = new();
            gifts.Add("Lego");
            gifts.Add("K'nex"); 
            _model.ChosenGifts = gifts;            
            var validationContext = new ValidationContext(_model)
            {
                MemberName = "ChosenGifts"
            };
            _results = new List<ValidationResult>();

            var result = Validator.TryValidateProperty(_model.ChosenGifts, validationContext, _results);

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void Wishlist_LegoAndDuplo_ReturnsTrue()
        {
            List<string> gifts = new();
            gifts.Add("Lego");
            gifts.Add("Duplo");        
            _model.ChosenGifts = gifts;
            var validationContext = new ValidationContext(_model)
            {
                MemberName = "ChosenGifts"
            };
            _results = new List<ValidationResult>();

            var result = Validator.TryValidateProperty(_model.ChosenGifts, validationContext, _results);

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void Wishlist_LegoBookAndKnex_ReturnsTrue()
        {
            List<string> gifts = new();
            gifts.Add("lego for dummies");
            gifts.Add("K'nex");
            _model.ChosenGifts = gifts;
            var validationContext = new ValidationContext(_model)
            {
                MemberName = "ChosenGifts"
            };
            _results = new List<ValidationResult>();

            var result = Validator.TryValidateProperty(_model.ChosenGifts, validationContext, _results);

            Assert.IsTrue(result);
        }

        public void Wishlist_NullList_ReturnsTrue()
        {
            _model.ChosenGifts = null;
            var validationContext = new ValidationContext(_model)
            {
                MemberName = "ChosenGifts"
            };
            _results = new List<ValidationResult>();
            var result = Validator.TryValidateProperty(_model.ChosenGifts, validationContext, _results);

            Assert.IsTrue(result);
        }
        private class MockModel
        {
            [CannotCombine("Lego", "K'nex")]
            public List<string> ChosenGifts { get; set; }
        }
    }
}