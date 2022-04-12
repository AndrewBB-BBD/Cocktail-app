using CocktailApp.Data;
using CocktailApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
namespace CocktailApp.Categories;
 
public class IndexModel : PageModel
{
    private readonly cocktailDBContext _cocktailDBContext;
    public IndexModel(cocktailDBContext cocktailDBContext)
    {
        _cocktailDBContext = cocktailDBContext;
    }
 
    public List<Category> AllCakes = new List<Category>();
 
    public async Task<IActionResult> OnGetAsync()
    {
        AllCakes = await _cocktailDBContext.Categories.ToListAsync();
        return Page();
    }
}