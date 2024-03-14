using tangti.Models;
using tangti.Services;
using Microsoft.AspNetCore.Mvc;
using tangti.DTOs;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LikeController
{
    [Route("api/like")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly LikeService _likeService;
        private readonly UserService _userService;

        public LikeController(LikeService likeService, UserService userService)
        {
            _likeService = likeService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Like>>> Get()
        {
            var likes = await _likeService.GetLikesAsync();

            return Ok(likes);
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Like>> GetLike(string id)
        {
            var like = await _likeService.GetLikeAsync(id);
            return Ok(like);
        }

        [HttpPost("create")]
        public async Task<ActionResult> Create([FromBody] CreateLikeDto createLikeDto)
        {
            var newLike = new Like
            {
                Id = ObjectId.GenerateNewId().ToString(),
                EventId = createLikeDto.eventId
            };

            await _likeService.CreateAsync(newLike);

            return Ok(newLike);
        }

        [HttpPost("userlike")]
        public async Task<ActionResult> UserLike([FromBody] LikeDto likeDto)
        {
            var user = await _userService.GetUserAsync(likeDto.userId);

            if (user is null)
            {
                return BadRequest("User not Found");
            }

            var like = await _likeService.GetLikeEventAsync(likeDto.eventId);

            if (like is null)
            {
                return BadRequest("Like not Found");
            }

            if (user.Id is null)
            {
                return BadRequest("User Id is null");
            }

            if(like.Likes.Any(member => member == likeDto.userId))
            {
                return Ok(like);    
            }

            like.Likes.Add(user.Id);

            like.LikeAmount = like.Likes.Count;

            if (like.Id is null)
            {
                return BadRequest("like Id is Null");
            }

            user.Liked.Add(like.Id);

            await _likeService.UpdateAsync(like.Id, like);

            await _userService.UpdateUserAsync(user.Id, user);

            return Ok(like);
        }

        [HttpPost("dislike")]
        public async Task<ActionResult> Dislike([FromBody] LikeDto likeDto)
        {
            var user = await _userService.GetUserAsync(likeDto.userId);

            if (user is null)
            {
                return BadRequest("User not Found");
            }

            var like = await _likeService.GetLikeEventAsync(likeDto.eventId);

            if (like is null)
            {
                return BadRequest("Like not Found");
            }

            if (user.Id is null)
            {
                return BadRequest("User Id is null");
            }

            if(!like.Likes.Any(member => member == likeDto.userId))
            {
                return Ok(like);    
            }

            like.Likes.Remove(user.Id);

            like.LikeAmount = like.Likes.Count;

            if (like.Id is null)
            {
                return BadRequest("like Id is Null");
            }

            user.Liked.Remove(like.Id);

            await _likeService.UpdateAsync(like.Id, like);

            await _userService.UpdateUserAsync(user.Id, user);

            return Ok(like);
        }

        [HttpPost("check")]
        public async Task<ActionResult> Check([FromBody] LikeDto likeDto)
        {
            var like = await _likeService.GetLikeEventAsync(likeDto.eventId);

            if (like is null)
            {
                return BadRequest("Like is Null");
            }

            foreach (var member in like.Likes){
                if (member == likeDto.userId)
                {
                    return Ok(true);
                }
            }

            return Ok(false);
        }
    }
}