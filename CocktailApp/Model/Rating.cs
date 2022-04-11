using System;
using System.Collections.Generic;

namespace CocktailApp.Model
{
    public partial class Rating
    {
        public string UserEmail { get; set; } = null!;
        public int RecipeId { get; set; }
        public decimal? NumStars { get; set; }
        public string? RatingComment { get; set; }

        public virtual Recipe Recipe { get; set; } = null!;
        public virtual UserLogin UserEmailNavigation { get; set; } = null!;
    }
}
