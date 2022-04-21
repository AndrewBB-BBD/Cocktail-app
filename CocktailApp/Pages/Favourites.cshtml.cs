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
    private Favourite deletedFavourite = new Favourite();
    private string currentUserEmail = "test@test.com";
    public List<Rating> ratings = new List<Rating>();
    public async Task<IActionResult> OnGetAsync()
    {
        favouritesList = await _cocktailDBContext.Favourites.ToListAsync();
        ratings = _cocktailDBContext.Ratings.ToList();
        foreach (Favourite entry in favouritesList)
        {
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
    public async Task<IActionResult> OnPostDeleteFromFavourites(int recipeID)
    {
        deletedFavourite.RecipeId = recipeID;
        deletedFavourite.UserEmail = currentUserEmail;
        try
        {
            _cocktailDBContext.Favourites.Remove(deletedFavourite);
            await _cocktailDBContext.SaveChangesAsync();
            return Page();
        }
        catch (NullReferenceException)
        {
            Console.WriteLine("NullReferenceException occured");
            return Page();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Some other exception occured. See details below: ");
            Console.WriteLine("\nMessage ---\n{0}", ex.Message);
            return Page();
        }
        finally
        {
            await OnGetAsync();
        }
    }
}