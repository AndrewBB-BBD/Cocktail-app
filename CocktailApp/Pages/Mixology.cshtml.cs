using CocktailApp.Data;
using CocktailApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
namespace CocktailApp.Pages;
 
public class MixologyModel : PageModel
{
    private readonly cocktailDBContext _cocktailDBContext;
    public MixologyModel(cocktailDBContext cocktailDBContext)
    {
        _cocktailDBContext = cocktailDBContext;
    }

    public List<Category> categoriesList = new List<Category>();
    public List<Ingredient> ingredientsList = new List<Ingredient>();
    public List<Ingredient> selectedIngredients = new List<Ingredient>();
    public bool IsChecked { get; set; }
 
    public async Task<IActionResult> OnGetAsync()
    {
        categoriesList = await _cocktailDBContext.Categories.ToListAsync();
        ingredientsList = await _cocktailDBContext.Ingredients.ToListAsync();
        return Page();
    }

//method called when "find coctail" button clicked
     public IActionResult OnPost()
    {
        //gets a string that represents all the check boxes and their values
        string results = Request.Form["isChecked"];
        //redirect to page with string
        return RedirectToPage("./MixResult", results);
    }

}