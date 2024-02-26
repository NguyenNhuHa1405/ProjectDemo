using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asp.Controllers;

[Area("Contact")]
[Route("{controller}/{action=Index}")]
public class ContactController : Controller {
    private readonly AppMvcContext appMvcContext;

    public ContactController(AppMvcContext appMvcContext) {
        this.appMvcContext = appMvcContext;
    }

    [TempData]
    public string StatusMessage {set; get;} = default!;

    [HttpGet]
    public async Task<IActionResult> Index() {
        var contacts = await appMvcContext.contacts.ToListAsync();
        return View(model: contacts);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult SendContact() {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendContact([Bind("FullName", "Email", "Message", "Phone")] ContactModel contactModel) {
        contactModel.DataSent = DateTime.Now;
        if(ModelState.IsValid) {
            try
            {
                appMvcContext.contacts.Add(contactModel);
                await appMvcContext.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                StatusMessage = "Có lỗi xảy ra";
                return RedirectToAction(nameof(Index), "Home");
            }
            StatusMessage = "Phản hồi thành công";
            return RedirectToAction(nameof(Index), "Home"); 
        }
        return View(contactModel);
    }
}