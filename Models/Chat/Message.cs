using System.ComponentModel.DataAnnotations;

public class Message {
    [Key]
    public string? MessageId { get; set; }

    public string? MessageBody { get; set; }
    
    public DateTime? DateCreated { get; set; }
}