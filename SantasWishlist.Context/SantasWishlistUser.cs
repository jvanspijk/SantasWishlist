using Microsoft.AspNetCore.Identity;
using SantasWishlist.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SantasWishlist.Context
{
    public class SantasWishlistUser : IdentityUser
    {
        public int? Age { get; set; }        
        public bool WasGood { get; set; }        
        public bool SentWishlist { get; set; }
    }
    
    
}
