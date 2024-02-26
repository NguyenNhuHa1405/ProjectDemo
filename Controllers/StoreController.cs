using Microsoft.AspNetCore.Mvc;

[Route("{controller}/{action=Index}")]
public class StoreController : Controller {
    public readonly AppMvcContext appMvcContext;

    public StoreController(AppMvcContext appMvcContext) {
        this.appMvcContext = appMvcContext;
    }

    public IActionResult Index() {
        var products = appMvcContext.Products.ToList();
        return View(products);
    }
}