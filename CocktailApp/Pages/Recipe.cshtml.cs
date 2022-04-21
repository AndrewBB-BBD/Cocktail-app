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
    public Recipe? recipe;
    public List<string> recipeTags = new List<string>();
    public List<IngredientMeasurement> ingredients = new List<IngredientMeasurement>();
    public List<string>? directions;
    public List<Favourite>? favouritesList;
    public List<Recipe> userFavourites = new List<Recipe>();
    private Favourite addedFavourite = new Favourite();
    private Favourite deletedFavourite = new Favourite();
    private string currentUserEmail = "test@test.com";
    public List<Rating> ratings = new List<Rating>();
    public async Task<IActionResult> OnGetAsync(int recipeID)
    {
        favouritesList = await _cocktailDBContext.Favourites.ToListAsync();
        ratings = _cocktailDBContext.Ratings.ToList();
        recipe = await _cocktailDBContext.Recipes.Where(r => r.RecipeId == recipeID).FirstAsync();
        recipeTags.Add(_cocktailDBContext.RecipeTypes.Where(t => t.TypeId == recipe.TypeId).First().TypeName);
        recipeTags.Add(_cocktailDBContext.FlavourProfiles.Where(t => t.FlavourId == recipe.FlavourId).First().FlavourName);
        recipeTags.Add(_cocktailDBContext.Difficulties.Where(t => t.DifficultyId == recipe.DifficultyId).First().DifficultyName);
        ingredients = await _cocktailDBContext.IngredientMeasurements.Where(i => i.RecipeId == recipeID).ToListAsync();
        foreach (IngredientMeasurement ing in ingredients)
        {
            ing.Ingredient = _cocktailDBContext.Ingredients.Where(i => i.IngredientId == ing.IngredientId).First();
            ing.Measurement = _cocktailDBContext.Measurements.Where(m => m.MeasurementId == ing.MeasurementId).First();
        }
        directions = recipe.RecipeMethod.Split('.').ToList();
        directions.RemoveAt(directions.Count - 1);

        if (recipe.RecipeImage is not null)
        {
            int start = recipe.RecipeImage.IndexOf("/file/d/") + 8;
            int end = recipe.RecipeImage.IndexOf("/view?usp=sharing");
            string imageURL = "https://drive.google.com/uc?id=" + recipe.RecipeImage.Substring(start, end - start);
            recipe.RecipeImage = imageURL;
        }
        foreach (Favourite entry in favouritesList)
        {
            if (entry.UserEmail == currentUserEmail)
            {
                userFavourites.Add(_cocktailDBContext.Recipes.Where(r => r.RecipeId == entry.RecipeId).First());
            }
        }
        return Page();
    }
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
            Console.WriteLine("SqlException caught");
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
            await OnGetAsync(recipeID);
        }
    }
}