using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IMapper _mapper;
        private readonly ICountryRepository _countryRepository;

        public OwnerController(IOwnerRepository ownerRepository,ICountryRepository countryRepository, IMapper mapper)
        {
            _ownerRepository = ownerRepository;
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        public IActionResult GetOwners()
        {
            var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());
            return Ok(owners);
        }

        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
                return NotFound();

            var owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwner(ownerId));
            return Ok(owner);
        }

        [HttpGet("{ownerId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
                return NotFound();

            var pokemon = _mapper.Map<List<PokemonDto>>(_ownerRepository.GetPokemonByOwner(ownerId));
            return Ok(pokemon);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult CreateOwner([FromQuery] int countryId, [FromBody] OwnerDto createOwner ){
            if(createOwner == null){
                return BadRequest(ModelState);
            }

            var ownerExist = _ownerRepository.GetOwners().Where(owner=>owner.FirstName.ToUpper() == createOwner.FirstName.TrimEnd().ToUpper()).FirstOrDefault();
            if(ownerExist != null){
                ModelState.AddModelError("","Owner already exists");
                return StatusCode(422,ModelState);
            }

            var owner = _mapper.Map<Owner>(createOwner);
            owner.Country = _countryRepository.GetCountry(countryId);
            var success = _ownerRepository.CreateOwner(owner);

            if(!success){
                ModelState.AddModelError("","Error occured while create owner");
                return StatusCode(500,ModelState);
            }

            return Ok("Owner created Successfully");

        } 


        [HttpPut("{ownerId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateOwner(int ownerId, [FromBody] OwnerDto updateOwner){
            if(updateOwner == null){
                return BadRequest(ModelState);
            }

            if(ownerId != updateOwner.Id){
                return BadRequest(ModelState);
            }

            var owner = _mapper.Map<Owner>(updateOwner);
            var success = _ownerRepository.UpdateOwner(owner);

            if(!success){
                ModelState.AddModelError("","Error occured while updating owner");
                return StatusCode(500,ModelState);
            }

            return Ok("Update Successfuly");
        }
        
    }
        
}

