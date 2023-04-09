using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SantasWishlist.Context;
using SantasWishlist.Domain;
using SantasWishlistWeb.Viewmodels;
using System.Security.Claims;

namespace SantasWishlistWeb.Controllers
{
    public class WishlistController : Controller
    {
        private readonly IGiftRepository _giftRepository;
        private readonly UserManager<SantasWishlistUser> _userManager;
        public WishlistController(IGiftRepository giftRepository, UserManager<SantasWishlistUser> userManager) 
        {
            _giftRepository = giftRepository;
            _userManager = userManager;
        }
        [Authorize(Roles = "Child")]
        public ActionResult AboutMe()
        {
            UserModel model = new();            
            model.Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.Name = User.FindFirstValue(ClaimTypes.Name);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult> AboutMeAsync(UserModel userModel)
        {
            if(!ModelState.IsValid)
            {
                return View(userModel);
            }
            if (await CheckIfUserLied(userModel))
            {
                userModel.Lied = true;
            }

            WishlistModel wishlist = SetupWishlistModel(userModel);
            
            return View("ChooseGifts", wishlist);
        }
        
        private async Task<bool> CheckIfUserLied(UserModel userModel)
        {
            bool userLied = false;
            var user = await _userManager.FindByNameAsync(userModel.Name);
            if(user != null)
            {
                userLied = userModel.WasGood && !user.WasGood;
            }                     
            return userLied;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Child")]
        public ActionResult ChooseGifts(WishlistModel wishlistModel)
        {
            SetGiftsInModel(wishlistModel);
            if (!ModelState.IsValid)
            {                
                return View(wishlistModel);
            }
            var chosenGifts = GetChosenGifts(wishlistModel).Select(g => new GiftModel
            {
                Name = g.Name,
                IsChosen = true,
                Category = g.Category
            }).ToList();
            
            return View("Success", wishlistModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Child")]
        public ActionResult ChooseGiftsBack(WishlistModel wishlistModel)
        {
            SetGiftsInModel(wishlistModel);            
            var chosenGifts = GetChosenGifts(wishlistModel).Select(g => new GiftModel
            {
                Name = g.Name,
                IsChosen = true,
                Category = g.Category
            }).ToList();

            return View("ChooseGifts", wishlistModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Child")]
        public  IActionResult Success(WishlistModel wishlistModel)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "We hebben je poging tot hacken onderschept, " +
                    "je gegevens zijn doorgestuurd naar de AIVD.");
                return View(wishlistModel);
            }            
            WishList wishList = CreateWishlist(wishlistModel);
            //_giftRepository.SendWishList(wishList);
            return RedirectToAction("Logout", "Account");
        }       

        private WishList CreateWishlist(WishlistModel wishlistModel)
        {
            var chosenGifts = GetChosenGifts(wishlistModel);
            var extraGifts = wishlistModel.GetExtraGiftsList();

            foreach (var extraGift in extraGifts)
            {
                Gift gift = new()
                {
                    Name = extraGift,
                    Category = GiftCategory.WANT
                };
                chosenGifts.Add(gift);
            }
            
            WishList wishList = new();
            wishList.Name = wishlistModel.User.Id;
            wishList.Wanted = chosenGifts;
            return wishList;
        }

        private void PopulateGifts(ref WishlistModel model)
        {
            var gifts = _giftRepository.GetPossibleGifts();
            var possibleGifts = gifts.Select(g => new GiftModel
            {
                Name = g.Name,
                IsChosen = false,
                Category = g.Category
            }).ToList();

            model.Gifts = possibleGifts;
        }    

        private WishlistModel SetupWishlistModel(UserModel user)
        {
            WishlistModel wishlist = new();
            wishlist.User = user;
            PopulateGifts(ref wishlist);            
            return wishlist;
        }
        
        private void SetGiftsInModel(WishlistModel model)
        {
            var gifts = _giftRepository.GetPossibleGifts();
            List<GiftModel> possibleGifts = gifts.Select(g => new GiftModel
            {
                Name = g.Name,
                IsChosen = model.ChosenGiftNames.Contains(g.Name),
                Category = g.Category
            }).ToList();

            model.Gifts = possibleGifts;
        }

        private List<Gift> GetChosenGifts(WishlistModel wishlistModel)
        {
            List<Gift> chosenGifts = new();
            foreach (string name in wishlistModel.ChosenGiftNames)
            {
                try
                {
                    Gift gift = _giftRepository.GetPossibleGifts().FirstOrDefault(g => g.Name == name);
                    chosenGifts.Add(gift);
                }
                catch
                {
                    continue;
                }
            }
            return chosenGifts;
        }
    }
}
