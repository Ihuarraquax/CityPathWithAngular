using System;
using System.Collections.Generic;

namespace CityPathWithAngular.Models.RequestResponse
{
    public class TrackFinderResponse
    {
        public double TotalCost { get; set; }
        public List<Object> NodeNames { get; set; }
        public List<Object> Costs { get; set; }
    }
}