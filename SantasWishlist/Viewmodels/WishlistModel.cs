using Microsoft.IdentityModel.Tokens;
using SantasWishlist.Domain;
using SantasWishlistWeb.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace SantasWishlistWeb.Viewmodels
{
    public class WishlistModel : IValidatableObject
    {
        public UserModel User { get; set; }
        /// <summary>
        /// All the gifts from the database that a child can choose from
        /// </summary>        
        public List<GiftModel>? Gifts { get; set; }
        /// <summary>
        /// All the chosen gifts, by name
        /// </summary>
        [MustCombine("Muziekinstrument", "Oordopjes")]
        [MustCombine("Nachtlampje", "Ondergoed")]
        [CannotCombine("Lego", "K`nex")]
        [MustBeSingular("Spelcomputer")]
        public List<string> ChosenGiftNames { get; set; }
        /// <summary>
        /// A string containing extra gifts not on the main list, seperated by commas
        /// </summary>        
        public string? ExtraGifts { get; set; }
        /// <summary>
        /// Extracts the seperate gifts from the ExtraGifts string
        /// </summary>
        /// <returns>List of gift names that were not on the main list</returns>
        public List<string> GetExtraGiftsList()
        {
            var giftList = new List<string>();
            if (ExtraGifts.IsNullOrEmpty())
            {
                return giftList;
            }

            if (ExtraGifts.Contains(','))
            {
                var splitGifts = ExtraGifts.Split(',');
                foreach(string gift in splitGifts)
                {
                    giftList.Add(gift.Trim());
                }
                return giftList.ToList();
            }
            else
            {
                giftList.Add(ExtraGifts);
                return giftList;
            }          
        }
        /// <summary>
        /// Gifts from the database list that have the category 'WANT'
        /// </summary>
        /// <returns></returns>
        public List<GiftModel> GetWantGifts()
        {
            return Gifts.Where(g => g.Category == GiftCategory.WANT).ToList() ?? new();
        }
        /// <summary>
        /// Gifts from the database list that have the category 'NEED'
        /// </summary>
        /// <returns></returns>
        public List<GiftModel> GetNeedGifts()
        {
            return Gifts.Where(g => g.Category == GiftCategory.NEED).ToList() ?? new();
        }
        /// <summary>
        /// Gifts from the database list that have the category 'WEAR'
        /// </summary>
        /// <returns></returns>
        public List<GiftModel> GetWearGifts()
        {
            return Gifts.Where(g => g.Category == GiftCategory.WEAR).ToList() ?? new();
        }
        /// <summary>
        /// Gifts from the database list that have the category 'READ'
        /// </summary>
        /// <returns></returns>
        public List<GiftModel> GetReadGifts()
        {
            return Gifts.Where(g => g.Category == GiftCategory.READ).ToList() ?? new();
        }
        /// <summary>
        /// Gifts that the user has chosen from the database list, from all categories
        /// </summary>
        /// <returns>A list of type GiftModel</returns>
        private List<GiftModel> GetChosenGifts()
        {
            return Gifts.Where(g => g.IsChosen == true).ToList() ?? new();
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var repo = validationContext.GetRequiredService<IGiftRepository>();
            Gifts = repo.GetPossibleGifts().ToList().Select(g => new GiftModel
            {
                Name = g.Name,
                IsChosen = ChosenGiftNames.Contains(g.Name),
                Category = g.Category
            }).ToList();
            int maxGiftsOutsideOfAge = 1;           
            if(ChoseMoreThanAllowedGiftsPerCategory())
            {
                //Lied is always false here because Good would also be true in the case of Lied == true
                if(!User.WasGood)
                {
                    yield return new ValidationResult("Je mag maar 1 cadeautje per categorie uitkiezen.");
                }                
                else
                {
                    yield return new ValidationResult("Je mag maar 3 cadeautjes per categorie uitkiezen.");
                }                
            }
            if(ChoseMoreThanTotalAllowedGifts())
            {
                if(User.Lied)
                {
                    yield return new ValidationResult("Je mag maar 1 cadeautje in totaal uitkiezen.");
                }
                else
                {
                    yield return new ValidationResult("Je kiest wel heel veel cadeautjes uit!");
                }                
            }            
            if (ExtraGiftsContainsGiftFromPossibleGifts())
            {
                yield return new ValidationResult("Je hebt een cadeautje ingevuld die al in het lijstje staat.");
            }
            if(CalculateAmountOfGiftsOutsideOfAgeRange(validationContext) > maxGiftsOutsideOfAge)
            {
                string errorString = "Je bent nog niet oud genoeg om deze cadeautjes te kiezen: ";               
                
                foreach(string giftName in GetGiftsOutsideOfAgeRange(validationContext))
                {                    
                    errorString += $"{giftName}, ";                    
                }
                yield return new ValidationResult(errorString);
            }            
        }
        private bool ChoseMoreThanAllowedGiftsPerCategory()
        {
            if(IsExtraGood())
            {
                return false;
            }

            int maxGiftsPerCategory = CalculateAllowedGiftsPerCategory();
            List<int> chosenAmountGiftsPerCategory = CalculateAmountChosenGiftsPerCategory();

            foreach(int chosenAmount in chosenAmountGiftsPerCategory)
            {
                if(chosenAmount> maxGiftsPerCategory)
                {
                    return true;
                }
            }           
            
            return false;
        }       
        private bool ChoseMoreThanTotalAllowedGifts()
        {            
            if (IsExtraGood())
            {
                return false;
            }

            int allowedAmountOfGifts = CalculateTotalAllowedGifts();
            if(CalculateTotalAmountChosenGifts() > allowedAmountOfGifts)
            {
                return true;
            }

            return false;
        }
        private int CalculateAllowedGiftsPerCategory()
        {            
            int normalMaxGifts = 3;
            int badMaxGifts = 1;
           
            if (!User.WasGood)
            {
                return badMaxGifts;
            }
            return normalMaxGifts;
        }
        private List<int> CalculateAmountChosenGiftsPerCategory() 
        {
            List<int> giftAmountPerCategory = new()
            {
                GetChosenGifts().Where(g => g.Category == GiftCategory.WANT).Count(),
                GetChosenGifts().Where(g => g.Category == GiftCategory.NEED).Count(),
                GetChosenGifts().Where(g => g.Category == GiftCategory.WEAR).Count()                
            };
            List<GiftModel> readGifts = GetChosenGifts().Where(g => g.Category == GiftCategory.READ).ToList();
            if (User.Name.ToLower() == "stijn")
            {
                giftAmountPerCategory.Add(readGifts.Where(g => !g.Name.ToLower().Contains("dolfje weerwolfje")).Count());
            }
            else
            {
                giftAmountPerCategory.Add(readGifts.Count());
            }
            return giftAmountPerCategory;
        }
        private int CalculateTotalAllowedGifts()
        {
            if (User.Lied)
            {
                return 1;
            }

            return 1000;
        }
        private int CalculateTotalAmountChosenGifts()
        {
            int totalAmount = 0;
            foreach(int categoryAmount in CalculateAmountChosenGiftsPerCategory())
            {
                totalAmount += categoryAmount;
            }
            return totalAmount;
        }
        private bool IsExtraGood()
        {
            if (User.GoodDescription != null)
            {                
                if (User.GoodDescription.ToLower().Contains("vrijwilligerswerk") && User.WasGood)
                {
                    return true;
                }
            }
            return false;
        }
        private bool ExtraGiftsContainsGiftFromPossibleGifts()
        {
            if(ExtraGifts.IsNullOrEmpty())
            {
                return false;
            }

            List<string> extraGifts = ExtraGifts.ToLower().Replace(", ", ",").Replace(" ,", ",").Split(',').ToList();
            foreach (var gift in Gifts)
            {
                string giftName = gift.Name.ToLower();
                if (extraGifts.Contains(giftName))
                {
                    return true;
                }
            }
            return false;
        }
        private int CalculateAmountOfGiftsOutsideOfAgeRange(ValidationContext context)
        {
            return GetGiftsOutsideOfAgeRange(context).Count;
        }
        private List<string> GetGiftsOutsideOfAgeRange(ValidationContext context)
        {
            List<string> disallowedGiftNames = new();
            if (User == null)
            {
                return disallowedGiftNames;
            }

            IGiftRepository repo;
            try
            {
                repo = context.GetRequiredService<IGiftRepository>();
            }
            catch (Exception ex)
            {
                repo = new GiftRepository();
            }
            
            foreach (var gift in GetChosenGifts())
            {
                int ageMinimum = repo.CheckAge(gift.Name);
                if (User.Age < ageMinimum)
                {
                    disallowedGiftNames.Add(gift.Name);
                }
            }
            return disallowedGiftNames;
        }
    }    
}
