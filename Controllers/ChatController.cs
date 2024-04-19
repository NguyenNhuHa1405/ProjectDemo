using Bogus.DataSets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;

[Route("{controller}/{action=Index}")]
[Authorize]
public class ChatController : Controller {
    private readonly UserManager<AppUser> userManager;
    private readonly AppMvcContext appMvcContext;

    public ChatController(UserManager<AppUser> userManager, AppMvcContext appMvcContext) {
        this.userManager = userManager;
        this.appMvcContext = appMvcContext;
    }
    public async Task<IActionResult> Index() {
        var user = await userManager.GetUserAsync(this.User);
        var users = new List<AppUser>();
        foreach(var us in userManager.Users.ToList()) {
            if(us.Id != user!.Id) {
                users.Add(us);
            }
        }
        ViewBag.User = user;
        return View(users);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> AddUser(string userId) {
        try {
            var user = await userManager.GetUserAsync(this.User);
            if(await CheckFriend(userId)) throw new Exception("Fail");
            var group = new Group {
                GroupId = Guid.NewGuid().ToString(),
                CreateDate = DateTime.Now,
                IsActive = true
            };
            var userGroups = new List<UserGroup> {
                new UserGroup { UserId = userId, Group = group, IsActive = true, JoinDate = DateTime.Now},
                new UserGroup { UserId = user!.Id, Group = group, IsActive = true, JoinDate = DateTime.Now}
            };
            await appMvcContext.Groups.AddAsync(group);
            await appMvcContext.UserGroups.AddRangeAsync(userGroups);
            await appMvcContext.SaveChangesAsync();
            return Json(new CheckFriendModel { Message = "OK", Success = true});
        }catch {
            return Json(new CheckFriendModel { Message = "Fail", Success = false});
        }
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> DeleteUser(string userId) {
        try
        {
            var user = await userManager.GetUserAsync(this.User);
            if(!(await CheckFriend(userId))) throw new Exception("Fail");
            var groups = await appMvcContext.Groups.Include(gr => gr.userGroups).ToListAsync();
            foreach(var group in groups) {
                var userFriend = group.userGroups!.Where(us => us.UserId == userId || us.UserId == user!.Id).Count();
                if(userFriend >= 2) {
                    var messageGroup = appMvcContext.MessageGroups.Where(ms => ms.GroupId == group.GroupId);
                    appMvcContext.MessageGroups.RemoveRange(messageGroup);
                    appMvcContext.Groups.Remove(group);
                    await appMvcContext.SaveChangesAsync();
                    return Json(new CheckFriendModel { Message = "OK", Success = true});
                }
            }
            throw new Exception();
        }
        catch (System.Exception)
        {
            return Json(new CheckFriendModel { Message = "Fail", Success = false});
            throw;
        }
    }

    [HttpGet("{groupId}")]
    public async Task<IActionResult> Group(string groupId) {
        try
        {
            var user = await userManager.GetUserAsync(this.User);
            if(!(await appMvcContext.UserGroups.AnyAsync(us => us.UserId == user!.Id && us.GroupId == groupId))) throw new Exception();
            var messageGroups = await appMvcContext.MessageGroups.Include(m => m.Message).Where(mg => mg.GroupId == groupId).ToListAsync();
            var messageGroupsBox = new List<MessageGroupBox>();
            foreach(var ms in messageGroups) {
                var messageGroupBox = new MessageGroupBox();
                if(ms.UserId == user!.Id) {
                    messageGroupBox.IsUserLogin = true;
                }
                messageGroupBox.GroupId = ms.GroupId;
                messageGroupBox.MessageId = ms.MessageId;
                messageGroupBox.UserId = ms.UserId;
                messageGroupBox.Message = ms.Message;
                messageGroupBox.SendDate = ms.SendDate;
                messageGroupsBox.Add(messageGroupBox);
            }
            messageGroupsBox = messageGroupsBox.OrderBy(mg => mg.SendDate).ToList();
            return Json(new {
                Status = true,
                Message = messageGroupsBox
            });
        }
        catch (System.Exception)
        {
            return Json(new {
                Message = "Fail",
                Status = false
            });
            throw;
        }
    }

    [NonAction]
    private async Task<bool> CheckFriend(string userId) {
        try {
            var groups = await appMvcContext.Groups.Include(g => g.userGroups).ToListAsync();
            var user = await userManager.GetUserAsync(this.User);
            foreach (var group in groups) {
                var userFriend = group.userGroups!.Where(us => us.UserId == userId || us.UserId == user!.Id).Count();
                if(userFriend >= 2) {
                    return true;
                }
            }
        }catch {
            return false;
        }
        return false;
    }
}