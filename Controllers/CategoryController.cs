using tangti.Models;
using tangti.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace tangti.Controllers;

public class CategoryController : Controller
{
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService) =>
        _categoryService = categoryService;

    public IActionResult Index()
    {
        var categories = _categoryService.GetAsync().Result;
        return View(categories);
    }

    public IActionResult Details(string id)
    {
        var category = _categoryService.GetAsync(id).Result;
        return View(category);
    }
    public ActionResult Create()
    {
        return View();
    }

    public ActionResult Edit(string id)
    {
        var category = _categoryService.GetAsync(id).Result;
        return View(category);
    }

    public ActionResult Delete(string id)
    {
        var category = _categoryService.GetAsync(id).Result;
        return View(category);
    }

    [HttpGet]
    public async Task<ActionResult<List<Category>>> Get()
    {
        return await _categoryService.GetAsync();
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Category>> Get(string id)
    {
        var category = await _categoryService.GetAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        return category;
    }
    [HttpPost]
    public async Task<ActionResult> Create(Category category)
    {
        try
        {
            string message_response;
            if (ModelState.IsValid)
            {
                category.Id = ObjectId.GenerateNewId().ToString();
                await _categoryService.CreateAsync(category);
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
    public async Task<IActionResult> Edit(string id, Category categoryIn)
    {
        var category = await _categoryService.GetAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        await _categoryService.UpdateAsync(id, categoryIn);

        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        var blog = await _categoryService.GetAsync(id);

        if (blog == null)
        {
            return NotFound();
        }

        await _categoryService.DeleteAsync(id);

        return RedirectToAction("Index");
    }
}