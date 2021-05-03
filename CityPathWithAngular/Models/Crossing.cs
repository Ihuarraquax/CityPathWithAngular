using System;

namespace CityPathWithAngular.Models
{
    public class Crossing
    {
        public Guid Id { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public GeoCoordinate Coordinate { get; set; }
    }
}