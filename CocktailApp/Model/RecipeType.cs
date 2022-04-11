using System;
using System.Collections.Generic;

namespace CocktailApp.Model
{
    public partial class RecipeType
    {
        public RecipeType()
        {
            Recipes = new HashSet<Recipe>();
        }

        public int TypeId { get; set; }
        public string TypeName { get; set; } = null!;

        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}
