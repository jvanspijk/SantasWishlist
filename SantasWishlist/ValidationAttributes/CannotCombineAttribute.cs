using Microsoft.IdentityModel.Tokens;
using SantasWishlist.Domain;
using SantasWishlistWeb.Models;
using SantasWishlistWeb.Viewmodels;
using System.ComponentModel.DataAnnotations;

namespace SantasWishlistWeb.ValidationAttributes
{
    /// <summary>
    /// Checks if two specific gifts are in the list that are not allowed to be combined
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CannotCombineAttribute : ValidationAttribute
    {
        private readonly string _firstGift;
        private readonly string _secondGift;
        public CannotCombineAttribute(string firstGift, string secondGift)
        {
            _firstGift = firstGift;
            _secondGift = secondGift;
        }
        public override object TypeId
        {
            get
            {
                return this;
            }
        }
        protected override ValidationResult? IsValid(object value, ValidationContext context)
        {
            //var wishlistService = context.GetRequiredService<WishlistService>();           
            List<string> gifts = value as List<string>;
            
            if (ContainsGift(gifts, _firstGift) && ContainsGift(gifts, _secondGift))
            {
                return new ValidationResult($"Je mag niet {_firstGift} en {_secondGift} tegelijk kiezen.");
            }

            return ValidationResult.Success;
        }

        private bool ContainsGift(List<string> gifts, string giftName)
        {
            if(gifts.IsNullOrEmpty())
            {
                return false;
            }
            giftName = giftName.ToLower();
            if(gifts.Any(g => g.ToLower() == giftName))
            {
                return true;
            }
            return false;
        }
    }
}
