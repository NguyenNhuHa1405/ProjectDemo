using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

public class PhotoUploadModel {
    [Key]
    public int Id {set; get;}
    public string? FileName {set; get;}
    public int ProductId {set; get;}
    [ForeignKey("ProductId")]
    public Product? Product {set; get;}
}