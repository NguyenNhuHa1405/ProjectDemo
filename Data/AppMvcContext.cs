using System.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppMvcContext : IdentityDbContext<AppUser> {
    public AppMvcContext(DbContextOptions<AppMvcContext> options) : base(options) { }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach(var entityType in modelBuilder.Model.GetEntityTypes()){
            var tableName = entityType.GetTableName();
            if(tableName != null) {
                if(tableName.StartsWith("AspNet")) {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
        }

        modelBuilder.Entity<PostCategory>(entity => {
            entity.HasKey(p => new { p.PostID, p.CategoryID });
        });
        modelBuilder.Entity<ProductCategory>(entity => {
            entity.HasKey(p => new { p.ProductId, p.StoreCategoryId });
        });
    }

    public DbSet<ContactModel> contacts {set; get;}
    public DbSet<Category> categories {set; get;}
    public DbSet<Post> Posts {set; get;}
    public DbSet<PostCategory> PostCategories {set; get;}
    public DbSet<Product> Products {set; get;}
    public DbSet<ProductCategory> ProductCategories {set; get;}
    public DbSet<StoreCategory> StoreCategories {set; get;}
    public DbSet<PhotoUploadModel> PhotoUploads {set; get;}
    public DbSet<RoomCommentModel> RoomComments {set; get;}
    public DbSet<MessageCommentModel> MessageComments {set; get;}
}