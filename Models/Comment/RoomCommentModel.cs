using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("RoomComment")]
public class RoomCommentModel {
    [Key]
    public int RoomId {set; get;}

    public string? RoomName {set; get;}
    public AppUser? User {set; get;}
}