using CocktailApp.Data;
using CocktailApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
namespace CocktailApp.Pages;


public class RecipeModel : PageModel
{
    private readonly cocktailDBContext _cocktailDBContext;
    public RecipeModel(cocktailDBContext cocktailDBContext)
    {
        _cocktailDBContext = cocktailDBContext;
    }

    // Warning wants us to make "recipe" and "directions" nullable - Do we want this? What happens if we don't do this?
    public Recipe? recipe;
    // type; flavour; difficulty
    public List<string> recipeTags = new List<string>();
    public List<IngredientMeasurement> ingredients = new List<IngredientMeasurement>();
    public List<string>? directions;
    public List<Favourite>? favourites;

    // Recipe to be added to favourites
    private Favourite addedFavourite = new Favourite();

    // Recipe to delete from favourites
    private Favourite deletedFavourite = new Favourite();

    // Temporary - remember to delete:
    private string currentUserEmail = "test@test.com";

    public async Task<IActionResult> OnGetAsync(int recipeID)
    {
        // get recipe and tags
        recipe = await _cocktailDBContext.Recipes.Where(r => r.RecipeId == recipeID).FirstAsync();
        recipeTags.Add(_cocktailDBContext.RecipeTypes.Where(t => t.TypeId == recipe.TypeId).First().TypeName);
        recipeTags.Add(_cocktailDBContext.FlavourProfiles.Where(t => t.FlavourId == recipe.FlavourId).First().FlavourName);
        recipeTags.Add(_cocktailDBContext.Difficulties.Where(t => t.DifficultyId == recipe.DifficultyId).First().DifficultyName);

        // get recipe ingredients with measurements
        ingredients = await _cocktailDBContext.IngredientMeasurements.Where(i => i.RecipeId == recipeID).ToListAsync();
        foreach (IngredientMeasurement ing in ingredients)
        {
            ing.Ingredient = _cocktailDBContext.Ingredients.Where(i => i.IngredientId == ing.IngredientId).First();
            ing.Measurement = _cocktailDBContext.Measurements.Where(m => m.MeasurementId == ing.MeasurementId).First();
        }

        // put directions into list
        directions = recipe.RecipeMethod.Split('.').ToList();
        directions.RemoveAt(directions.Count - 1);

        if (recipe.RecipeImage is not null)
        {
            // image URL
            int start = recipe.RecipeImage.IndexOf("/file/d/") + 8;
            int end = recipe.RecipeImage.IndexOf("/view?usp=sharing");
            string imageURL = "https://drive.google.com/uc?id=" + recipe.RecipeImage.Substring(start, end - start);
            recipe.RecipeImage = imageURL;
        }

        favourites = await _cocktailDBContext.Favourites.ToListAsync();

        return Page();
    }

    // ADD TO FAVOURITES
    public async Task<IActionResult> OnPostAsync()
    {
        string recipeIdStrng = Request.Query["recipeID"];
        int recipeID = Int32.Parse(recipeIdStrng);
        addedFavourite.RecipeId = recipeID;
        addedFavourite.UserEmail = currentUserEmail;

        _cocktailDBContext.Favourites.Add(addedFavourite);
        await _cocktailDBContext.SaveChangesAsync();
        await OnGetAsync(recipeID);
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteFromFavourites(int id)
    {
        // Baby warning: If you click "Remove from favourites" it works. If you navigate to a new page like "Favourites" or "Discover" and then click back you get a "Confirm Form Resubmission" error.
        deletedFavourite.RecipeId = id;
        deletedFavourite.UserEmail = currentUserEmail;

        if (_cocktailDBContext.Favourites.Where(r => r.RecipeId == deletedFavourite.RecipeId) is not null)
        {
            _cocktailDBContext.Favourites.Remove(deletedFavourite);
            await _cocktailDBContext.SaveChangesAsync();
            await OnGetAsync(id);
            return Page();
        }

        else return Page();
    }
}