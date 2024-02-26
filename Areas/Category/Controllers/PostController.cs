using App.ExtendMethods;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using App.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;
using App.Utilities;

[Area("Category")]
[Route("{controller}/{action=Index}")]
public class PostController : Controller {
    private readonly AppMvcContext appMvcContext;
    private readonly UserManager<AppUser> userManager;

    public PostController(AppMvcContext appMvcContext, UserManager<AppUser> userManager) {
        this.appMvcContext = appMvcContext;
        this.userManager = userManager;
    }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> Index([FromQuery(Name = "p")] int currentPage, int pageSize) {
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        var posts = await appMvcContext.Posts
                            .Include(p => p.Author)
                            .Include(p => p.PostCategories)
                            .ThenInclude(pc => pc.Category)
                            .ToListAsync();
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

        if (pageSize == 0) pageSize = 10;
        if(currentPage <= 0) currentPage = 1;

        var totalPage = posts.Count;
        var countPage = (int) Math.Ceiling((decimal) totalPage / pageSize);

        if(currentPage > countPage) currentPage = countPage;

#nullable disable
        var pageModel = new PagingModel() {
            countpages = countPage,
            currentpage = currentPage,
            generateUrl = (page) => Url.Action("Index", new { p = page })
        };
#nullable enable 

        ViewBag.pageModel = pageModel;
        ViewBag.totalPage = totalPage;
        var postPage = posts.Skip((currentPage - 1) * pageSize).Take(pageSize);

        return View(postPage);
    }

    [HttpGet]
    public async Task<IActionResult> Create() {
        var categories = await appMvcContext.categories.ToListAsync();
        var items = new SelectList(categories, "Id", "Slug");

        ViewBag.items = items;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind("Title,Description,Content,Published,CategoryId")] CreatePostModel post) {
        if(ModelState.IsValid) {
            var userCreate = await userManager.GetUserAsync(User);
            var category = await appMvcContext.categories.FindAsync(post.CategoryId);
            if(category == null) {
                return NotFound();
            }

            post.Author = userCreate;
            post.DateCreated = DateTime.Now;
#nullable disable
            post.Slug = AppUtilities.GenerateSlug(post.Title);
#nullable enable

            var postCategory = new PostCategory() {
                Post = post,
                Category = category
            };

            await appMvcContext.PostCategories.AddAsync(postCategory);
            await appMvcContext.Posts.AddAsync(post);
            await appMvcContext.SaveChangesAsync();
            StatusMessage = "Thêm thành công";
            return RedirectToAction(nameof(Index));
        }
        StatusMessage = "Không thể thêm";
        return View(post);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Delete(int id) {
        var post = await appMvcContext.Posts.FindAsync(id);
        return View(post);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> DeleteAsync(int id) {
        var post = await appMvcContext.Posts.FindAsync(id);
        if(post == null) {
            StatusMessage = "Không thể xóa";
            return RedirectToAction(nameof(Index));
        }
        try
        {
            appMvcContext.Posts.Remove(post);
        }
        catch (System.Exception e)
        {
            ModelState.AddModelError(e.Message);
            throw;
        }
        await appMvcContext.SaveChangesAsync();
        StatusMessage = "Xóa thành công";
        return RedirectToAction(nameof(Index));
    }
}