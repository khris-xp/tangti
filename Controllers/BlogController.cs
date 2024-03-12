using tangti.Models;
using tangti.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace tangti.Controllers;

public class BlogController : Controller
{
    private readonly BlogService _blogsService;

    public BlogController(BlogService blogsService) =>
        _blogsService = blogsService;

    public IActionResult Index()
    {
        var blogs = _blogsService.GetAsync().Result;
        return View(blogs);
    }

    public IActionResult Details(string id)
    {
        var blog = _blogsService.GetAsync(id).Result;
        return View(blog);
    }
    public ActionResult Create()
    {
        return View();
    }

    public ActionResult Edit(string id)
    {
        var blog = _blogsService.GetAsync(id).Result;
        return View(blog);
    }

    public ActionResult Delete(string id)
    {
        var blog = _blogsService.GetAsync(id).Result;
        return View(blog);
    }

    [HttpGet]
    public async Task<ActionResult<List<Blog>>> Get()
    {
        return await _blogsService.GetAsync();
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Blog>> Get(string id)
    {
        var blog = await _blogsService.GetAsync(id);

        if (blog is null)
        {
            return NotFound();
        }

        return blog;
    }
    [HttpPost]
    public async Task<ActionResult> Create(Blog blog)
    {
        try
        {
            string message_response;
            if (ModelState.IsValid)
            {
                if(blog.CreatedBy is null)
                {
                    message_response = "User id is null";
                    ViewBag.Message = message_response;
                    return View();
                }

                blog.Id = ObjectId.GenerateNewId().ToString();
                await _blogsService.CreateAsync(blog);
                message_response = "Blog created successfully";
                ViewBag.Message = message_response;
                return RedirectToAction("Index");
            }
            else
            {
                message_response = "Invalid model state";
                ViewBag.Message = message_response;
                return View();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Create action: {ex}");
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, Blog blogIn)
    {
        var blog = await _blogsService.GetAsync(id);

        if (blog is null)
        {
            return NotFound();
        }

        await _blogsService.UpdateAsync(id, blogIn);

        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        var blog = await _blogsService.GetAsync(id);

        if (blog == null)
        {
            return NotFound();
        }

        await _blogsService.DeleteAsync(id);

        return RedirectToAction("Index");
    }
}