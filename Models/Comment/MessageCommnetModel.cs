using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("MessageComment")]
public class MessageCommentModel {
    [Key]
    public int MessageId { get; set; }
    public string? Content { get; set; }
    public string? UserId { get; set; }

    [ForeignKey("UserId")]
    public AppUser? User { get; set; }
    public int? RoomId {set; get;}

    [ForeignKey("RoomId")]
    public RoomCommentModel? Room {set; get;}
}