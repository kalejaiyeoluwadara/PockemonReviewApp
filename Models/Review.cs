namespace PokemonReviewApp.Models
{

    public class Review
    {

        public string Id { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public Reviewer Reviewer { get; set; }



    }