using CocktailApp.Data;
using CocktailApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
namespace CocktailApp.Pages;
using System.Data.SqlClient;

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
        favouritesList = _cocktailDBContext.Favourites.ToList();
    }

    // public List<Recipe> recipesList = new List<Recipe>();
    private List<Favourite> favouritesList = new List<Favourite>();

    // filter options
    public List<RecipeType> recipeTypes = new List<RecipeType>();
    public List<FlavourProfile> flavourProfiles = new List<FlavourProfile>();
    public List<Difficulty> difficulties = new List<Difficulty>();

    // selected filters
    public List<string> selectedRecipeTypes = new List<string>();
    public List<string> selectedFlavourProfiles = new List<string>();
    public List<string> selectedDifficulties = new List<string>();

    // Recipe to be added to favourites
    private Favourite addedFavourite = new Favourite();
    // Recipe to delete from favourites
    private Favourite deletedFavourite = new Favourite();
    public List<Recipe> userFavouritesList = new List<Recipe>();

    // Temporary - remember to delete:
    private string currentUserEmail = "test@test.com";
    // private string currentUserEmail = "test1@test.com";
    // private string currentUserEmail = "test2@test.com";
    // private string currentUserEmail = "test3@test.com";
    // private string currentUserEmail = "test4@test.com";

    public int minTime;
    public int maxTime;
    public string searchResults = "";

    public IActionResult OnGet()
    {
        loadRecipesAndInfo(recipesList);

        // Get user's favourite recipes list
        foreach (Favourite entry in favouritesList)
        {
            if (entry.UserEmail == currentUserEmail)
            {
                userFavouritesList.Add(_cocktailDBContext.Recipes.Where(r => r.RecipeId == entry.RecipeId).First());
            }
        }
        return Page();
    }

    // redirects the user to a recipe page when they select a recipe card
    public IActionResult OnGetViewRecipe(int ID, string URL)
    {
        return RedirectToPage("Recipe", new { recipeID = ID });
    }

    // executes when the user searches or filters recipes and retrieves the request data
    public void OnPost()
    {
        string reset = Request.Form["reset"];
        string searchTerm = Request.Form["searchTerm"];

        if (reset != null)
        {    // reset 
            loadRecipesAndInfo(_cocktailDBContext.Recipes.ToList());
        }
        else if (!string.IsNullOrEmpty(searchTerm))
        {     // search
            loadRecipesAndInfo(_cocktailDBContext.Recipes.ToList());
            findRecipes(searchTerm);
        }
        else
        {    // filter
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
    public void loadRecipesAndInfo(List<Recipe> recipes)
    {
        recipesList = recipes;
        minTime = recipesList.Select(r => r.RecipeTime).Min();
        maxTime = recipesList.Select(r => r.RecipeTime).Max();

        foreach (var recipe in recipesList)
        {
            int start = recipe.RecipeImage.IndexOf("/file/d/") + 8;
            int end = recipe.RecipeImage.IndexOf("/view?usp=sharing");
            string imageURL = "https://drive.google.com/uc?id=" + recipe.RecipeImage.Substring(start, end - start);
            recipe.RecipeImage = imageURL;
        }
    }

    // filters the recipes displayed according to the user's selection
    public void filterRecipes(List<string> selectedRecipeTypes, List<string> selectedFlavourProfiles, List<string> selectedDifficulties, int time, List<Recipe> recipes)
    {

        if (recipes.Count() > 0)
            loadRecipesAndInfo(recipes);
        else
            loadRecipesAndInfo(_cocktailDBContext.Recipes.ToList());

        if (selectedRecipeTypes.Count > 0)
        {
            recipesList = recipesList.Where(r => selectedRecipeTypes.Select(int.Parse).ToList().Contains(r.TypeId)
            ).ToList();
        }

        if (selectedFlavourProfiles.Count > 0)
        {
            recipesList = recipesList.Where(r => selectedFlavourProfiles.Select(int.Parse).ToList().Contains(r.FlavourId)
            ).ToList();
        }

        if (selectedDifficulties.Count > 0)
        {
            recipesList = recipesList.Where(r => selectedDifficulties.Select(int.Parse).ToList().Contains(r.DifficultyId)
            ).ToList();
        }

        if (time > 0)
            recipesList = recipesList.Where(r => r.RecipeTime <= time).ToList();

        searchResults = recipesList.Count().ToString() + " recipes found.";
    }

    // searches for a recipe with the specified search term in its name
    public void findRecipes(string searchTerm)
    {
        recipesList = recipesList.Where(r => r.RecipeName.ToUpper().Contains(searchTerm.ToUpper())).ToList();
        searchResults = recipesList.Count().ToString() + " results for '" + searchTerm + "'.";
    }



    // ADD TO FAVOURITES METHOD
    public async Task<IActionResult> OnPostFavouriteAsync(int recipeID)
    {
        addedFavourite.RecipeId = recipeID;
        addedFavourite.UserEmail = currentUserEmail;

        try
        {
            _cocktailDBContext.Favourites.Add(addedFavourite);
            await _cocktailDBContext.SaveChangesAsync();
            return Page();
        }
        catch (SqlException e)
        {
            // This exception occurs if you attempt to add the same cocktail to favourites twice.
            Console.WriteLine("SqlException caught");
            // Console.WriteLine("\nMessage ---\n{0}", e.Message);
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
            OnGet();
        }
    }

    // REMOVE FROM FAVOURITES 
    public async Task<IActionResult> OnPostDeleteFromFavourites(int recipeID)
    {
        // Baby warning: If you click "Remove from favourites" it works. If you navigate to a new page like "Favourites" or "Discover" and then click back you get a "Confirm Form Resubmission" error.
        deletedFavourite.RecipeId = recipeID;
        deletedFavourite.UserEmail = currentUserEmail;

        try
        {
            _cocktailDBContext.Favourites.Remove(deletedFavourite);
            await _cocktailDBContext.SaveChangesAsync();
            return Page();
        }
        catch (DbUpdateConcurrencyException)
        {
            Console.WriteLine("Cocktail does not exist in Favourites table in DB");

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
            OnGet();
        }


    }

}