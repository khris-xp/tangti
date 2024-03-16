using tangti.Models;
using tangti.Services;
using Microsoft.AspNetCore.Mvc;
using tangti.DTOs;
using MongoDB.Bson;

namespace RatingController
{
    [Route("api/rating")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly RatingService _ratingService;

        public RatingController(RatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Rating>>> Get()
        {
            var ratings = await _ratingService.GetRatingsAsync();
            return Ok(ratings);
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Rating>> GetRating(string id)
        {
            var rating = await _ratingService.GetRatingAsync(id);
            return Ok(rating);
        }

        [HttpPost("create")]
        public async Task<ActionResult> Create([FromBody] CreateRatingDto createRatingDto)
        {
            if (createRatingDto.userId is null)
            {
                return BadRequest("User id is null");
            }

            if (createRatingDto.eventId is null)
            {
                return BadRequest("Event id is null");
            }

            var newRating = new Rating
            {
                Id = ObjectId.GenerateNewId().ToString(),
                EventId = createRatingDto.eventId,
                UserId = createRatingDto.userId,
                RatingScore = createRatingDto.ratingScore
            };

            await _ratingService.CreateAsync(newRating);

            return Ok(newRating);
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update([FromBody] UpdateRatingDto updateRatingDto)
        {
            var rating = await _ratingService.GetRatingAsync(updateRatingDto.id);

            if (rating is null)
            {
                return BadRequest("Rating is Null");
            }

			if (rating.Id != null)
			{
            	rating.RatingScore = updateRatingDto.ratingScore;
            	await _ratingService.UpdateAsync(rating.Id, rating);
			}
            return Ok(rating);
        }

        [HttpPost("delete")]
        public async Task<ActionResult> Delete([FromBody] DeleteRatingDto deleteRatingDto)
        {
            var rating = await _ratingService.GetRatingAsync(deleteRatingDto.id);

            if (rating is null)
            {
                return BadRequest("Rating is null");
            }
			if (rating.Id != null)
            	await _ratingService.DeleteAsync(rating.Id);

            return Ok(rating);
        }
    }
}