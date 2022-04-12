using CocktailApp.Data;
using CocktailApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
namespace CocktailApp.Pages;
 
public class DiscoverModel : PageModel
{
    private readonly cocktailDBContext _cocktailDBContext;
    public DiscoverModel(cocktailDBContext cocktailDBContext)
    {
        _cocktailDBContext = cocktailDBContext;
    }
 
    public List<Recipe> recipesList = new List<Recipe>();

    // filter options
    public List<RecipeType> recipeTypes = new List<RecipeType>();
    public List<FlavourProfile> flavourProfiles = new List<FlavourProfile>();
    public List<Difficulty> difficulties = new List<Difficulty>();

    public List<bool> selectedTypes = new List<bool>();
    public string req;
 
    public async Task<IActionResult> OnGet()
    {
        loadRecipes();
        return Page();
    }

    public IActionResult OnGetViewRecipe(int ID, string URL) {
        return RedirectToPage("Recipe", new {recipeID = ID});
    }

    public void OnPost() {
        req = Request.Form["recipeType"];
        loadRecipes(int.Parse(req));
    }

    public void loadRecipes(int type = 0) {
        recipesList =  _cocktailDBContext.Recipes.ToList();

        if (!type.Equals(0)) {
            recipesList = recipesList.Where(r => r.TypeId == type).ToList();
        }

        foreach (var recipe in recipesList) {
            int start = recipe.RecipeImage.IndexOf("/file/d/") + 8;
            int end = recipe.RecipeImage.IndexOf("/view?usp=sharing");
            string imageURL = "https://drive.google.com/uc?id=" + recipe.RecipeImage.Substring(start, end - start);
            recipe.RecipeImage = imageURL;
        }

        recipeTypes =  _cocktailDBContext.RecipeTypes.ToList();
        flavourProfiles =  _cocktailDBContext.FlavourProfiles.ToList();
        difficulties =  _cocktailDBContext.Difficulties.ToList();

        foreach (var t in recipeTypes) {
            selectedTypes.Add(false);
        }
    }
}