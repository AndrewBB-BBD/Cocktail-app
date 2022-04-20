using System;
using System.Collections.Generic;

namespace CocktailApp.Model
{
    public partial class Recipe
    {
        public Recipe()
        {
            IngredientMeasurements = new HashSet<IngredientMeasurement>();
            Ratings = new HashSet<Rating>();
            UserEmails = new HashSet<UserLogin>();
        }

        public int RecipeId { get; set; }
        public int FlavourId { get; set; }
        public int DifficultyId { get; set; }
        public int TypeId { get; set; }
        public string RecipeName { get; set; } = null!;
        public string? RecipeDescription { get; set; }
        public string RecipeMethod { get; set; } = null!;
        public int RecipeTime { get; set; }
        public string RecipeImage { get; set; } = null!;
        public bool ContaintsAlcohol { get; set; }

        public virtual Difficulty Difficulty { get; set; } = null!;
        public virtual FlavourProfile Flavour { get; set; } = null!;
        public virtual RecipeType Type { get; set; } = null!;
        public virtual ICollection<IngredientMeasurement> IngredientMeasurements { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }

        public virtual ICollection<UserLogin> UserEmails { get; set; }
        public virtual ICollection<Favourite>? Favourites { get; set; }
    }
}
