using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace SantasWishlistWeb.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class MustBeSingularAttribute : ValidationAttribute
    {
        private readonly string _singularGift;
        public MustBeSingularAttribute(string singularGift)
        {
            _singularGift = singularGift;
        }
        protected override ValidationResult? IsValid(object value, ValidationContext context)
        {
            //var wishlistService = context.GetRequiredService<WishlistService>();           
            List<string> gifts = value as List<string>;

            if (ContainsGift(gifts, _singularGift) && gifts.Count > 1)
            {
                return new ValidationResult($"Je mag alleen {_singularGift} kiezen als dat je enige cadeau is.");
            }

            return ValidationResult.Success;
        }

        private bool ContainsGift(List<string> gifts, string giftName)
        {
            if (gifts.IsNullOrEmpty())
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
        public override object TypeId
        {
            get
            {
                return this;
            }
        }
    }
}
