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
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewerRepository _reviewerRepository;

        public ReviewController(IReviewRepository reviewRepository, IMapper mapper, IPokemonRepository pokemonRepository, IReviewerRepository reviewerRepository)
        {
            _reviewRepository = reviewRepository;
            _pokemonRepository = pokemonRepository;
            _reviewerRepository = reviewerRepository;
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
            review.Pokemon = _pokemonRepository.GetPokemon(pokemonId);
            review.Reviewer = _reviewerRepository.GetReviewer(reviewerId);
            var success = _reviewRepository.CreateReview(review);

            if(!success){
                ModelState.AddModelError("","Error occured while creating review");
                return StatusCode(500,ModelState);
            }

            return Ok("Review created Successfully");

        }
        
        [HttpPut("{reviewId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto updateReview){
            if(updateReview == null){
                return BadRequest(ModelState);
            }

            if(reviewId != updateReview.Id){
                return BadRequest(ModelState);
            }

            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();

            var review = _mapper.Map<Review>(updateReview);
            var success = _reviewRepository.UpdateReview(review);

            if(!success){
                ModelState.AddModelError("","Error occured while updating review");
                return StatusCode(500,ModelState);
            }

            return Ok("Review updated Successfully");
        }
       
}}

        
        
 