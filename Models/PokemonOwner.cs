namespace PokemonReviewApp.Models
{
    public class PokemonOwner
    {
        public int PokemonId { get; set; }

        public int OwnernerId { get; set; }
        public Pokemon Pokemon { get; set; }

        public Owner Owner { get; set; }
    }
}

