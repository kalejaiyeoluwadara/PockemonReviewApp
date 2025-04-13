using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using AutoMapper;
using PokemonReviewApp.Dto;

namespace PokemonReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewController(IReviewRepository reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        public IActionResult GetReviews()
        {
            var reviews = _reviewRepository.GetReviews();
            var reviewsDto = _mapper.Map<List<ReviewDto>>(reviews);
            return Ok(reviewsDto);
        }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();
            var review = _reviewRepository.GetReview(reviewId);
            var reviewDto = _mapper.Map<ReviewDto>(review);
            return Ok(reviewDto);
        }

        [HttpGet("{reviewId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();
            var reviews = _reviewRepository.GetReviewsOfAPokemon(reviewId);

            if(!ModelState.IsValid)
                return BadRequest();
            return Ok(reviews);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult CreateReview([FromQuery] int pokemonId, [FromQuery] int reviewerId, [FromBody] ReviewDto createReview){
            if(createReview == null){
                return BadRequest(ModelState);
            }

            var reviewExist = _reviewRepository.GetReviews().Where(reviews => reviews.Title.ToUpper() == createReview.Title.TrimEnd().ToUpper()).FirstOrDefault();

            if(reviewExist != null){
                ModelState.AddModelError("","Review already exists");
                return StatusCode(422,ModelState);
            }

            var review = _mapper.Map<Review>(createReview);
            var success = _reviewRepository.CreateReview(review);

            if(!success){
                ModelState.AddModelError("","Error occured while creating review");
                return StatusCode(500,ModelState);
            }

            return Ok("Review created Successfully");


        }
    }
}

        
        
 