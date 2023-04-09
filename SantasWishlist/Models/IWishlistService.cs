using SantasWishlist.Domain;

namespace SantasWishlistWeb.Models
{
    public interface IWishlistService
    {
        public bool ContainsGift(List<Gift> gifts, string giftName);
    }
}
