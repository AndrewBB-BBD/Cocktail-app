using CocktailApp.Data;
using CocktailApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
namespace CocktailApp.Pages;
 
public class MixResultModel : PageModel
{
    private readonly cocktailDBContext _cocktailDBContext;
    public MixResultModel(cocktailDBContext cocktailDBContext)
    {
        _cocktailDBContext = cocktailDBContext;
    }
    public IEnumerable<Recipe> chosenRecipesList;
    public List<Recipe> fullRecipesList = new List<Recipe>();
    public List<Recipe> chosenRecipes = new List<Recipe>();
    public List<int> userSelectedIds = new List<int>();
    public List<Ingredient> Selected = new List<Ingredient>();
    public List<IngredientMeasurement> fullIngredMeasList = new List<IngredientMeasurement>();
    public List<int> recIds = new List<int>();
    public List<Rating> ratings = new List<Rating>();
    public string noIngredientSelectedMsg = "";
    public async Task<IActionResult> OnGetAsync(List<int> userSelectedIds)
    {
        userSelectedIds = userSelectedIds;
        if(!userSelectedIds.Any()) {
            noIngredientSelectedMsg = "You have not selected any ingredients :(";
        }
        fullIngredMeasList = await _cocktailDBContext.IngredientMeasurements.ToListAsync();
        fullRecipesList = await _cocktailDBContext.Recipes.ToListAsync();
        ratings = _cocktailDBContext.Ratings.ToList();
        foreach(var item in userSelectedIds) {
            chosenRecipesList = from recipe in fullRecipesList
                                join measure in fullIngredMeasList on recipe.RecipeId equals measure.RecipeId into recipeGroup
                                from recGroup in recipeGroup
                                where recGroup.IngredientId == item
                                select recipe;
            foreach(var itit in chosenRecipesList) {
                chosenRecipes.Add(itit);
            }
        }
        loadRecipes(chosenRecipes);
        return Page();
    }
    public IActionResult OnGetViewRecipe(int ID, string URL) {
        return RedirectToPage("Recipe", new {recipeID = ID});
    }
    public void loadRecipes(List<Recipe> chosenRecipes) {
        foreach (var recipe in chosenRecipes) {
            int start = recipe.RecipeImage.IndexOf("/file/d/") + 8;
            int end = recipe.RecipeImage.IndexOf("/view?usp=sharing");
            string imageURL = "https://drive.google.com/uc?id=" + recipe.RecipeImage.Substring(start, end - start);
            recipe.RecipeImage = imageURL;
        }
    }
}