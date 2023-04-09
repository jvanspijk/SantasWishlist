using Microsoft.IdentityModel.Tokens;
using SantasWishlist.Domain;
using SantasWishlistWeb.Models;
using SantasWishlistWeb.Viewmodels;
using System.ComponentModel.DataAnnotations;

namespace SantasWishlistWeb.ValidationAttributes
{
    /// <summary>
    /// If either one gift is chosen, both must be chosen at the same time.
    /// </summary>    
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class MustCombineAttribute : ValidationAttribute
    {
        private readonly string _firstGift;
        private readonly string _secondGift;
        public MustCombineAttribute(string firstGift, string secondGift)
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
                return ValidationResult.Success;
            }
            else if(ContainsGift(gifts, _firstGift) || ContainsGift(gifts, _secondGift))
            {
                return new ValidationResult($"Je moet {_firstGift} en {_secondGift} samen kiezen.");
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
            if (gifts.Any(g => g.ToLower() == giftName))
            {
                return true;
            }
            return false;
        }
    }
}
