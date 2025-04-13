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
        private readonly IMapper _mapper;
        public PokemonController(IPokemonRepository pokemonRepository, DataContext context, IMapper mapper)
        {
            this._pokemonRepository = pokemonRepository;
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

    }

    
}
