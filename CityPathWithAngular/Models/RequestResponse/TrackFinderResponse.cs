namespace CityPathWithAngular.Models.RequestResponse
{
    public class TrackFinderResponse
    {
        public double TotalCost { get; set; }
        public string[] NodeNames { get; set; }
        public double[] Costs { get; set; }
    }
}
