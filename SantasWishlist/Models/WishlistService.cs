using SantasWishlist.Domain;

namespace SantasWishlistWeb.Models
{
    public class WishlistService : IWishlistService
    {
        public bool ContainsGift(List<Gift> gifts, string giftName)
        {
            giftName = giftName.ToLower();
            if (gifts.Any(g => g.Name.ToLower() == giftName))
            {
                return true;
            }
            return false;
        }
    }
}
