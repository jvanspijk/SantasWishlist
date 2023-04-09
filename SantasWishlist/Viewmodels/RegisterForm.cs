using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace SantasWishlistWeb.Viewmodels
{
    public class RegisterForm : IValidatableObject
    {
        /// <summary>
        /// The entire string that goes into the input field,
        /// e.g. "Bob,Bas,Jan,Kees"
        /// </summary>
        [RegularExpression(@"^\s*$|^.*[a-zA-Z].*",
         ErrorMessage = "Vul hier tekst in.")]
        public string NamesInput { get; set; }

        /// <summary>
        /// The shared password for all the accounts in Names
        /// </summary>
        [Required(ErrorMessage = "Vul een wachtwoord in.")]
        [RegularExpression(@"^[a-zA-z]+$",
        ErrorMessage = "Maak alleen gebruik van letters")]
        public string Password { get; set; }

        /// <summary>
        /// True if the children behaved
        /// </summary>
        public bool WereGood { get; set; }
        /// <summary>
        /// The list of all names seperately,
        /// e.g. "Bob", "Bas", "Jan", "Kees"
        /// </summary>
        public List<string> GetNamesList()
        {   
            var namesList = new List<string>();
            if(NamesInput== null)
            {
                return namesList;
            }

            if(NamesInput.Contains(','))
            {
                var splitNames = NamesInput.Split(',');
                foreach (string name in splitNames)
                {
                    namesList.Add(name.Trim());
                }
                return namesList;
            }
            else if(NamesInput.Contains(' '))
            {
                string[] names = NamesInput.Split(' ');
                for(int i = 0; i < names.Length; i++)
                {
                    names[i] = names[i].Trim();
                }
                return names.ToList();
            }

            return namesList;            
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var names = GetNamesList();
            if(names != null)
            {
                bool doubleValues = names.Distinct().Count() != names.Count();
                if (doubleValues)
                {
                    yield return new ValidationResult("Je hebt dezelfde persoon meerdere keren in de lijst staan.");
                }
            }                     
        }
    }
}
