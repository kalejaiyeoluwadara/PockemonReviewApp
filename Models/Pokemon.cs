namespace PokemonReviewApp.Models
{
    public class Pokemon
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        public ICollection<Review> Reviews { get; set; }
    }
}