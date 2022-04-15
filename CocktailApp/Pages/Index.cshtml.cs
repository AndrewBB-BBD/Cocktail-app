using CocktailApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CocktailApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly cocktailDBContext _cocktailDBContext;

    public IndexModel(ILogger<IndexModel> logger, cocktailDBContext cocktailDBContext)
    {
        _logger = logger;
        _cocktailDBContext = cocktailDBContext;
    }

    public List<Recipe> recipesList = new List<Recipe>();
    public List<Recipe> popuarList = new List<Recipe>();
    public List<Rating> ratingList = new List<Rating>();
    public Recipe featuredRecipe = new Recipe();
    public void OnGet()
    {
        recipesList = _cocktailDBContext.Recipes.ToList();
        ratingList = _cocktailDBContext.Ratings.ToList();
        GetPopularRecipes();
        GetFeatured();
    }

    public List<Recipe> GetPopularRecipes()
    {
        foreach (var recipe in recipesList)
        {
            int start = recipe.RecipeImage.IndexOf("/file/d/") + 8;
            int end = recipe.RecipeImage.IndexOf("/view?usp=sharing");
            string imageURL = "https://drive.google.com/uc?id=" + recipe.RecipeImage.Substring(start, end - start);
            recipe.RecipeImage = imageURL;
        }

        foreach (var rating in ratingList)
        {
            if (rating.NumStars >= 4 && popuarList.Count < 15)
            {
                popuarList.Add(_cocktailDBContext.Recipes.Find(rating.RecipeId));
            }
        }
        return popuarList;
    }

    public Recipe GetFeatured()
    {

        Random rnd = new Random();
        int intRecRandom = rnd.Next(1, popuarList.Count());  // creates a number between 1 and number of recipes

        Console.WriteLine(intRecRandom);

        var tempRecipe = new Recipe();

        tempRecipe.RecipeId = intRecRandom;

        featuredRecipe = _cocktailDBContext.Recipes.Find(tempRecipe.RecipeId);

        return featuredRecipe;

    }
}
