using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CategoryViewComponent : ViewComponent {
    private readonly AppMvcContext appMvcContext;

    public CategoryViewComponent(AppMvcContext appMvcContext) {
        this.appMvcContext = appMvcContext;
    }

    public IViewComponentResult Invoke(dynamic model) {
        var categories = appMvcContext.categories.Include(c => c.CategoryChildren)
                                                .AsEnumerable()
                                                .Where(c => c.ParentCategory == null)
                                                .ToList();
        ViewBag.ShowCategory = model;
        return View(categories);
    }
}