namespace CityPathWithAngular.Models
{
    public class GeoCoordinate
    {
        private readonly float latitude;
        private readonly float longitude;

        public float Latitude { get { return latitude; } }
        public float Longitude { get { return longitude; } }

        public GeoCoordinate(float latitude, float longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", Latitude, Longitude);
        }
    }
}