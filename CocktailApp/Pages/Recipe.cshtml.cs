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

    public Recipe recipe;
    // type; flavour; difficulty
    public List<string> recipeTags = new List<string>();
    public List<IngredientMeasurement> ingredients = new List<IngredientMeasurement>();
    public List<string> directions;
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

        // image URL
        int start = recipe.RecipeImage.IndexOf("/file/d/") + 8;
        int end = recipe.RecipeImage.IndexOf("/view?usp=sharing");
        string imageURL = "https://drive.google.com/uc?id=" + recipe.RecipeImage.Substring(start, end - start);
        recipe.RecipeImage = imageURL;
        return Page();
    }
}