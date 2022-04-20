using CocktailApp.Data;
using CocktailApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
namespace CocktailApp.Pages;
 
public class DiscoverModel : PageModel
{
    private readonly cocktailDBContext _cocktailDBContext;
    public List<Recipe> recipesList = new List<Recipe>();

    // sets the DB context and list of recipes as well as the recipe types, flavours, difficulties, and times for the filter menu
    public DiscoverModel(cocktailDBContext cocktailDBContext)
    {
        _cocktailDBContext = cocktailDBContext;
        recipesList = _cocktailDBContext.Recipes.ToList();
        recipeTypes = _cocktailDBContext.RecipeTypes.ToList();
        flavourProfiles = _cocktailDBContext.FlavourProfiles.ToList();
        difficulties = _cocktailDBContext.Difficulties.ToList();
    }
 

    // filter options
    public List<RecipeType> recipeTypes = new List<RecipeType>();
    public List<FlavourProfile> flavourProfiles = new List<FlavourProfile>();
    public List<Difficulty> difficulties = new List<Difficulty>();

    // selected filters
    public List<string> selectedRecipeTypes = new List<string>();
    public List<string> selectedFlavourProfiles = new List<string>();
    public List<string> selectedDifficulties = new List<string>();

    public int minTime;
    public int maxTime;
    public string searchResults = "";
 
    public IActionResult OnGet()
    {
        loadRecipesAndInfo(recipesList);
        return Page();
    }

    // redirects the user to a recipe page when they select a recipe card
    public IActionResult OnGetViewRecipe(int ID, string URL) {
        return RedirectToPage("Recipe", new {recipeID = ID});
    }

    // executes when the user searches or filters recipes and retrieves the request data
    public void OnPost() {
        string reset = Request.Form["reset"];
        string searchTerm = Request.Form["searchTerm"];

        if (reset != null) {    // reset 
            loadRecipesAndInfo(_cocktailDBContext.Recipes.ToList());
        } else if (!string.IsNullOrEmpty(searchTerm)) {     // search
            loadRecipesAndInfo(_cocktailDBContext.Recipes.ToList());
            findRecipes(searchTerm);
        } else {    // filter
            string reqData;

            // recipe types
            reqData = Request.Form["recipeType"];
            if (reqData != null)   
                selectedRecipeTypes = reqData.Split(',').ToList();

            // flavour profiles
            reqData = Request.Form["flavourProfile"];
            if (reqData != null)   
                selectedFlavourProfiles = reqData.Split(',').ToList();

            // difficulty
            reqData = Request.Form["difficulty"];
            if (reqData != null)   
                selectedDifficulties = reqData.Split(',').ToList();

            // time
            reqData = Request.Form["time"];
            if (reqData != null)
                filterRecipes(selectedRecipeTypes, selectedFlavourProfiles, selectedDifficulties, int.Parse(reqData), recipesList);
            else    
                filterRecipes(selectedRecipeTypes, selectedFlavourProfiles, selectedDifficulties, 0, recipesList);
        }
    }

    // loads all recipes and their data
    public void loadRecipesAndInfo(List<Recipe> recipes) {
        recipesList =  recipes;
        minTime = recipesList.Select(r => r.RecipeTime).Min();
        maxTime = recipesList.Select(r => r.RecipeTime).Max();

        foreach (var recipe in recipesList) {
            int start = recipe.RecipeImage.IndexOf("/file/d/") + 8;
            int end = recipe.RecipeImage.IndexOf("/view?usp=sharing");
            string imageURL = "https://drive.google.com/uc?id=" + recipe.RecipeImage.Substring(start, end - start);
            recipe.RecipeImage = imageURL;
        }
    }

    // filters the recipes displayed according to the user's selection
    public void filterRecipes(List<string> selectedRecipeTypes, List<string> selectedFlavourProfiles, List<string> selectedDifficulties, int time, List<Recipe> recipes) {
        
        if (recipes.Count() > 0)
            loadRecipesAndInfo(recipes);
        else
            loadRecipesAndInfo(_cocktailDBContext.Recipes.ToList());

        if (selectedRecipeTypes.Count > 0) {
            recipesList = recipesList.Where(r => selectedRecipeTypes.Select(int.Parse).ToList().Contains(r.TypeId)
            ).ToList();
        } 

        if (selectedFlavourProfiles.Count > 0) {
            recipesList = recipesList.Where(r => selectedFlavourProfiles.Select(int.Parse).ToList().Contains(r.FlavourId)
            ).ToList();
        } 

        if (selectedDifficulties.Count > 0) {
            recipesList = recipesList.Where(r => selectedDifficulties.Select(int.Parse).ToList().Contains(r.DifficultyId)
            ).ToList();
        } 

        if (time > 0)
            recipesList = recipesList.Where(r => r.RecipeTime <= time).ToList();

        searchResults = recipesList.Count().ToString() + " recipes found.";
    }

    // searches for a recipe with the specified search term in its name
    public void findRecipes(string searchTerm) {
        recipesList =  recipesList.Where(r => r.RecipeName.ToUpper().Contains(searchTerm.ToUpper())).ToList();
        searchResults = recipesList.Count().ToString() + " results for '" + searchTerm + "'.";
    }

}