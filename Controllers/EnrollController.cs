using tangti.Models;
using tangti.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace tangti.Controllers;

public class EnrollController : Controller
{
    private readonly EnrollService _enrollService;

    public EnrollController(EnrollService enrollService) =>
        _enrollService = enrollService;

    [HttpGet]
    public async Task<ActionResult<List<Enroll>>> Get()
    {
        return await  _enrollService.GetAsync();
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Enroll>> Get(string id)
    {
        var enroll = await _enrollService.GetAsync(id);

        if (enroll is null){
            return NotFound();
        }

        return enroll;
    }

    [HttpPost]
    public async Task<ActionResult> Create(Enroll enroll)
    {
        try
        {
            string message_response;
            if (ModelState.IsValid)
            {
                enroll.Id = ObjectId.GenerateNewId().ToString();
                await _enrollService.CreateAsync(enroll);
                message_response = "Enroll created Successfully";
                ViewBag.Message = message_response;

                //return some Data
                return View();
            }
            else
            {
                message_response = "invalid model state";
                ViewBag.Message = message_response;
                return View();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Create action: {ex}");
            return View();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, Enroll enrollIn)
    {
        var enroll = await _enrollService.GetAsync(id);

        if (enroll is null)
        {
            return NotFound();
        }

        await _enrollService.UpdateAsync(id, enrollIn);

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        var enroll = await _enrollService.GetAsync(id);

        if (enroll == null)
        {
            return NotFound();
        }

        await _enrollService.DeleteAsync(id);

        return View();
    }
}