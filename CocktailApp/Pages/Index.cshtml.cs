using CocktailApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Text;

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
    public List<RecipeType> recipeTypeList = new List<RecipeType>();

    public void OnGet()
    {
        recipesList = _cocktailDBContext.Recipes.ToList();
        ratingList = _cocktailDBContext.Ratings.ToList();
        recipeTypeList = _cocktailDBContext.RecipeTypes.ToList();
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

        Dictionary<Recipe, Double> recipeAvrg = new Dictionary<Recipe, Double>();

        foreach (var recipe in recipesList)
        {
            var ratingList = recipe.Ratings.ToList();
            if(ratingList != null && ratingList.Count != 0) {
                var averageRating = 0.0;
                foreach (var rating in ratingList)
                {
                    averageRating += (double)rating.NumStars;
                }
                averageRating /= ratingList.Count;

                recipeAvrg.Add(recipe, averageRating);
            }       
        }

        foreach (KeyValuePair<Recipe, Double> recipe in recipeAvrg.OrderByDescending(key => key.Value))
        {
            if(popuarList.Count >= 15)
            {
                break;
            }

            if(recipe.Value >= 4)
            {
                popuarList.Add(recipe.Key);
            }
        }
        return popuarList;
    }

    public Recipe GetFeatured()
    {
        Random rnd = new Random();

        String path = AppDomain.CurrentDomain.BaseDirectory + @"OfTheDay";

        int temprand = rnd.Next(0, popuarList.Count());

        while(System.IO.File.ReadAllText(path).Length != 0 && int.Parse(System.IO.File.ReadAllText(path)) == temprand)
        {
            temprand = rnd.Next(1, popuarList.Count());
        }

        System.IO.File.WriteAllText(path, temprand.ToString());

        var tempRecipe = new Recipe();

        tempRecipe = popuarList[temprand];

        //tempRecipe.RecipeId = int.Parse(System.IO.File.ReadAllText(path));

        featuredRecipe = _cocktailDBContext.Recipes.Find(tempRecipe.RecipeId);

        return featuredRecipe;

    }
}
