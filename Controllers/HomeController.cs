using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AspMvc.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;
using Microsoft.AspNetCore.Identity;
using Bogus;

namespace AspMvc.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppMvcContext appMvcContext;
    private readonly UserManager<AppUser> userManager;
    public HomeController(ILogger<HomeController> logger, AppMvcContext appMvcContext, UserManager<AppUser> userManager)
    {
        _logger = logger;
        this.appMvcContext = appMvcContext;
        this.userManager = userManager;
        Randomizer.Seed = new Random(8675309);
    }

    [TempData]
    public string StatusMessage {set; get;} = default!;

    [Route("{slug?}")]
    public async Task<IActionResult> Index(string slug) {
        if(slug == null){
            var PostCategories = await appMvcContext.PostCategories.Include(pc => pc.Category)
                                                                    .Include(pc => pc.Post)
                                                                    .ThenInclude(p => p!.Author)
                                                                    .OrderByDescending(pc => pc.Post!.DateCreated).ToListAsync();
            var index = new Random().Next(PostCategories.Count - 5);
            return View(PostCategories.Skip(index).Take(5).ToList());
        }
        try
        {
            var category = appMvcContext.categories.Include(c => c.ParentCategory).AsEnumerable().FirstOrDefault(c => c.Slug == slug);
            var childCategories = appMvcContext.categories.Where(c => c.ParentCategory == category);
            var categoryPosts = new List<PostCategory>();
            var qr = appMvcContext.PostCategories.Include(pc => pc.Category)
                                                .Include(pc => pc.Post)
                                                .ThenInclude(p => p!.Author).ToList();
            
            categoryPosts.AddRange(qr.Where(pc => pc.Category == category));
            foreach(var c in childCategories) {
                categoryPosts.AddRange(qr.Where(pc => pc.Category == c));
            }
            categoryPosts = categoryPosts.OrderBy(pc => pc.Post!.DateCreated).ToList();

            var listParentsCategory = new List<Category>();
            ListParentsCategory(category!,listParentsCategory);
            ViewData["ParentsCategory"] = listParentsCategory;
            ViewBag.CurrentCategory = category;

            return View(categoryPosts);
        }
        catch (System.Exception)
        {
            return NotFound();
            throw;
        }
    }

    [Route("{action}/{slug}")]
    public async Task<IActionResult> Views(string slug) {
        try
        {
            var post = appMvcContext.Posts.Where(p => p.Slug == slug)
                                        .Include(p => p.Author)
                                        .Include(p => p.PostCategories)!
                                        .ThenInclude(pc => pc.Category)
                                        .FirstOrDefault();
            if(post == null) {
                return NotFound();
            }
            ViewBag.MessageForRoom = await appMvcContext.MessageComments.Include(m => m.Room)
                                                                    .Include(m => m.User)
                                                                    .Where(m => m.Room!.RoomName == slug)
                                                                    .ToListAsync();
            if(ViewBag.MessageForRoom == null) {
                ViewBag.MessageForRoom = new List<MessageCommentModel>();
            }
            return View(post);
        }
        catch (System.Exception)
        {
            return NotFound();
            throw;
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [NonAction]
    private void ListParentsCategory(Category category, List<Category> listParentsCategory) {
        if(category.ParentCategory != null) {
            listParentsCategory.Insert(0, category.ParentCategory);
            ListParentsCategory(category.ParentCategory, listParentsCategory);
        }
    }
}
