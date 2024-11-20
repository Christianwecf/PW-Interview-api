namespace PW_Interview_api.Models
{
    public class WeatherStation
    {
        public string StationId { get; set; }
        public string Location { get; set; }
        public string Temperature { get; set; }
        public ReadingModel Readings { get; set; }
        public string Altitude { get; set; }

        public WeatherStation(string stationId, string location, string temperature, ReadingModel readings, string altitude)
        {
            StationId = stationId;
            Location = location;
            Temperature = temperature;
            Readings = readings;
            Altitude = altitude;
        }
    }
}
