using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;

public class ContactModel {
    [Key]
    public int Id {set;get;}

    [Column(name: "nvarchar")]
    [Required(ErrorMessage ="Phải nhập {0}")]
    [StringLength(50)]
    [DisplayName("Họ tên")]
    public string? FullName {set;get;}

    [Required(ErrorMessage ="Phải nhập {0}")]
    [StringLength(100)]
    [EmailAddress(ErrorMessage ="Phải là địa chỉ {0}")]
    public string? Email {set;get;}
    
    public DateTime? DataSent {set;get;}

    [DisplayName("Nội dung")]
    public string? Message {set;get;}
    
    // [Phone(ErrorMessage ="Phải là số điện thoại")]
    [RegexStringValidator("(84|0[3|5|7|8|9])+([0-9]{8})\b")]
    [DisplayName("Số điện thoại")]
    public uint? Phone {set;get;}
}