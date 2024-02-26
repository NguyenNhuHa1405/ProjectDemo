using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.DiaSymReader;
using Microsoft.EntityFrameworkCore;

public class CommentHub : Hub {
    private readonly AppMvcContext appMvcContext;
    private readonly UserManager<AppUser> userManager;

    public CommentHub(AppMvcContext appMvcContext, UserManager<AppUser> userManager) {
        this.userManager = userManager;
        this.appMvcContext = appMvcContext;
    }

    public async Task JoinGroup(string groupName)
    {
        try {
            var isGroup = await appMvcContext.RoomComments.AnyAsync(r => r.RoomName == groupName);
            if(!isGroup) {
                var newRoom = new RoomCommentModel {
                    RoomName = groupName,
                    User = null
                };
                await appMvcContext.RoomComments.AddAsync(newRoom);
                await appMvcContext.SaveChangesAsync();
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("JoinGroup", $"{Context.ConnectionId} has joined the group {groupName}.");
        }catch (Exception e) {
            await Clients.Caller.SendAsync("OnError", $"Lỗi Server : {e.Message}");
        }
        
    }

    public async Task SendMessageToGroup(string groupName, string message) {
        try {
            var userLogin = await userManager.GetUserAsync(Context.User!);
            var room = await appMvcContext.RoomComments.FirstOrDefaultAsync(r => r.RoomName == groupName);
            if(userLogin == null) {
                userLogin = new AppUser {
                    UserName = "Không xác định"
                };
            }else {
                var newMessage = new MessageCommentModel {
                    Content = message,
                    Room = room,
                    User = userLogin
                };
                await appMvcContext.MessageComments.AddAsync(newMessage);
                await appMvcContext.SaveChangesAsync();
            }
            await Clients.Groups(groupName).SendAsync("ReceiveMessage", userLogin.UserName, message);
        }catch (Exception e) {
            await Clients.Caller.SendAsync("OnError", $"Lỗi Server : {e.Message}");
        }
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("LeaveGroup", $"{Context.ConnectionId} has left the group {groupName}.");
    }
}