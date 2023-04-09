using SantasWishlist.Domain;

namespace SantasWishlistWeb.Viewmodels
{
    public class SuccessModel
    {
        public UserModel User { get; set; }
        public List<GiftModel> ChosenGifts { get; set; }
    }
}
