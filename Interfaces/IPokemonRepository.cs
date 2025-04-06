using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IPokemonRepository
    {
        ICollection<Pokemon> GetPokemons();
        Pokemon GetPokemon(int Id);
        Pokemon GetPokemon(string name);
        decimal GetpokemonRating(int pokeId);
        bool PokemonExists(int PokeId);
    }
}
