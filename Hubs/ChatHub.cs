using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

public class ChatHub : Hub {
    private readonly AppMvcContext appMvcContext;
    private readonly UserManager<AppUser> userManager;

    public ChatHub(AppMvcContext appMvcContext, UserManager<AppUser> userManager) {
        this.userManager = userManager;
        this.appMvcContext = appMvcContext;
    }

    public async Task JoinGroup(string groupId) {
        try {
            var user = await userManager.GetUserAsync(Context.User!); 
            var group = await appMvcContext.Groups.FindAsync(groupId);
            var checkUserInGroup = await appMvcContext.UserGroups.AnyAsync(us => us.UserId == user!.Id && us.GroupId == group!.GroupId);
            if(!checkUserInGroup) throw new Exception("");

            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            await Clients.Group(groupId).SendAsync("JoinGroup", $"{user!.UserName} joined group {groupId}");
            Console.WriteLine("successfully joined group");
        }catch(Exception e) {
            Console.WriteLine(e.Message);
            await Clients.Caller.SendAsync("OnError", $"Có lỗi xảy ra {e.Message}");
        }
    }

    public async Task SendToGroup(string groupId, string messages) {
        try {
            var user = await userManager.GetUserAsync(Context.User!); 
            var group = await appMvcContext.Groups.FindAsync(groupId);
            var checkUserInGroup = await appMvcContext.UserGroups.AnyAsync(us => us.UserId == user!.Id && us.GroupId == group!.GroupId);
            if(!checkUserInGroup) throw new Exception("");

            var message = new Message {
                MessageId = Guid.NewGuid().ToString(),
                MessageBody = messages,
                DateCreated = DateTime.Now
            };

            var messageGroup = new MessageGroup {
                MessageGroupId = Guid.NewGuid().ToString(),
                Group = group,
                Message = message,
                SendDate = DateTime.Now,
                User = user
            };

            await appMvcContext.Messages.AddAsync(message);
            await appMvcContext.MessageGroups.AddAsync(messageGroup);
            await appMvcContext.SaveChangesAsync();

            await Clients.Group(groupId).SendAsync("ReceiveMessage", $"{user!.Id}", $"{messages}");
            Console.WriteLine("ReceiveMessage");
        }catch(Exception e) {
            Console.WriteLine(e.Message);
            await Clients.Caller.SendAsync("OnError", $"Có lỗi xảy ra {e.Message}");
        }
    }
}