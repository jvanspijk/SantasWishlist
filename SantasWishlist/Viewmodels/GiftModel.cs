using SantasWishlist.Domain;

namespace SantasWishlistWeb.Viewmodels
{
    public class GiftModel
    {
        public string Name { get; set; }
        public bool IsChosen { get; set; }
        public GiftCategory Category { get; set; }
    }
}
