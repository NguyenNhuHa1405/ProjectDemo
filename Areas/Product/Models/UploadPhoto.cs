using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public class UploadPhoto {
    [Required(ErrorMessage = "Phải có {0}")]
    [DataType(DataType.Upload)]
    // [FileExtensions(Extensions = "png,jpg,jpeg", ErrorMessage = "Không đúng định dạng")]
    [DisplayName("File IMG")]
    [BindProperty]
    public IFormFile[]? Files {set; get;}
}