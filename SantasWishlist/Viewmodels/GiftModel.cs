using SantasWishlist.Domain;
using System;

namespace SantasWishlistWeb.Viewmodels
{
    public class GiftModel
    {
        public string Name { get; set; }
        public bool IsChosen { get; set; }
        public GiftCategory Category { get; set; }
        public override bool Equals(object obj)
        {
            return this.Equals(obj as GiftModel);
        }
        public bool Equals(GiftModel gift)
        {
            if (gift.Name.ToLower() == Name.ToLower())
            {
                return true;
            }
            return false;
        }
    }
}
