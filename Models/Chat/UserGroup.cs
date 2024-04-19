using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UserGroup {

    public string? GroupId {set; get;}
    public string? UserId {set; get;}

    [ForeignKey("UserId")]
    public AppUser? user {set; get;}

    [ForeignKey("GroupId")]
    public Group? Group {set; get;}

    public DateTime? JoinDate {set; get;}
    public bool? IsActive {set; get;}
}