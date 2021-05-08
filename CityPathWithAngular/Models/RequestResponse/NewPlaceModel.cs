using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CityPathWithAngular.Models.RequestResponse
{
    public class NewPlaceModel
    {
        [Required]
        public string Name { get; set; }
        public List<SasiadModel> Sasiads { get; set; }
    }

    public class SasiadModel
    {
        public string Name { get; set; }
        public double Distance { get; set; }
    }
}
