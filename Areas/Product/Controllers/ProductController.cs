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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.Intrinsics.X86;
using Microsoft.AspNetCore.Http.Metadata;

[Area("Product")]
[Route("{controller}/{action=Index}")]
public class ProductController : Controller {
    private readonly AppMvcContext appMvcContext;
    private readonly UserManager<AppUser> userManager;
    private readonly IWebHostEnvironment env;

    public ProductController(AppMvcContext appMvcContext, UserManager<AppUser> userManager, IWebHostEnvironment env) {
        this.appMvcContext = appMvcContext;
        this.userManager = userManager;
        this.env = env;
    }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> Index([FromQuery(Name = "p")] int currentPage, int pageSize) {
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        var posts = await appMvcContext.Products
                            .Include(p => p.Author)
                            .Include(p => p.ProductCategories)
                            .ThenInclude(pc => pc.StoreCategory)
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
        var categories = await appMvcContext.StoreCategories.ToListAsync();
        var items = new SelectList(categories, "Id", "Title");

        ViewBag.items = items;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind("Name,Price,Description,Content,CategoryId")] CreateProductModel post) {
        if(ModelState.IsValid) {
            var userCreate = await userManager.GetUserAsync(User);
            var category = await appMvcContext.StoreCategories.FindAsync(post.CategoryId);
            if(category == null) {
                return NotFound();
            }
            var ramdom = new Random();
            post.Author = userCreate;
            post.DateCreated = DateTime.Now;
#nullable disable
            post.Slug = AppUtilities.GenerateSlug(post.Name);
            if(await appMvcContext.Products.AnyAsync(p => p.Slug == post.Slug)) {
                post.Slug += ramdom.Next(0);
            }
#nullable enable

            var postCategory = new ProductCategory() {
                Product = post,
                StoreCategory = category
            };

            await appMvcContext.ProductCategories.AddAsync(postCategory);
            await appMvcContext.Products.AddAsync(post);
            await appMvcContext.SaveChangesAsync();
            StatusMessage = "Thêm thành công";
            return RedirectToAction(nameof(Index));
        }
        StatusMessage = "Không thể thêm";
        return View(post);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Delete(int id) {
        var post = await appMvcContext.Products.FindAsync(id);
        return View(post);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> DeleteAsync(int id) {
        var post = await appMvcContext.Products.FindAsync(id);
        if(post == null) {
            StatusMessage = "Không thể xóa";
            return RedirectToAction(nameof(Index));
        }
        try
        {
            appMvcContext.Products.Remove(post);
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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Edit(int id) {
        try {
            var categoryID = appMvcContext.ProductCategories.Where(pc => pc.ProductId == id)
                                                            .Select(pc => pc.StoreCategoryId)
                                                            .FirstOrDefault();
            var product = appMvcContext.Products.FirstOrDefault(p => p.ProductId == id);
            var productEdit = new CreateProductModel {
                CategoryId = categoryID,
                Name = product!.Name,
                Description = product!.Description,
                Content = product!.Content,
                Price = product!.Price,
                ProductId = product!.ProductId
            };

            var categories = await appMvcContext.StoreCategories.ToListAsync();
            ViewBag.Items = new SelectList(categories, "Id", "Title");
            
            return View(productEdit);
        }catch (Exception) {
            return NotFound();
        }
    }

    [HttpPost("{id:int}"), ActionName("Edit")]
    public async Task<IActionResult> EditAsync(int id, CreateProductModel productModel) {
        var random = new Random();
        if(ModelState.IsValid) {
            try {
                var productUpdate = appMvcContext.Products.Include(p => p.ProductCategories)
                                                            !.ThenInclude(pc => pc.StoreCategory)
                                                            .AsEnumerable()
                                                            .FirstOrDefault(p => p.ProductId == id);
                var productCategory = productUpdate!.ProductCategories!.FirstOrDefault();
                appMvcContext.ProductCategories.Remove(productCategory!);
                productUpdate.Name = productModel.Name;
                productUpdate.Description = productModel.Description;
                productUpdate.Slug = productModel.Slug;

                var check = await appMvcContext.Products.Where(p => p.ProductId != productUpdate.ProductId)
                                                        .AnyAsync(p => p.Slug == productUpdate.Slug);
                if(check) {
                    productUpdate.Slug += random.Next(0);
                }
                productUpdate.Content = productModel.Content;
                productUpdate.Price = productModel.Price;
                productUpdate.DateUpdated = DateTime.Now;
                appMvcContext.Products.Update(productUpdate);

                var newCategory = await appMvcContext.StoreCategories.FindAsync(productModel.CategoryId);
                var newCategoryProduct = new ProductCategory {
                    StoreCategory = newCategory,
                    Product = productUpdate
                };
                await appMvcContext.ProductCategories.AddAsync(newCategoryProduct);
                await appMvcContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }catch {
                return NotFound();
            }
        }
        return NotFound();
    }

    [HttpGet("{id:int}")]
    public IActionResult UploadPhoto(int id) {
        var product = appMvcContext.Products.Include(p => p.PhotoUploads).AsEnumerable().FirstOrDefault(p => p.ProductId == id);
        if(product == null) {
            return NotFound();
        }
        ViewData["Product"] = product;
        return View(new UploadPhoto());
    }

    [HttpPost("{id:int}"), ActionName("UploadPhoto")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadPhotoAsync(int id, UploadPhoto uploadPhoto) {
        if(ModelState.IsValid) {
            var product = appMvcContext.Products.Include(p => p.PhotoUploads).AsEnumerable().FirstOrDefault(p => p.ProductId == id);
            try {
            var listFilePhoto = new List<PhotoUploadModel>();
            var fileExtensions = new string[]{"jpg", "png"};
            foreach(var file in uploadPhoto.Files!) {
                var fileName = file.FileName.ToLower();
                var checkExtension = fileExtensions.Contains(fileName.Split('.')[1]);
                if(!checkExtension) {
                    ModelState.AddModelError("Không đúng định dạng");
                    ViewBag.Product = product;
                    return View(new UploadPhoto());
                }
                var path = Path.Combine(env.WebRootPath, "uploads", file.FileName);
                using var stream = new FileStream(path, FileMode.Create);
                if(!await appMvcContext.PhotoUploads.Include(pt => pt.Product)
                                                    .Where(pt => pt.Product == product)
                                                    .AnyAsync(pt => pt.FileName == file.FileName)) {
                    listFilePhoto.Add(new PhotoUploadModel {
                        FileName = file.FileName,
                        Product = product
                    });
                }
                await file.CopyToAsync(stream);
            }
            await appMvcContext.PhotoUploads.AddRangeAsync(listFilePhoto);
            await appMvcContext.SaveChangesAsync();

            return RedirectToAction(nameof(UploadPhoto));
            }catch {
                return NotFound();
            }
        }
        return NotFound();
    }

    [HttpGet("{productId:int}")]
    public async Task<IActionResult> ListPhoto(int productId) {
        var product = await appMvcContext.Products.Include(p => p.PhotoUploads)
                                                .Where(p => p.ProductId == productId)
                                                .FirstOrDefaultAsync();
        
        if(product == null) {
            return Json(new {
                success = 0,
                path = "Không có content"
            });
        }
        var listPhoto = product?.PhotoUploads?.Select(p => new {
                                                success = 1,
                                                path = $"/uploads/{p.FileName}",
                                                id = p.Id
                                            });

        return Json(new {
            path = listPhoto
        });
    }

    [HttpDelete("{photoId:int}")]
    public async Task<IActionResult> DeletePhoto(int photoId) {
        var photo = await appMvcContext.PhotoUploads.FindAsync(photoId);
        if(photo == null) return Json(new { success = 0, Content = "Lỗi" });

        appMvcContext.PhotoUploads.Remove(photo);
        await appMvcContext.SaveChangesAsync();
        return Json(new { success = 1, content = "Thành công" });
    }
}