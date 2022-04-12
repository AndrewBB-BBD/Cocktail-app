using CocktailApp.Data;
using CocktailApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
namespace CocktailApp.Pages;
 
public class FavouritesModel : PageModel
{
    private readonly cocktailDBContext _cocktailDBContext;
    public FavouritesModel(cocktailDBContext cocktailDBContext)
    {
        _cocktailDBContext = cocktailDBContext;
    }
 
 
    public async Task<IActionResult> OnGetAsync()
    {
        return Page();
    }
}