using tangti.Models;
using tangti.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using tangti.DTOs;
using System.Formats.Asn1;

namespace CommentController
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly CommentService _commentService;

        public CommentController(CommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Comment>>> Get()
        {
            var comments = await _commentService.GetCommentsAsync();

            Console.WriteLine("Get");

            return Ok(comments);
        }

        [HttpPost("create")]
        public async Task<ActionResult> Create([FromBody] CommentDto commentDto)
        {
            try
            {
                Console.WriteLine("Create");
                var newComment = new Comment{
                    EventId = commentDto.eventId,
                    CreatedBy = commentDto.userId,
                    Content = commentDto.content
                };

                await _commentService.CreateAsync(newComment);

                return Ok(newComment);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Create action: {ex}");
                return BadRequest();
            }
        }

        [HttpPost("delete")]
        public async Task<ActionResult> DeleteAsync([FromBody] DeleteCommentDto deleteCommentDto)
        {
            var comment = await _commentService.GetCommentAsync(deleteCommentDto.id);

            if (comment is null)
            {
                return BadRequest("Comment not Found");
            }
            
            await _commentService.DeleteAsync(deleteCommentDto.id);

            return Ok("Comment has been Deleted");
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update([FromBody] UpdateCommentDto updatedCommentDto)
        {
            var comment = await _commentService.GetCommentAsync(updatedCommentDto.id);

            if (comment is null)
            {
                return BadRequest("Comment not Found");
            }

            comment.Content = updatedCommentDto.content;

            await _commentService.UpdateAsync(updatedCommentDto.id, comment);

            return Ok(comment);
        }

        [HttpPost("eventcomment")]
        public async Task<ActionResult> GetEventComment([FromBody] EventCommentDto eventCommentDto)
        {
            var comments = await _commentService.GetEventCommentsAsync(eventCommentDto.eventId);

            if (comments is null)
            {
                return BadRequest("Comment not Found");
            }
            
            return Ok(comments);
        }
    }
}