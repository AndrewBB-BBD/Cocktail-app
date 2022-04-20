using CocktailApp.Data;
using CocktailApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
namespace CocktailApp.Pages;
using System.Data.SqlClient;


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
    public List<Favourite>? favouritesList;
    // List of User's Favourite Recipes
    public List<Recipe> userFavourites = new List<Recipe>();

    // Recipe to be added to / deleted from favourites
    private Favourite addedFavourite = new Favourite();
    private Favourite deletedFavourite = new Favourite();

    // Temporary - remember to delete:
    private string currentUserEmail = "test@test.com";
    // private string currentUserEmail = "test1@test.com";
    // private string currentUserEmail = "test2@test.com";
    // private string currentUserEmail = "test3@test.com";
    // private string currentUserEmail = "test4@test.com";

    public List<Rating> ratings = new List<Rating>();

    public async Task<IActionResult> OnGetAsync(int recipeID)
    {
        favouritesList = await _cocktailDBContext.Favourites.ToListAsync();
        ratings = _cocktailDBContext.Ratings.ToList();

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

            // Sometimes "file/d/" and "/view?usp=sharing", are already omitted then we get an error, hence the write line statements above for testing.
        }

        // Get user's favourite recipes list
        foreach (Favourite entry in favouritesList)
        {
            if (entry.UserEmail == currentUserEmail)
            {
                userFavourites.Add(_cocktailDBContext.Recipes.Where(r => r.RecipeId == entry.RecipeId).First());
            }
        }


        return Page();
    }

    // ADD TO FAVOURITES
    public async Task<IActionResult> OnPostAsync(int recipeID)
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
            // Exception thrown when SQL Server returns warning or error
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
            await OnGetAsync(recipeID);
        }
    }

    // REMOVE FROM FAVOURITES
    public async Task<IActionResult> OnPostDeleteFromFavourites(int recipeID)
    {
        // Baby warning: If you click "Remove from favourites" it works. If you navigate to a new page like "Favourites" or "Discover" and then click back you get a "Confirm Form Resubmission" error.
        deletedFavourite.RecipeId = recipeID;
        deletedFavourite.UserEmail = currentUserEmail;

        // Concurrency conflicts happen when cocktail that isn't in Favourites is removed from favourites. 
        // DbUpdateConcurrencyException thrown
        // Exception thrown by DbContext when it was expected that SaveChanges for an entity would result in a database update but in fact no rows in the database were affected. DB has been concurrently updated such that a concurrency token that was expected to match did not actually match.

        // DOCS NOTES:
        // DB concurrency = situations in which multiple processes or users access or change the same data in a database at the same time
        // Worst case scenario: two or more processes will attempt to make conflicting changes, and only one of them should succeed.

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
            await OnGetAsync(recipeID);
        }
    }
}