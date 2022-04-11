using System;
using System.Collections.Generic;

namespace CocktailApp.Model
{
    public partial class Category
    {
        public Category()
        {
            Ingredients = new HashSet<Ingredient>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;

        public virtual ICollection<Ingredient> Ingredients { get; set; }
    }
}
