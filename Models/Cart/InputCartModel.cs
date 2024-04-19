using Microsoft.AspNetCore.Mvc;

public class InputCartModel {
    [BindProperty]
    public int InputQuantity {set; get;}

    [BindProperty]
    public int ProductId {set; get;}
}