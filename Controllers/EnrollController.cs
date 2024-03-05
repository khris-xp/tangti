using tangti.Models;
using tangti.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace tangti.Controllers;

public class EnrollController : Controller
{
    private readonly EnrollService _enrollService;
    private readonly UserService _userService;

    public EnrollController(EnrollService enrollService, UserService userService){
        _enrollService = enrollService;
        _userService = userService;
    }

    public IActionResult Index()
    {
        var enrolls = _enrollService.GetAsync().Result;
        return View(enrolls);
    }

    public IActionResult Details(string id)
    {
        var enrolls = _enrollService.GetAsync(id).Result;
        return View(enrolls);
    }

    public ActionResult Create()
    {
        return View();
    }

    public ActionResult Edit(string id)
    {
        var enrolls = _enrollService.GetAsync(id).Result;
        return View(enrolls);
    }

    public ActionResult Delete(string id)
    {
        var enrolls = _enrollService.GetAsync(id).Result;
        return View(enrolls);
    }

    public ActionResult Update(string evnetid)
    {
        var enrolls = _enrollService.GetEventEnrollAsync(evnetid).Result;
        return View(enrolls);
    }

    public ActionResult Unenroll(string eventid)
    {
        var enrolls = _enrollService.GetEventEnrollAsync(eventid).Result;
        return View(enrolls);
    }

    [HttpGet]
    public async Task<ActionResult<List<Enroll>>> Get()
    {
        return await _enrollService.GetAsync();
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Enroll>> Get(string id)
    {
        var _enroll = await _enrollService.GetAsync(id);

        if (_enroll is null)
        {
            return NotFound();
        }

        return _enroll;
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
                return RedirectToAction("Index");
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

        return RedirectToAction("Index");
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

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Update(string id, string userId)
    {   

        var enroll = await _enrollService.GetAsync(id);


        if (enroll is null)
        {
            return NotFound();
        }

        if (enroll.MemberList.Any(member => member.UserID == userId))
        {
            // Can not join this Event because already joined
            return RedirectToAction("Index");
        }

        var user = await _userService.GetUserAsync(userId);

        if (user is null)
        {
            return NotFound();
        }

        user.Enrolled.Add(enroll.EventID);

        enroll.MemberList.Add(
            new Enroll.JoinUserData(userId)
        );

        enroll.Member = enroll.MemberList.Count;

        await _enrollService.UpdateAsync(id, enroll);

        await _userService.UpdateUserAsync(userId, user);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Unenroll(string id, string userId)
    {
        var enroll = await _enrollService.GetAsync(id);

        if (enroll is null)
        {
            return NotFound();
        }

        if (!enroll.MemberList.Any(member => member.UserID == userId))
        {
            // Can not exit group this Event because not in this Event
            return RedirectToAction("Index");
        }

        var user = await _userService.GetUserAsync(userId);

        if(user is null){
            return NotFound();
        }

        user.Enrolled.Remove(enroll.EventID);

        Enroll.JoinUserData? target = enroll.MemberList.FirstOrDefault(member => member.UserID == userId);

        if (target != null)
        {
            enroll.MemberList.Remove(target);
        }

        enroll.Member = enroll.MemberList.Count; ;

        await _enrollService.UpdateAsync(id, enroll);

        await _userService.UpdateUserAsync(userId, user);

        return RedirectToAction("Index");
    }
}