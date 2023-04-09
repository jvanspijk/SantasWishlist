using Microsoft.IdentityModel.Tokens;
using SantasWishlist.Context;
using System.ComponentModel.DataAnnotations;


namespace SantasWishlistWeb.Viewmodels
{
    public class UserModel : IValidatableObject
    {
        public string Id { get; set; }
        /// <summary>
        /// User name of the child account
        /// </summary>
        [Required(ErrorMessage = "Je naam is onbekend.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vul een leeftijd in")]
        [Range(4, 110,
        ErrorMessage = "Je moet tussen de {1} en {2} jaar oud zijn.")]
        public int Age { get; set; }
        /// <summary>
        /// If they've been good or bad
        /// </summary>
        [Required(ErrorMessage = "De kerstman wil weten of je braaf bent geweest.")]
        public bool WasGood { get; set; }
        /// <summary>
        /// A description detailing why they were good, only applicable when Good == True
        /// </summary>
        [RegularExpression(@"^\s*$|^.*[a-zA-Z].*",
         ErrorMessage = "Vul hier tekst in.")]
        public string? GoodDescription { get; set; }
        /// <summary>
        /// If they lied about being good
        /// </summary>
        public bool Lied { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(WasGood && GoodDescription.IsNullOrEmpty())
            {
                yield return new ValidationResult("Vul iets in om aan te geven waarom je braaf bent geweest.");
            }
            if (!WasGood && !GoodDescription.IsNullOrEmpty())
            {
                yield return new ValidationResult("Vul hier alleen iets in als je braaf bent geweest.");
            }
        }
    }
}
