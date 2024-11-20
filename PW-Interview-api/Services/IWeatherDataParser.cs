using PW_Interview_api.Models;
using System.Text;

namespace PW_Interview_api.Services
{
    public interface IWeatherDataParser
    {
        WeatherStation Parse(string input);
    }    
}
