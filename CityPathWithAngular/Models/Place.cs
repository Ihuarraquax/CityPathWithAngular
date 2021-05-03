using System;

namespace CityPathWithAngular.Models
{
    public class Place
    {

        public long Id { get; set; }
        public string Name { get; set; }
        public GeoCoordinate Coordinate { get; set; }

        public Place()
        {
            
        }
        public Place(long id, string name, float latitude, float longitude)
        {
            Id = id;
            Name = name;
            Coordinate = new GeoCoordinate(latitude, longitude);
        }
    }
}