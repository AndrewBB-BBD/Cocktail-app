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
    public DiscoverModel(cocktailDBContext cocktailDBContext)
    {
        _cocktailDBContext = cocktailDBContext;
    }

    public List<Recipe> recipesList = new List<Recipe>();

    // filter options
    public List<RecipeType> recipeTypes = new List<RecipeType>();
    public List<FlavourProfile> flavourProfiles = new List<FlavourProfile>();
    public List<Difficulty> difficulties = new List<Difficulty>();

    // Recipe to be added to favourites
    private Favourite addedFavourite = new Favourite();

    // Recipe to delete from favourites
    private Favourite deletedFavourite = new Favourite();

    // Temporary - remember to delete:
    private string currentUserEmail = "test@test.com";

    public int minTime;
    public int maxTime;

    public IActionResult OnGet()
    {
        loadRecipes(new List<string>(), new List<string>(), new List<string>());
        minTime = _cocktailDBContext.Recipes.Select(r => r.RecipeTime).Min();
        maxTime = _cocktailDBContext.Recipes.Select(r => r.RecipeTime).Max();
        return Page();
    }

    public IActionResult OnGetViewRecipe(int ID, string URL)
    {
        return RedirectToPage("Recipe", new { recipeID = ID });
    }

    public void OnPost()
    {
        string reqData;

        // recipe types
        reqData = Request.Form["recipeType"];
        List<string> selectedRecipeTypes = new List<string>();
        if (reqData != null)
            selectedRecipeTypes = reqData.Split(',').ToList();

        // flavour profiles
        reqData = Request.Form["flavourProfile"];
        List<string> selectedFlavourProfiles = new List<string>();
        if (reqData != null)
            selectedFlavourProfiles = reqData.Split(',').ToList();

        // difficulty
        reqData = Request.Form["difficulty"];
        List<string> selectedDifficulties = new List<string>();
        if (reqData != null)
            selectedDifficulties = reqData.Split(',').ToList();

        loadRecipes(selectedRecipeTypes, selectedFlavourProfiles, selectedDifficulties);
    }


    public void loadRecipes(List<string> selectedRecipeTypes, List<string> selectedFlavourProfiles, List<string> selectedDifficulties)
    {
        recipesList = _cocktailDBContext.Recipes.ToList();

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

        foreach (var recipe in recipesList)
        {
            int start = recipe.RecipeImage.IndexOf("/file/d/") + 8;
            int end = recipe.RecipeImage.IndexOf("/view?usp=sharing");
            string imageURL = "https://drive.google.com/uc?id=" + recipe.RecipeImage.Substring(start, end - start);
            recipe.RecipeImage = imageURL;
        }

        recipeTypes = _cocktailDBContext.RecipeTypes.ToList();
        flavourProfiles = _cocktailDBContext.FlavourProfiles.ToList();
        difficulties = _cocktailDBContext.Difficulties.ToList();
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