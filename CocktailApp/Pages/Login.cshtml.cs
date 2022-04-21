using CocktailApp.Data;
using CocktailApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CocktailApp.Pages{ 
public class LoginModel : PageModel
{
    private readonly cocktailDBContext _cocktailDBContext;
    public LoginModel(cocktailDBContext cocktailDBContext)
    {
        _cocktailDBContext = cocktailDBContext;
    }
    [BindProperty]
    public Credential Credential { get; set;}
    public bool userExists {get; set;} = true;
    public void OnGet(){}
    static private string deEncodeFrom64 (byte[] toDecode) {
        string convertFromBytes = System.Text.ASCIIEncoding.ASCII.GetString(toDecode);
        var removedExcess = convertFromBytes.Replace("\0", "");
        byte[] toDecodeAsBytes = System.Convert.FromBase64String(removedExcess);
        return System.Text.ASCIIEncoding.ASCII.GetString(toDecodeAsBytes);
    }
    public IActionResult OnPost()
    {
        var UserNameExists = _cocktailDBContext.UserLogins.FirstOrDefault(first => first.Username == Credential.UserName);
        if(UserNameExists is not null) {
            byte[] passwordToDecode = UserNameExists.UserPassword;
            var actualPassword = deEncodeFrom64(passwordToDecode);
            if(actualPassword == (Credential.Password + UserNameExists.Salt)) {
                return RedirectToPage("Index", new {loggedInUser = Credential.UserName});
            }
            return Page();
        } else {
            userExists = false;
            return Page();
        }
    }
}
public class Credential 
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName {get; set;} = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password {get; set;} = null!;
    }
}