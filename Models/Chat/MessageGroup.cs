using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class MessageGroup {
    [Key]
    public string? MessageGroupId { get; set; }
    public string? MessageId { get; set; }
    public string? GroupId {set; get;}
    public string? UserId {set; get;}

    [ForeignKey("UserId")]
    public AppUser? User {set; get;}

    [ForeignKey("GroupId")]
    public Group? Group {get; set; }

    [ForeignKey("MessageId")]
    public Message? Message {get; set; }

    public DateTime? SendDate {get; set; }
}