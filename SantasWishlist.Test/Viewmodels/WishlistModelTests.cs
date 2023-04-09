using Moq;
using SantasWishlist.Domain;
using SantasWishlistWeb.Viewmodels;

namespace SantasWishlist.Test.Viewmodels
{
    [TestClass()]
    public class WishlistModelTests
    {
        private WishlistModel _testModel;
        private List<string> _expectedExtraGifts;
        private List<Gift> _testGifts;
        private Mock<IGiftRepository> _mockGiftRepo;

        [TestInitialize]
        public void Setup()
        {
            _testModel = new WishlistModel();
            _testGifts = new List<Gift>()
            {
                new Gift()
                {
                    Name = "Fiets",
                    Category = GiftCategory.NEED
                },
                new Gift()
                {
                    Name="Nintendo",
                    Category=GiftCategory.WANT
                },
                new Gift()
                {
                    Name = "Shirt",
                    Category = GiftCategory.WEAR
                },
                new Gift()
                {
                    Name = "Sokken",
                    Category = GiftCategory.WEAR
                }
            };

            _mockGiftRepo = new Mock<IGiftRepository>();
            _mockGiftRepo.Setup(r => r.GetPossibleGifts()).Returns(_testGifts);
            _testModel.Gifts = _mockGiftRepo.Object.GetPossibleGifts().Select(g => new GiftModel
            {
                Name = g.Name,
                IsChosen = false,
                Category = g.Category
            }).ToList();

            _expectedExtraGifts = new List<string>()
            {
                "Schoenen",
                "Racestuur",
                "Hot Wheels",
                "Electronic device and circuit theory 11th edition By Robert L. Boylestad"
            };           
        }


        [TestMethod()]
        public void GetExtraGiftsList_CommaSeperatedInputReturnsListOfGifts_ReturnsTrue()
        {
            _testModel.ExtraGifts = "Schoenen, Racestuur, Hot Wheels, Electronic device and circuit theory 11th edition By Robert L. Boylestad";

            bool giftsListsAreEqual = _testModel.GetExtraGiftsList().SequenceEqual(_expectedExtraGifts);

            Assert.IsTrue(giftsListsAreEqual);
        }

        [TestMethod()]
        public void GetExtraGiftsList_SpaceSeperatedInputReturnsListOfGifts_ReturnsFalse()
        {
            _testModel.ExtraGifts = "Schoenen Racestuur Hot Wheels Electronic device and circuit theory 11th edition By Robert L. Boylestad";

            bool giftsListsAreEqual = _testModel.GetExtraGiftsList().SequenceEqual(_expectedExtraGifts);

            Assert.IsFalse(giftsListsAreEqual);
        }       

        [TestMethod()]
        public void GetWantGiftsTest()
        {
            List<GiftModel> expectedWantGifts = GetGiftsFromCategory(GiftCategory.WANT);
            List<GiftModel> wantGifts = _testModel.GetWantGifts();

            bool wantGiftsAreEqual = wantGifts.SequenceEqual(expectedWantGifts);

            Assert.IsTrue(wantGiftsAreEqual);
        }

        [TestMethod()]
        public void GetNeedGiftsTest()
        {
            List<GiftModel> expectedNeedGifts = GetGiftsFromCategory(GiftCategory.NEED) ;
            List<GiftModel> needGifts = _testModel.GetNeedGifts();

            bool needGiftsAreEqual = needGifts.SequenceEqual(expectedNeedGifts);

            Assert.IsTrue(needGiftsAreEqual);
        }

        [TestMethod()]
        public void GetWearGiftsTest()
        {
            List<GiftModel> expectedWearGifts = GetGiftsFromCategory(GiftCategory.WEAR);
            List<GiftModel> wearGifts = _testModel.GetWearGifts();

            bool wearGiftsAreEqual = wearGifts.SequenceEqual(expectedWearGifts);

            Assert.IsTrue(wearGiftsAreEqual);
        }

        [TestMethod()]
        public void GetReadGiftsTest()
        {
            List<GiftModel> expectedReadGifts = GetGiftsFromCategory(GiftCategory.READ);
            List<GiftModel> readGifts = _testModel.GetReadGifts();

            bool readGiftsAreEqual = readGifts.SequenceEqual(expectedReadGifts);

            Assert.IsTrue(readGiftsAreEqual);
        }

        private List<GiftModel> GetGiftsFromCategory(GiftCategory category)
        {
            return _mockGiftRepo.Object.GetPossibleGifts().Where(g => g.Category == category).Select(g => new GiftModel
            {
                Name = g.Name,
                IsChosen = false,
                Category = g.Category
            }).ToList();
        }
    }
}