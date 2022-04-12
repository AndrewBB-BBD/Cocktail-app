using System;
using System.Collections.Generic;

namespace CocktailApp.Model
{
    public partial class Difficulty
    {
        public Difficulty()
        {
            Recipes = new HashSet<Recipe>();
        }

        public int DifficultyId { get; set; }
        public string DifficultyName { get; set; } = null!;

        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}
