using System.Collections.Generic;

namespace CityPathWithAngular.Repositories
{
    public class PlaceDetails
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<Path> Paths { get; set; }
    }
}