
namespace PokemonReviewApp.Models
{
    public class PockemonCategory
    {
        public int PockemonId { get; set; }

        public int CtegoryId { get; set; }

        public Pokemon Pokemon { get; set; }

        public Category Category { get; set; }

    }
}
