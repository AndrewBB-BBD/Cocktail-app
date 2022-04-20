using CocktailApp.Data;
using CocktailApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
namespace CocktailApp.Pages;
 
public class MixologyModel : PageModel
{
    private readonly cocktailDBContext _cocktailDBContext;
    public MixologyModel(cocktailDBContext cocktailDBContext)
    {
        _cocktailDBContext = cocktailDBContext;
    }

    public List<Category> categoriesList = new List<Category>();
    public List<Ingredient> ingredientsList = new List<Ingredient>();
    public List<Ingredient> selectedIngredients = new List<Ingredient>();

    public List<int> checkedIngredientIds = new List<int>();
 
    public async Task<IActionResult> OnGetAsync()
    {
        categoriesList = await _cocktailDBContext.Categories.ToListAsync();
        ingredientsList = await _cocktailDBContext.Ingredients.ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        ingredientsList = await _cocktailDBContext.Ingredients.ToListAsync();

        foreach(var item in ingredientsList) 
        {

        if (Request.Form[item.IngredientId.ToString()] == "on")
        {
            //add ingredient IDs that were checked
            checkedIngredientIds.Add(item.IngredientId);
        }
        }

        //Redirect to MixResult with IDs of ingredients user selected
        return RedirectToPage("/MixResult", new {userSelectedIds = checkedIngredientIds});
    }

}