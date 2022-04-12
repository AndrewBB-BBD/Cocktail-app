using System;
using System.Collections.Generic;

namespace CocktailApp.Model
{
    public partial class IngredientMeasurement
    {
        public int IngredientId { get; set; }
        public int RecipeId { get; set; }
        public int MeasurementId { get; set; }
        public string MeasurementAmount { get; set; } = null!;

        public virtual Ingredient Ingredient { get; set; } = null!;
        public virtual Measurement Measurement { get; set; } = null!;
        public virtual Recipe Recipe { get; set; } = null!;
    }
}
