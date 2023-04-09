using Microsoft.AspNetCore.Identity;
using Moq;
using SantasWishlist.Context;
using SantasWishlist.Domain;
using SantasWishlistWeb.Viewmodels;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;


namespace SantasWishlist.Test.Viewmodels
{
    [TestClass()]
    public class WishlistModelValidationTests
    {
        private UserModel _goodUser;
        private UserModel _badUser;
        private UserModel _badUserLied;
        private UserModel _extraGoodUser;
        private UserModel _youngUser;

        private ValidationContext _mockContext;

        [TestInitialize]
        public void Setup()
        {
            SetupMockUsers();
            SetupMockContext();           
        }

        private void SetupMockUsers()
        {
            _goodUser = new UserModel()
            {
                Name = "gooduser",
                Age = 24,
                WasGood = true
            };

            _badUser = new UserModel()
            {
                Name = "baduser",
                Age = 24,
                WasGood = false
            };

            _badUserLied = new UserModel()
            {
                Name = "baduserlied",
                Age = 24,
                WasGood = true,
                Lied = true
            };

            _extraGoodUser = new UserModel()
            {
                Name = "extragooduser",
                Age = 24,
                WasGood = true,
                GoodDescription = "ik heb vrijwilligerswerk gedaan."
            };

            _youngUser = new UserModel()
            {
                Name = "younguser",
                Age = 6,
                WasGood = true,
                Lied = false,
                GoodDescription = "ik heb goed opgelet tijdens de les."
            };
        }

