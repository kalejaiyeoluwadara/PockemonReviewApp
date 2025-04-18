using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Data;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : Controller
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        public PokemonController(IPokemonRepository pokemonRepository,IReviewRepository reviewRepository, DataContext context, IMapper mapper)
        {
            this._pokemonRepository = pokemonRepository;
            this._reviewRepository = reviewRepository;
            this._mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]

        public IActionResult GetPokemons() {
            var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(pokemons);
        }


        [HttpGet("{pokeId}")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }

            var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokeId));
            return Ok(pokemon);
        }

      

        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }

            var rating = _pokemonRepository.GetPokemonRating(pokeId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(rating);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult CreatePokemon([FromQuery] int ownerId,[FromQuery] int catId, [FromBody] PokemonDto pokemonCreate ){
            if(pokemonCreate == null){
                return BadRequest(ModelState);
            }

            var pokemonExist = _pokemonRepository.GetPokemons().Where(pokemon=>pokemon.Name.ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper()).FirstOrDefault();
            if(pokemonExist != null){
                ModelState.AddModelError("","Pokemon already exists");
                return StatusCode(422,ModelState);
            }

            var pokemon = _mapper.Map<Pokemon>(pokemonCreate);
            
            var success = _pokemonRepository.CreatePokemon(ownerId,catId,pokemon);

            if(!success){
                ModelState.AddModelError("","Error occured while create owner");
                return StatusCode(500,ModelState);
            }

            return Ok("Owner created Successfully");

        } 

        [HttpPut("{pokeId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePokemon( int pokeId, [FromQuery] int catId, [FromQuery] int ownerId,  [FromBody] PokemonDto updatePokemon){
            if(updatePokemon == null){
                return BadRequest(ModelState);
            }

            if(pokeId != updatePokemon.Id){
                return BadRequest(ModelState);
            }

            var pokemon = _mapper.Map<Pokemon>(updatePokemon);

            var success = _pokemonRepository.UpdatePokemon(ownerId,catId,pokemon);

            if(!success){
                ModelState.AddModelError("","Error occured while updating owner");
                return StatusCode(500,ModelState);
            }

            return Ok("Owner updated Successfully");
    }

            
            [HttpDelete("{pokeId}")]
            [ProducesResponseType(200)]
            [ProducesResponseType(400)]
            [ProducesResponseType(404)]
            public IActionResult DeletePokemon(int pokeId){
                if(!_pokemonRepository.PokemonExists(pokeId)){
                    return NotFound();
                }
    
                var reviewsToDelete = _reviewRepository.GetReviewsOfAPokemon(pokeId).ToList();

                var pokemon = _pokemonRepository.GetPokemon(pokeId);

                if(!ModelState.IsValid){
                    return BadRequest(ModelState);
                }

                if(!_reviewRepository.DeleteReviews(reviewsToDelete)){
                    ModelState.AddModelError("","Error occured while deleting reviews");
                    return StatusCode(500,ModelState);
                }

                if(!_pokemonRepository.DeletePokemon(pokemon)){
                    ModelState.AddModelError("","Error occured while deleting pokemon");
                    return StatusCode(500,ModelState);
                }
    
                return Ok("Successfully Deleted Pokemon");
            }
    }

    
}
