using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Area("Category")]
[Route("{controller}/{action=Index}")]
public class CategoryController : Controller
{
    private readonly AppMvcContext appMvcContext;

    public CategoryController(AppMvcContext appMvcContext) {
        this.appMvcContext = appMvcContext;
    }

    public async Task<IActionResult> Index()
    {
        var qr = (from c in appMvcContext.categories select c)
                    .Include(c => c.ParentCategory)
                    .Include(c => c.CategoryChildren);
        var categories = (await qr.ToListAsync()).Where(c => c.ParentCategory == null);
        return View(model: categories);
    }

    [HttpGet]
    public async Task<IActionResult> Create() {
        var categories = await (from c in appMvcContext.categories select c).ToListAsync();
        var items = new SelectList(categories, dataTextField: "Slug", dataValueField: "Id");
        ViewData["items"] = items;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind("Title", "Slug", "Content", "ParentCategoryId")] Category category) {
        if(ModelState.IsValid) {
            await appMvcContext.categories.AddAsync(category);
            await appMvcContext.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{id?}")]
    public IActionResult Delete(int id) {
        var category = appMvcContext.categories.Include(c => c.CategoryChildren).FirstOrDefault(c => c.Id == id);
        if(category == null) {
            return NotFound();
        }
        appMvcContext.categories.Remove(category);
        appMvcContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}