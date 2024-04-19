using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

[Route("{controller}/{action=Index}")]
public class StoreController : Controller {
    private readonly AppMvcContext appMvcContext;
    private readonly SignInManager<AppUser> signInManager;
    private readonly UserManager<AppUser> userManager;
    public const string CartKey = "cart";

    public StoreController(AppMvcContext appMvcContext, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager) {
        this.appMvcContext = appMvcContext;
        this.signInManager = signInManager;
        this.userManager = userManager;
    }

    [TempData]
    public string StatusMessage {set; get;} = default!;

    public IActionResult Index() {
        var products = appMvcContext.Products.Include(p => p.PhotoUploads).ToList();
        return View(products);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCart(InputCartModel cartItem) {
        var isLogin = signInManager.IsSignedIn(User);
        if(!isLogin) return LocalRedirect($"/login?ReturnUrl=/Store");

        var product = await appMvcContext.Products.FindAsync(cartItem.ProductId);
        if(product == null) {
            StatusMessage = "Khum có sản phẩm này"; 
            return RedirectToAction(nameof(Index));
        }

        var listCart = ListCart();
        var cartFind = listCart.Find(c => c.Product!.ProductId == cartItem.ProductId);
        if(cartFind != null) {
            cartFind.Quantity += cartItem.InputQuantity;
        }
        else {
            listCart.Add(new CartItemModel {Quantity = cartItem.InputQuantity, Product = product });
        }

        AddCart(listCart);
        StatusMessage = "Đã thêm vào giỏ hàng";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Cart() {
        var listCart = ListCart();
        return View(listCart);
    }

    // public async Task<IActionResult> PayMent() {
    //     var user = userm
    // }

    private List<CartItemModel> ListCart() {
        var session = this.HttpContext.Session;
        var jsonCart = session.GetString(CartKey);
        if(jsonCart != null) {
            return JsonConvert.DeserializeObject<List<CartItemModel>>(jsonCart)!;
        }
        
        return new List<CartItemModel>();
    }

    private void AddCart(IList<CartItemModel> cartNew) {
        var session = this.HttpContext.Session;
        var jsonCart = JsonConvert.SerializeObject(cartNew);
        session.SetString(CartKey, jsonCart);
    }
}