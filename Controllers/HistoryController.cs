using tangti.Models;
using tangti.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using tangti.DTOs;
using System.Formats.Asn1;

namespace HistoryController
{
    
    public class HistoryController : Controller
    {
        private readonly HistoryService _historyService;

        public HistoryController(HistoryService historyService)
        {
            _historyService = historyService;
        }

        public IActionResult Index()
        {
            var historys = _historyService.GetAsync().Result;
            return View(historys);
        }


        public IActionResult Details(string id)
        {
            return View(id);
        }
        
        [HttpGet]
        public async Task<ActionResult<List<History>>> Get()
        {
            return await _historyService.GetAsync();
        }

        // [Route("history")]
        // //input userId and search where equal userId
        // [HttpGet("user/{userId}")]
        // public async Task<ActionResult<List<History>>> GetByUserId(string userId)
        // {
        //     var historys = await _historyService.GetByUserIdAsync(userId);

        //     if (historys is null)
        //     {
        //         return BadRequest("History not Found");
        //     }

        //     return Ok(historys);
        // }


}
}