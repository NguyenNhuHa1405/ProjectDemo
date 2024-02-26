using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class CreateProductModel : Product {   
    [DisplayName("Danh mục")]
    [Required(ErrorMessage = "Phải có {0}")]
    public int CategoryId { set; get; }
}