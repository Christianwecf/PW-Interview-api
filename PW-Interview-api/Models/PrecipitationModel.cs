namespace PW_Interview_api.Models
{
    public class PrecipitationModel
    {
        public string Amount { get; set; }
        public string Type { get; set; }
        public string Intensity { get; set; }

        public PrecipitationModel(string amount, string type, string intensity)
        {
            Amount = amount;
            Type = type;
            Intensity = intensity;
        }
    }
}
