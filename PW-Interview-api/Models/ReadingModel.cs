namespace PW_Interview_api.Models
{
    public class ReadingModel
    {
        public string Timestamp { get; set; }
        public string WindSpeed { get; set; }
        public PrecipitationModel Precipitation { get; set; }

        public ReadingModel(string timestamp, string windSpeed, PrecipitationModel precipitation)
        {
            Timestamp = timestamp;
            WindSpeed = windSpeed;
            Precipitation = precipitation;
        }
    }
}
