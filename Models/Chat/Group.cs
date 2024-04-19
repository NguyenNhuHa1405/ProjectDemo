using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

public class Group {
    [Key]
    public string? GroupId {set; get;}
    public string? GroupName {set; get;}
    public bool? GroupRoom {set; get;}
    public DateTime? CreateDate {set; get;}
    public bool? IsActive {set; get;}

    public IList<UserGroup>? userGroups {set; get;}
}