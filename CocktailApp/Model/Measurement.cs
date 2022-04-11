using System;
using System.Collections.Generic;

namespace CocktailApp.Model
{
    public partial class Measurement
    {
        public Measurement()
        {
            IngredientMeasurements = new HashSet<IngredientMeasurement>();
        }

        public int MeasurementId { get; set; }
        public string MeasurementName { get; set; } = null!;

        public virtual ICollection<IngredientMeasurement> IngredientMeasurements { get; set; }
    }
}
