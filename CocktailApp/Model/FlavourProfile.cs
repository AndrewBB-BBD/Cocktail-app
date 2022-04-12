using System;
using System.Collections.Generic;

namespace CocktailApp.Model
{
    public partial class FlavourProfile
    {
        public FlavourProfile()
        {
            Recipes = new HashSet<Recipe>();
        }

        public int FlavourId { get; set; }
        public string FlavourName { get; set; } = null!;

        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}
