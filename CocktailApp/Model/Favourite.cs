using System;
using System.Collections.Generic;

namespace CocktailApp.Model
{
    public class Favourite
    {
        public string UserEmail { get; set; } = null!;
        public int RecipeId { get; set; }

        public virtual Recipe Recipe { get; set; } = null!;
        public virtual UserLogin UserLogin { get; set; } = null!;
    }
}
