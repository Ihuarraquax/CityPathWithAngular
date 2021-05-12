using System;

namespace CityPathWithAngular.Models
{
    public class Place
    {

        public long Id { get; set; }
        public string Name { get; set; }

        public Place()
        {
            
        }
        public Place(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}