using CocktailApp.Data;
using CocktailApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CocktailApp.Pages{ 
public class RegisterModel : PageModel
{
    private readonly cocktailDBContext _cocktailDBContext;
    public bool emailExists {get; set;} = false;
    public bool userNameExists {get; set;} = false;
    
    public RegisterModel(cocktailDBContext cocktailDBContext)
    {
        _cocktailDBContext = cocktailDBContext;
    }

    [BindProperty]
    public RegisterCredential RegisterCredential { get; set;}
    public void OnGet(){}

    static private byte[] EncodeTo64(string toEncode)
    {
      byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
      string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
      return System.Text.ASCIIEncoding.ASCII.GetBytes(returnValue);
    }

    public IActionResult OnPost()
    {
        var UserEmailExists = _cocktailDBContext.UserLogins.FirstOrDefault(first => first.UserEmail == RegisterCredential.Email);
        var UserNameExists = _cocktailDBContext.UserLogins.FirstOrDefault(first => first.Username == RegisterCredential.UserName);
        if (UserEmailExists is null && UserNameExists is null) {
            Guid salt = Guid.NewGuid();
            UserLogin user = new UserLogin{
                UserEmail = RegisterCredential.Email,
                Username = RegisterCredential.UserName,
                UserPassword = EncodeTo64(RegisterCredential.Password + salt), //convert to byte64() + UUID for password storage.
                Salt = salt //Generate UUID before this point
            };
            _cocktailDBContext.UserLogins.Add(user);
            _cocktailDBContext.SaveChanges();
            return RedirectToPage("Login", new {});
        } else
        {
            if(UserEmailExists is not null)
                emailExists = true;
            if(UserNameExists is not null)
                userNameExists = true;
            return Page();
        }
    }
}

public class RegisterCredential 
    {
        [RegularExpression(@"/^[a-zA-Z0-9_!#$%&*+\/=?`{|}~^.-]+@[a-zA-Z0-9.-]+$/g")]
        [Required]
        [Display(Name = "Email Address")]
        public string Email {get; set;} = null!;

        [StringLength(50, MinimumLength = 8)]
        [Required]
        [Display(Name = "User Name")]
        public string UserName {get; set;} = null!;

        [StringLength(50, MinimumLength = 8)]
        [Required]
        [DataType(DataType.Password)]
        public string Password {get; set;} = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string PasswordConfirmation {get; set;} = null!;
    }
}