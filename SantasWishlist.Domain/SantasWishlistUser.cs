using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SantasWishlist.Domain
{
    public class SantasWishlistUser : IdentityUser
    {
        public int? Age { get; set; }        
        public GoodType GoodType { get; set; }
    }

    public enum GoodType
    {
        BAD, TRIED, AVERAGE, VERY
    }
}
