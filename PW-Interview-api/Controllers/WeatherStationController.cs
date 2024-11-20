using Microsoft.AspNetCore.Mvc;
using PW_Interview_api.Models;
using PW_Interview_api.Services;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using static PW_Interview_api.Services.WeatherDataParser;

namespace PW_Interview_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherStationController : ControllerBase
    {
        private readonly IWeatherDataParser _parser;
        private readonly IWeatherDataParser _weatherDataParser;

        public WeatherStationController(IWeatherDataParser parser, IWeatherDataParser weatherDataParser)
        {
            _parser = parser;
            _weatherDataParser = weatherDataParser;
        }

        // Endpoint para obtener la estación meteorológica en formato JSON
        [HttpPost("parse-json")]
        public IActionResult ParseJson([FromBody] string jsonInput)
        {
            try
            {
                var weatherStation = _weatherDataParser.Parse(jsonInput);

                // Devolvemos el resultado en formato JSON
                return Ok(weatherStation);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // Endpoint para recibir y devolver los datos en formato texto plano
        [HttpPost("parse-text")]
        public IActionResult ParseText([FromBody] string textInput)
        {
            try
            {
                var weatherStation = _weatherDataParser.Parse(textInput);

                // Imprimir el resultado como texto plano con indentación
                var printer = new WeatherStationPrinter();
                using (var writer = new StringWriter())
                {
                    printer.Print(weatherStation, writer);
                    return Ok(writer.ToString());
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // Endpoint para cargar datos desde un archivo CSV
        [HttpPost("upload/csv")]
        public IActionResult UploadCsvFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var records = new List<WeatherStation>();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var header = reader.ReadLine(); // Lee la cabecera y la omite

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    // Aquí deberías mapear correctamente las columnas a las propiedades de WeatherStation
                    var weatherStation = new WeatherStation(
                        values[0], // StationId
                        values[1], // Location
                        values[2], // Temperature
                        new ReadingModel(values[3], values[4], new PrecipitationModel(values[5], values[6], values[7])), // Readings
                        values[8]  // Altitude
                    );

                    records.Add(weatherStation);
                }
            }

            return Ok(new { message = $"{records.Count} records loaded successfully", data = records });
        }

        // Endpoint para exportar datos a un archivo CSV
        [HttpGet("export/csv")]
        public IActionResult ExportToCsv()
        {
            // Obtener los datos (puedes obtenerlos desde una base de datos o servicio)
            var weatherStations = GetWeatherStations();

            var csv = new StringBuilder();
            csv.AppendLine("StationId,Location,Temperature,Timestamp,WindSpeed,PrecipitationAmount,PrecipitationType,PrecipitationIntensity,Altitude");

            foreach (var station in weatherStations)
            {
                var line = $"{station.StationId},{station.Location},{station.Temperature}," +
                           $"{station.Readings.Timestamp},{station.Readings.WindSpeed}," +
                           $"{station.Readings.Precipitation.Amount},{station.Readings.Precipitation.Type}," +
                           $"{station.Readings.Precipitation.Intensity},{station.Altitude}";

                csv.AppendLine(line);
            }

            var fileName = "weather_data.csv";
            var fileBytes = Encoding.UTF8.GetBytes(csv.ToString());

            return File(fileBytes, "text/csv", fileName);
        }       

        // Método para obtener algunas estaciones meteorológicas (simulado)
        private List<WeatherStation> GetWeatherStations()
        {
            return new List<WeatherStation>
            {
                new WeatherStation("WS001", "Seattle", "72F", new ReadingModel("2024-03-14T10:00:00", "15mph", new PrecipitationModel("0.5in", "rain", "moderate")), "520ft"),
                new WeatherStation("WS002", "Portland", "65F", new ReadingModel("2024-03-14T10:05:00", "12mph", new PrecipitationModel("0.2in", "snow", "light")), "800ft")
            };
        }
    }
}
