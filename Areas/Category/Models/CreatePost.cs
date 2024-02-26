using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class CreatePostModel : Post {   
    [DisplayName("Danh mục")]
    [Required(ErrorMessage = "Phải có {0}")]
    public int CategoryId { set; get; }
}