using Microsoft.AspNetCore.Mvc;
using tangti.Models;
using tangti.Services;
using MongoDB.Bson;

namespace tangti.Controllers;

public class ReportController : Controller
{
    private readonly ReportService _reportService;

    public ReportController(ReportService reportService)
    {
        _reportService = reportService;
    }

    public IActionResult Index()
    {
        var report = _reportService.GetReportsAsync().Result;
        return View(report);
    }

    public IActionResult Details(string id)
    {
        var report = _reportService.GetReportAsync(id).Result;
        return View(report);
    }

    public ActionResult Create()
    {
        return View();
    }

    public ActionResult Edit(string id)
    {
        var report = _reportService.GetReportAsync(id).Result;
        return View(report);
    }

    public ActionResult Delete(string id)
    {
        var report = _reportService.GetReportAsync(id).Result;
        return View(report);
    }

    [HttpGet]
    public async Task<ActionResult<List<Report>>> Get()
    {
        return await _reportService.GetReportsAsync();
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Report>> Get(string id)
    {
        var _report = await _reportService.GetReportAsync(id);

        if (_report is null)
        {
            return NotFound();
        }

        return _report;
    }

    [HttpPost]
    public async Task<ActionResult> Create(Report report)
    {
        try
        {
            string message_response;
            if (ModelState.IsValid)
            {
                if (report.Createdby is null)
                {
                    message_response = "User id is null";
                    ViewBag.Message = message_response;
                    return View();
                }

                report.Id = ObjectId.GenerateNewId().ToString();
                await _reportService.CreateAsync(report);
                message_response = "Report created Successfully";
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
    public async Task<IActionResult> Edit(string id, Report reportIn)
    {
        var report = await _reportService.GetReportAsync(id);

        if (report is null)
        {
            return NotFound();
        }

        await _reportService.UpdateAsync(id, reportIn);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        var report = await _reportService.GetReportAsync(id);

        if (report == null)
        {
            return NotFound();
        }

        await _reportService.DeleteAsync(id);

        return RedirectToAction("Index");
    }
}