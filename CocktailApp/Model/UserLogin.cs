using System;
using System.Collections.Generic;

namespace CocktailApp.Model
{
    public partial class UserLogin
    {
        public UserLogin()
        {
            Ratings = new HashSet<Rating>();
            Recipes = new HashSet<Recipe>();
        }

        public string UserEmail { get; set; } = null!;
        public string Username { get; set; } = null!;
        public byte[] UserPassword { get; set; } = null!;
        public Guid Salt { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }

        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}
