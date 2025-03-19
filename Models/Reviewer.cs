using System.Collections;

namespace PokemonReviewApp.Models
{

    public class Reviewer
    {

        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ICollection<Review> Reviews { get; set; }



    }