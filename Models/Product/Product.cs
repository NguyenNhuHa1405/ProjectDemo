using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Product {
    [Key]
    public int ProductId {set; get;}

    [Required(ErrorMessage = "Phải có {0}")]
    [Display(Name = "Tên sản phẩm")]
    [StringLength(160, MinimumLength = 5, ErrorMessage = "{0} dài {1} đến {2}")]
    public string? Name {set; get;}

    [Display(Name = "Mô tả ngắn")]
    public string? Description {set; get;}
    public string? Slug {set; get;}

    [Display(Name = "Nội dung")]
    public string? Content {set; get;}

    [Required(ErrorMessage = "Phải có {0}")]
    [DisplayName("Giá sản phẩm")]
    public decimal Price {set; get;}

    [Display(Name = "Tác giả")]
    public string? AuthorId {set; get;}
    
    [ForeignKey("AuthorId")]
    [Display(Name = "Tác giả")]
    public AppUser? Author {set; get;}

    [Display(Name = "Ngày tạo")]
    public DateTime DateCreated {set; get;}

    [Display(Name = "Ngày cập nhật")]
    public DateTime DateUpdated {set; get;}

    public List<ProductCategory>? ProductCategories {set; get;}
    public List<PhotoUploadModel>? PhotoUploads {set; get;}
}