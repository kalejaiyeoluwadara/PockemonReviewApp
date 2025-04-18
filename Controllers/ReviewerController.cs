using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewerController : Controller
    {
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewerController(IReviewerRepository reviewerRepository,IReviewRepository reviewRepository, IMapper mapper)
        {
            _reviewerRepository = reviewerRepository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
        public IActionResult GetReviewers()
        {
            var reviewers = _reviewerRepository.GetReviewers();
            var reviewersDto = _mapper.Map<List<ReviewerDto>>(reviewers);
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            return Ok(reviewersDto);
        }

        [HttpGet("{reviewerId}")]
        [ProducesResponseType(200, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();
            var reviewer = _reviewerRepository.GetReviewer(reviewerId);
            var reviewerDto = _mapper.Map<ReviewerDto>(reviewer);

            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            return Ok(reviewerDto);
        }

        [HttpGet("{reviewerId}/reviews")]
        public IActionResult GetReviewsByAReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();
            var reviews = _mapper.Map<List<ReviewDto>>(
                _reviewerRepository.GetReviewsByAReviewer(reviewerId));

            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            return Ok(reviews);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult CreateReviewer([FromBody] ReviewerDto createReviewer){
            if(createReviewer == null){
                return BadRequest(ModelState);
            }

            var reviewerExist = _reviewerRepository.GetReviewers().Where(reviewer => reviewer.FirstName.ToUpper() == createReviewer.FirstName.TrimEnd().ToUpper()).FirstOrDefault();

            if(reviewerExist != null){
                ModelState.AddModelError("","Reviewer already exists");
                return StatusCode(422,ModelState);
            }

            var reviewer = _mapper.Map<Reviewer>(createReviewer);
            var success = _reviewerRepository.CreateReviewer(reviewer);

            if(!success){
                ModelState.AddModelError("","Error occured while creating reviewer");
                return StatusCode(500,ModelState);
            }

            return Ok("Reviewer created Successfully");
        }


        [HttpPut("{reviewerId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReviewer(int reviewerId, [FromBody] ReviewerDto updateReviewer){
            if(updateReviewer == null){
                return BadRequest(ModelState);
            }

            if(reviewerId != updateReviewer.Id){
                return BadRequest(ModelState);
            }

            if(!_reviewerRepository.ReviewerExists(reviewerId)){
                return NotFound();
            }

            var review = _mapper.Map<Reviewer>(updateReviewer);
            var success = _reviewerRepository.UpdateReviewer(review);

            if(!success){
                ModelState.AddModelError("","Error occured while updating review");
                return StatusCode(500,ModelState);
            }

            return Ok("Reviewer updated Successfully");
    }

        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReviewer(int reviewerId){
            if(!_reviewerRepository.ReviewerExists(reviewerId)){
                return NotFound();
            }

            var reviewer = _reviewerRepository.GetReviewer(reviewerId);
            var reviews = _reviewerRepository.GetReviewsByAReviewer(reviewerId);

            if(reviews.Count > 0){
                ModelState.AddModelError("","Reviewer cannot be deleted, as they have reviews");
                return StatusCode(422,ModelState);
            }

            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }

            if(!_reviewerRepository.DeleteReviewer(reviewer)){
                ModelState.AddModelError("","Error occured while deleting reviewer");
                return StatusCode(500,ModelState);
            }

            return Ok("Reviewer deleted Successfully");

        }
    }
}