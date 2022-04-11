using System;
using System.Collections.Generic;

namespace CocktailApp.Model
{
    public partial class Ingredient
    {
        public Ingredient()
        {
            IngredientMeasurements = new HashSet<IngredientMeasurement>();
        }

        public int IngredientId { get; set; }
        public int CategoryId { get; set; }
        public string? IngredientName { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<IngredientMeasurement> IngredientMeasurements { get; set; }
    }
}
