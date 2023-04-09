using Microsoft.VisualStudio.TestTools.UnitTesting;
using SantasWishlist.Domain;
using SantasWishlistWeb.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SantasWishlist.Test.ValidationAttributes
{
    [TestClass()]
    public class MustCombineAttributeTests
    {
        private MockModel _model;
        private List<ValidationResult> _results;

        [TestInitialize]
        public void Setup()
        {
            _model = new MockModel();
        }
        [TestMethod()]
        public void Wishlist_NachtlampjeAndOndergoed_ReturnsTrue()
        {
            List<string> gifts = new();
            gifts.Add("Nachtlampje");
            gifts.Add("Ondergoed");
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
        public void Wishlist_NachtlampjeWithoutOndergoed_ReturnsFalse()
        {
            List<string> gifts = new();
            gifts.Add("Nachtlampje");
            gifts.Add("Duplo");
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
        public void Wishlist_MuziekinstrumentAndOordopjes_ReturnsTrue()
        {
            List<string> gifts = new();
            gifts.Add("Muziekinstrument");
            gifts.Add("Oordopjes");
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
        public void Wishlist_MuziekinstrumentWithoutOordopjes_ReturnsFalse()
        {
            List<string> gifts = new();
            gifts.Add("Muziekinstrument");
            gifts.Add("Duplo");
            _model.ChosenGifts = gifts;
            var validationContext = new ValidationContext(_model)
            {
                MemberName = "ChosenGifts"
            };
            _results = new List<ValidationResult>();

            var result = Validator.TryValidateProperty(_model.ChosenGifts, validationContext, _results);

            Assert.IsFalse(result);
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
            [MustCombine("Muziekinstrument", "Oordopjes")]
            [MustCombine("Nachtlampje", "Ondergoed")]            
            public List<string> ChosenGifts { get; set; }
        }
    }
}