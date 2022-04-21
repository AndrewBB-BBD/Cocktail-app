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
    public DiscoverModel(cocktailDBContext cocktailDBContext)
    {
        _cocktailDBContext = cocktailDBContext;
        recipesList = _cocktailDBContext.Recipes.ToList();
        recipeTypes = _cocktailDBContext.RecipeTypes.ToList();
        flavourProfiles = _cocktailDBContext.FlavourProfiles.ToList();
        difficulties = _cocktailDBContext.Difficulties.ToList();

        ratings = _cocktailDBContext.Ratings.ToList();
    }
    private List<Favourite> favouritesList = new List<Favourite>();
    public List<RecipeType> recipeTypes = new List<RecipeType>();
    public List<FlavourProfile> flavourProfiles = new List<FlavourProfile>();
    public List<Difficulty> difficulties = new List<Difficulty>();
    public List<string> selectedRecipeTypes = new List<string>();
    public List<string> selectedFlavourProfiles = new List<string>();
    public List<string> selectedDifficulties = new List<string>();
    private Favourite addedFavourite = new Favourite();
    private Favourite deletedFavourite = new Favourite();
    public List<Recipe> userFavouritesList = new List<Recipe>();
    private string currentUserEmail = "test@test.com";
    public int minTime;
    public int maxTime;
    public string searchResults = "";

    public List<Rating> ratings = new List<Rating>();

    public async Task<IActionResult> OnGetAsync()
    {
        loadRecipesAndInfo(recipesList);
        favouritesList = await _cocktailDBContext.Favourites.ToListAsync();
        foreach (Favourite entry in favouritesList)
        {
            if (entry.UserEmail == currentUserEmail)
            {
                userFavouritesList.Add(_cocktailDBContext.Recipes.Where(r => r.RecipeId == entry.RecipeId).First());
            }
        }
        return Page();
    }
    public IActionResult OnGetViewRecipe(int ID, string URL)
    {
        return RedirectToPage("Recipe", new { recipeID = ID });
    }
    public void OnPost()
    {
        string reset = Request.Form["reset"];
        string searchTerm = Request.Form["searchTerm"];

        if (reset != null)
        {
            loadRecipesAndInfo(_cocktailDBContext.Recipes.ToList());
        }
        else if (!string.IsNullOrEmpty(searchTerm))
        {
            loadRecipesAndInfo(_cocktailDBContext.Recipes.ToList());
            findRecipes(searchTerm);
        }
        else
        {
            string reqData;
            reqData = Request.Form["recipeType"];
            if (reqData != null)
                selectedRecipeTypes = reqData.Split(',').ToList();
            reqData = Request.Form["flavourProfile"];
            if (reqData != null)
                selectedFlavourProfiles = reqData.Split(',').ToList();
            reqData = Request.Form["difficulty"];
            if (reqData != null)
                selectedDifficulties = reqData.Split(',').ToList();
            reqData = Request.Form["time"];
            if (reqData != null)
                filterRecipes(selectedRecipeTypes, selectedFlavourProfiles, selectedDifficulties, int.Parse(reqData), recipesList);
            else
                filterRecipes(selectedRecipeTypes, selectedFlavourProfiles, selectedDifficulties, 0, recipesList);
        }
    }
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
    public void findRecipes(string searchTerm)
    {
        recipesList = recipesList.Where(r => r.RecipeName.ToUpper().Contains(searchTerm.ToUpper())).ToList();
        searchResults = recipesList.Count().ToString() + " results for '" + searchTerm + "'.";
    }
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
            Console.WriteLine("SqlException caught", e.Message);
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
            await OnGetAsync();
        }
    }
}