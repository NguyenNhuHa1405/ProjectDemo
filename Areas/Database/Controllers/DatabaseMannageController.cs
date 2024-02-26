using App.Areas.Identity.Models.RoleViewModels;
using App.Data;
using App.ExtendMethods;
using Bogus;
using Bogus.DataSets;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit.Cryptography;

[Area("Database")]
[Route("{controller}/{action=Index}")]
public class DatabaseManageController : Controller {
    private readonly AppMvcContext appMvcContext;
    private readonly UserManager<AppUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;

    public DatabaseManageController(AppMvcContext appMvcContext, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager) {
        this.userManager = userManager;
        this.appMvcContext = appMvcContext;
        this.roleManager = roleManager;
    }

    [HttpGet]
    public IActionResult Index() {
        return View();
    }
    
    [HttpGet]
    public IActionResult DeleteDb() {
        return View();
    }

    [TempData]
    public string StatusMessage {set; get;} = default!;

    [HttpPost]
    public async Task<IActionResult> DeleteDbAsync() {
        var success = await appMvcContext.Database.EnsureDeletedAsync();
        StatusMessage = success ? "Xóa Database thành công" : "Không xóa được";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Migrate() {
        await appMvcContext.Database.MigrateAsync();
        StatusMessage = "Cập nhật Database thành công";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> AddRole() {
        var user = await userManager.GetUserAsync(User);
        await SeedData();
        if(user == null) {
            StatusMessage = "Không có user";
            return RedirectToAction("Index");
        }
        var roleFind = await roleManager.FindByNameAsync(RoleName.Administrator);
        if(roleFind == null) {
            await roleManager.CreateAsync(new IdentityRole(RoleName.Administrator));
        }
        var result = await userManager.AddToRoleAsync(user, RoleName.Administrator);
        if(!result.Succeeded) {
            StatusMessage = "Không thêm được role";
            return RedirectToAction("Index");
        }
        StatusMessage = "Thêm thành công";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> RenderProduct() {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        appMvcContext.StoreCategories.RemoveRange(appMvcContext.StoreCategories.Where(c => c.Content.Contains("[fakedata]")));
        appMvcContext.Products.RemoveRange(appMvcContext.Products.Where(c => c.Content.Contains("[fakedata]")));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        Randomizer.Seed = new Random(8675309);
        var fake = new Faker<StoreCategory>();
        var cm = 1;
        fake.RuleFor(c => c.Title, f => $"Cm{cm++} " + f.Lorem.Sentence(1, 2).Trim('.'));
        fake.RuleFor(c => c.Content, f => f.Lorem.Sentence(5) + "[fakedata]");
        fake.RuleFor(c => c.Slug, f => f.Lorem.Slug());

        var cate1 = fake.Generate();
        var cate11 = fake.Generate();
        var cate12 = fake.Generate();
        var cate2 = fake.Generate();
        var cate21 = fake.Generate();
        var cate22 = fake.Generate();

        cate11.ParentCategory = cate1;
        cate12.ParentCategory = cate1;
        cate21.ParentCategory = cate2;
        cate22.ParentCategory = cate2;

        var categories = new StoreCategory[] {cate1, cate11, cate12, cate2, cate22, cate22};

        var random = new Random();
        var user = await userManager.GetUserAsync(User);
        var faker = new Faker<Product>();
        faker.RuleFor(b => b.Name, f =>  f.Lorem.Sentence(3, 4).Trim('.'));
        faker.RuleFor(b => b.Author, f => user);
        faker.RuleFor(b => b.Description, f => f.Lorem.Sentence(3));
        faker.RuleFor(b => b.Slug, f => f.Lorem.Slug());
        faker.RuleFor(b => b.Content, f => f.Lorem.Paragraph() + "[fakedata]");
        faker.RuleFor(b => b.DateCreated, fake => DateTime.Now);
        faker.RuleFor(b => b.Price, f => f.Random.Number(10000, 1000000));

        var posts = new List<Product>();
        var postCategories = new List<ProductCategory>();

        for(var i = 0; i < 40; i++) {
            var post = faker.Generate();
            posts.Add(post);
            postCategories.Add(new ProductCategory() {
                Product = post,
                StoreCategory = categories[random.Next(5)]
            });
        }

        await appMvcContext.Products.AddRangeAsync(posts);
        await appMvcContext.StoreCategories.AddRangeAsync(categories);
        await appMvcContext.ProductCategories.AddRangeAsync(postCategories);
        await appMvcContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task SeedData() {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        appMvcContext.categories.RemoveRange(appMvcContext.categories.Where(c => c.Content.Contains("[fakedata]")));
        appMvcContext.Posts.RemoveRange(appMvcContext.Posts.Where(c => c.Content.Contains("[fakedata]")));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        Randomizer.Seed = new Random(8675309);
        var fake = new Faker<Category>();
        var cm = 1;
        fake.RuleFor(c => c.Title, f => $"Cm{cm++} " + f.Lorem.Sentence(1, 2).Trim('.'));
        fake.RuleFor(c => c.Content, f => f.Lorem.Sentence(5) + "[fakedata]");
        fake.RuleFor(c => c.Slug, f => f.Lorem.Slug());

        var cate1 = fake.Generate();
        var cate11 = fake.Generate();
        var cate12 = fake.Generate();
        var cate2 = fake.Generate();
        var cate21 = fake.Generate();
        var cate22 = fake.Generate();

        cate11.ParentCategory = cate1;
        cate12.ParentCategory = cate1;
        cate21.ParentCategory = cate2;
        cate22.ParentCategory = cate2;

        var categories = new Category[] {cate1, cate11, cate12, cate2, cate22, cate22};

        var random = new Random();
        var bv = 1;
        var user = await userManager.GetUserAsync(User);
        var faker = new Faker<Post>();
        faker.RuleFor(b => b.Title, f => $"Bài {bv++} " + f.Lorem.Sentence(3, 4).Trim('.'));
        faker.RuleFor(b => b.Author, f => user);
        faker.RuleFor(b => b.Description, f => f.Lorem.Sentence(3));
        faker.RuleFor(b => b.Slug, f => f.Lorem.Slug());
        faker.RuleFor(b => b.Content, f => f.Lorem.Paragraph() + "[fakedata]");
        faker.RuleFor(b => b.Published, fake => true);
        faker.RuleFor(b => b.DateCreated, fake => DateTime.Now);

        var posts = new List<Post>();
        var postCategories = new List<PostCategory>();

        for(var i = 0; i < 40; i++) {
            var post = faker.Generate();
            posts.Add(post);
            postCategories.Add(new PostCategory() {
                Post = post,
                Category = categories[random.Next(5)]
            });
        }

        await appMvcContext.Posts.AddRangeAsync(posts);
        await appMvcContext.categories.AddRangeAsync(categories);
        await appMvcContext.PostCategories.AddRangeAsync(postCategories);
        await appMvcContext.SaveChangesAsync();
    }
}