        private void SetupMockContext()
        {
            var gifts = CreateMockPossibleGifts();
            var mockGiftRepository = new Mock<IGiftRepository>();
            mockGiftRepository.Setup(g => g.CheckAge(It.IsAny<string>())).Returns(0);            
            mockGiftRepository.Setup(g => g.GetPossibleGifts()).Returns(gifts);
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IGiftRepository))).Returns(mockGiftRepository.Object);
            _mockContext = new ValidationContext(new object(), mockServiceProvider.Object, null);
        }

        [TestMethod()]
        public void MaximumGifts_GoodUserWithFourGiftsPerCategory_ReturnsFalse()
        {
            List<Gift> chosenGifts = CreateChosenGiftsList(4);
            WishlistModel model = SetupModel(_goodUser, chosenGifts, null);

            var results = model.Validate(_mockContext).ToList();

            Assert.AreEqual(results.Count, 1);
            Assert.AreEqual(results.First().ErrorMessage, "Je mag maar 3 cadeautjes per categorie uitkiezen.");
        }

        [TestMethod()]
        public void MaximumGifts_GoodUserWithTwoGiftsPerCategory_ReturnsTrue()
        {
            List<Gift> chosenGifts = CreateChosenGiftsList(2);
            WishlistModel model = SetupModel(_goodUser, chosenGifts, null);

            var results = model.Validate(_mockContext).ToList();

            Assert.AreEqual(results.Count(), 0);
        }

        [TestMethod()]
        public void MaximumGifts_BadUserWithTwoGiftsPerCategory_ReturnsFalse()
        {
            List<Gift> chosenGifts = CreateChosenGiftsList(2);
            WishlistModel model = SetupModel(_badUser, chosenGifts, null);

            var results = model.Validate(_mockContext).ToList();  

            Assert.AreEqual(results.Count, 1);
            Assert.AreEqual(results.First().ErrorMessage, "Je mag maar 1 cadeautje per categorie uitkiezen.");
        }

        [TestMethod()]
        public void MaximumGifts_BadUserWhoLiedWithOneGift_ReturnsTrue()
        {
            List<Gift> chosenGifts = new List<Gift>()
            {
                new Gift(){Name = "Snoepje", Category = GiftCategory.NEED}
            };
            WishlistModel model = SetupModel(_badUserLied, chosenGifts, null);           

            var results = model.Validate(_mockContext).ToList();

            Assert.AreEqual(results.Count(), 0);            
        }

        [TestMethod()]
        public void MaximumGifts_ExtraGoodUserWithEightyGifts_ReturnsTrue()
        {
            List<Gift> chosenGifts = CreateChosenGiftsList(20);
            WishlistModel model = SetupModel(_extraGoodUser, chosenGifts, null);            

            var results = model.Validate(_mockContext).ToList();

            Assert.AreEqual(results.Count(), 0);            
        }

        [TestMethod()]
        public void MaximumGifts_BadUserWithOneGiftPerCategory_ReturnsTrue()
        {
            List<Gift> chosenGifts = CreateChosenGiftsList(1);
            WishlistModel model = SetupModel(_badUser, chosenGifts, null);

            var results = model.Validate(_mockContext).ToList();

            Assert.AreEqual(results.Count(), 0);            
        }

        [TestMethod()]
        public void MaximumGifts_BadUserThatLiedWithOneGiftPerCategory_ReturnsFalse() 
        { 
            List<Gift> chosenGifts = CreateChosenGiftsList(1);
            WishlistModel model = SetupModel(_badUserLied, chosenGifts, null);

            var results = model.Validate(_mockContext).ToList();

            Assert.AreEqual(results.Count(), 1);
            Assert.AreEqual(results.First().ErrorMessage, "Je mag maar 1 cadeautje in totaal uitkiezen.");
        }

        [TestMethod()]
        public void MaximumGifts_StijnHasExtraDolfjeWeerwolfjeGift_ReturnsTrue()
        {
            List<Gift> chosenGifts = CreateChosenGiftsList(3);
            chosenGifts.Add(new Gift { Category = GiftCategory.READ, Name = "Dolfje Weerwolfje" });
            WishlistModel model = SetupModel(_goodUser, chosenGifts, null);
            model.User.Name = "Stijn";            

            var results = model.Validate(_mockContext).ToList();

            Assert.AreEqual(results.Count(), 0);
        }

        [TestMethod()]
        public void MaximumGifts_StijnHasExtraOtherGift_ReturnsFalse()
        {
            List<Gift> chosenGifts = CreateChosenGiftsList(3);
            chosenGifts.Add(new Gift { Category = GiftCategory.READ, Name = "Hoe overleef ik de middelbare school!" });
            WishlistModel model = SetupModel(_goodUser, chosenGifts, null);
            model.User.Name = "Stijn";
            

            var results = model.Validate(_mockContext).ToList();

            Assert.AreEqual(results.Count(), 1);
        }        

        [TestMethod()]
        public void ExtraGifts_NoDuplicatesInExtra_ReturnsTrue()
        {
            List<Gift> chosenGifts = CreateChosenGiftsList(1);            
            string extraGifts = "Bijbel, hoedje, toeter";
            WishlistModel model = SetupModel(_goodUser, chosenGifts, extraGifts);
            var validationContext = new ValidationContext(model);

            var results = model.Validate(_mockContext).ToList();

            Assert.AreEqual(results.Count(), 0);            
        }

        [TestMethod()]
        public void ExtraGifts_DuplicatesInExtra_ReturnsFalse()
        {
            List<Gift> chosenGifts = CreateChosenGiftsList(1);            
            string extraGifts = "Bijbel, hoedje, toeter, schoenen";
            WishlistModel model = SetupModel(_goodUser, chosenGifts, extraGifts);           
            var validationContext = new ValidationContext(model);

            var results = model.Validate(_mockContext).ToList();

            Assert.AreEqual(results.Count(), 1);
            Assert.AreEqual(results.First().ErrorMessage, "Je hebt een cadeautje ingevuld die al in het lijstje staat.");
        }

        private List<Gift> CreateChosenGiftsList(int giftsPerCategory)
        {
            if(giftsPerCategory > 4)
            {
                giftsPerCategory = 4;
            }

            var allGifts = CreateMockPossibleGifts();
            List<Gift> gifts = new();
            for(int i = 0; i < giftsPerCategory; i++)
            {
                gifts.Add(allGifts[i]);
                gifts.Add(allGifts[i + 4]);
                gifts.Add(allGifts[i + 8]);
                gifts.Add(allGifts[i + 12]);
            }
            return gifts;
        }

        [TestMethod]
        public void CreateGiftList_CreateListWithTwoPerCategoryHasEightTotalGifts_ReturnsTrue()
        {
            var gifts = CreateChosenGiftsList(2);

            Assert.AreEqual(8, gifts.Count);
        }

        [TestMethod]
        public void CreateGiftList_CreateListWithTwoPerCategoryHasFourCategories_ReturnsTrue()
        {
            var gifts = CreateChosenGiftsList(2);

            bool hasAllCategories = true;
            foreach(GiftCategory category in Enum.GetValues(typeof(GiftCategory)))
            {
                bool containsCategory = gifts.Any(g => g.Category == category);
                if(!containsCategory)
                {
                    hasAllCategories = false;
                }
            }

            Assert.IsTrue(hasAllCategories);            
        }

        [TestMethod]
        public void CreateGiftList_TwoGiftsPerCategoryHasTwoPerCategory_ReturnsTrue()
        {
            var gifts = CreateChosenGiftsList(2);
            bool twoGiftsInCategory = true;
            
            foreach (GiftCategory category in Enum.GetValues(typeof(GiftCategory)))
            {
                int amountGifts = gifts.Where(g => g.Category == category).Count();
                if (amountGifts != 2)
                {
                    twoGiftsInCategory = false;
                }
            }

            Assert.IsTrue(twoGiftsInCategory);
        }

        private List<GiftModel> CreatePossibleGiftsList(List<string> chosenGiftNames)
        {
            var list = new List<GiftModel>();
            list.Add(new GiftModel { Name = "Lego", Category = GiftCategory.WANT });
            list.Add(new GiftModel { Name = "Duplo", Category = GiftCategory.WANT });
            list.Add(new GiftModel { Name = "Roblox", Category = GiftCategory.WANT });
            list.Add(new GiftModel { Name = "Pokemonkaartjes", Category = GiftCategory.WANT });

            list.Add(new GiftModel { Name = "Shampoo", Category = GiftCategory.NEED });
            list.Add(new GiftModel { Name = "Deo", Category = GiftCategory.NEED });
            list.Add(new GiftModel { Name = "Tandenborstel", Category = GiftCategory.NEED });
            list.Add(new GiftModel { Name = "Tandpasta", Category = GiftCategory.NEED });

            list.Add(new GiftModel { Name = "Schoenen", Category = GiftCategory.WEAR });
            list.Add(new GiftModel { Name = "Trui", Category = GiftCategory.WEAR });
            list.Add(new GiftModel { Name = "Sokken", Category = GiftCategory.WEAR });
            list.Add(new GiftModel { Name = "Broek", Category = GiftCategory.WEAR });

            list.Add(new GiftModel { Name = "lego for dummies", Category = GiftCategory.READ });
            list.Add(new GiftModel { Name = "Knex for dummies", Category = GiftCategory.READ });
            list.Add(new GiftModel { Name = "Fort: op naar level 1000", Category = GiftCategory.READ });
            list.Add(new GiftModel { Name = "Hoe overleef ik de middelbare school!", Category = GiftCategory.READ });

            foreach (var gift in list)
            {
                if(chosenGiftNames.Contains(gift.Name))
                {
                    gift.IsChosen = true;
                }
            }
            var giftModelNames = list.Select(g => g.Name).ToList();
            foreach (var name in chosenGiftNames)
            {
               if (!giftModelNames.Contains(name))
               {
                    list.Add(new GiftModel { Name = name, Category = GiftCategory.READ, IsChosen = true });
               }
            }

            return list;
        }

        private List<Gift> CreateMockPossibleGifts()
        {
            var list = new List<Gift>();
            list.Add(new Gift { Name = "Lego", Category = GiftCategory.WANT });
            list.Add(new Gift { Name = "Duplo", Category = GiftCategory.WANT });
            list.Add(new Gift { Name = "Roblox", Category = GiftCategory.WANT });
            list.Add(new Gift { Name = "Pokemonkaartjes", Category = GiftCategory.WANT });

            list.Add(new Gift { Name = "Shampoo", Category = GiftCategory.NEED });
            list.Add(new Gift { Name = "Deo", Category = GiftCategory.NEED });
            list.Add(new Gift { Name = "Tandenborstel", Category = GiftCategory.NEED });
            list.Add(new Gift { Name = "Tandpasta", Category = GiftCategory.NEED });

            list.Add(new Gift { Name = "Schoenen", Category = GiftCategory.WEAR });
            list.Add(new Gift { Name = "Trui", Category = GiftCategory.WEAR });
            list.Add(new Gift { Name = "Sokken", Category = GiftCategory.WEAR });
            list.Add(new Gift { Name = "Broek", Category = GiftCategory.WEAR });

            list.Add(new Gift { Name = "lego for dummies", Category = GiftCategory.READ });
            list.Add(new Gift { Name = "Knex for dummies", Category = GiftCategory.READ });
            list.Add(new Gift { Name = "Fort: op naar level 1000", Category = GiftCategory.READ });
            list.Add(new Gift { Name = "Hoe overleef ik de middelbare school!", Category = GiftCategory.READ });

            return list;
        }      

        private WishlistModel SetupModel(UserModel user, List<Gift>? chosenGifts, string? extraGifts)
        {
            WishlistModel model = new();
            model.User = user;            
            model.ChosenGiftNames = chosenGifts.Select(g => g.Name).ToList();
            model.Gifts = CreatePossibleGiftsList(model.ChosenGiftNames);            
            model.ExtraGifts = extraGifts;
            return model;
        }
    }
}
