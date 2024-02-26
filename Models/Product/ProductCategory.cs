using System.ComponentModel.DataAnnotations.Schema;

public class ProductCategory {
    public int ProductId {set; get;}

    public int StoreCategoryId {set; get;}

    [ForeignKey("ProductId")]
    public Product? Product {set; get;}

    [ForeignKey("StoreCategoryId")]
    public StoreCategory? StoreCategory {set; get;}
}