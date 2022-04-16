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

    public List<Favourite> favouritesList = new List<Favourite>();
    public List<Recipe> usersFavouritesList = new List<Recipe>();
    private string currentUserEmail = "test@test.com";

    public async Task<IActionResult> OnGetAsync()
    {

        favouritesList = await _cocktailDBContext.Favourites.ToListAsync();

        foreach (Favourite entry in favouritesList)
        {
            // Create a list of recipes - waiting for a way to tell which user is logged in
            // usersFavouritesList.Add(_cocktailDBContext.Recipes.Where(r => r.RecipeId == entry.RecipeId).First());


            // Create a list of a user's favourite recipes (List of Recipe type)
            if (entry.UserEmail == currentUserEmail)
            {
                usersFavouritesList.Add(_cocktailDBContext.Recipes.Where(r => r.RecipeId == entry.RecipeId).First());
            }


        }

        foreach (var recipe in usersFavouritesList)
        {
            int start = recipe.RecipeImage.IndexOf("/file/d/") + 8;
            int end = recipe.RecipeImage.IndexOf("/view?usp=sharing");
            string imageURL = "https://drive.google.com/uc?id=" + recipe.RecipeImage.Substring(start, end - start);
            recipe.RecipeImage = imageURL;
        }

        return Page();
    }

    public IActionResult OnGetViewRecipe(int ID)
    {
        return RedirectToPage("Recipe", new { recipeID = ID });
    }
}