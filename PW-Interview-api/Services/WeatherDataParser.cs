using PW_Interview_api.Models;
using System.Text;

namespace PW_Interview_api.Services
{
    public class WeatherDataParser : IWeatherDataParser
    {
        public WeatherStation Parse(string input)
        {
            input = input.Trim(); // Elimina espacios innecesarios
            input = input.Substring(1, input.Length - 2); // Elimina paréntesis exteriores

            var parts = SplitByCommas(input);
            if (parts.Count < 5)
                throw new InvalidOperationException("Formato de entrada incorrecto.");

            var stationId = parts[0];
            var location = parts[1];
            var temperature = parts[2];
            var readingsData = parts[3];
            var altitude = parts[4];

            var readings = ParseReadings(readingsData);
            return new WeatherStation(stationId, location, temperature, readings, altitude);
        }

        private List<string> SplitByCommas(string input)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            int depth = 0;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c == '(') depth++;
                if (c == ')') depth--;
                if (c == ',' && depth == 0)
                {
                    result.Add(current.ToString().Trim());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            result.Add(current.ToString().Trim());
            return result;
        }

        private ReadingModel ParseReadings(string input)
        {
            input = input.Trim();

            // Asegúrate de que "readings(" está al principio y ")" al final
            if (!input.StartsWith("readings(") || !input.EndsWith(")"))
                throw new InvalidOperationException("Formato de 'readings' incorrecto.");

            input = input.Substring(9, input.Length - 10); // Eliminar "readings(" y ")"
            var parts = SplitByCommas(input);

            if (parts.Count < 3)
                throw new InvalidOperationException("Formato de 'readings' incorrecto.");

            var timestamp = parts[0];
            var windSpeed = parts[1];
            var precipitationData = parts[2];
            var precipitation = ParsePrecipitation(precipitationData);

            return new ReadingModel(timestamp, windSpeed, precipitation);
        }

        private PrecipitationModel ParsePrecipitation(string input)
        {
            input = input.Trim();

            // Asegúrate de que "precipitation(" está al principio y ")" al final
            if (!input.StartsWith("precipitation(") || !input.EndsWith(")"))
                throw new InvalidOperationException("Formato de 'precipitation' incorrecto.");

            input = input.Substring(14, input.Length - 15); // Eliminar "precipitation(" y ")"
            var parts = SplitByCommas(input);

            if (parts.Count != 3)
                throw new InvalidOperationException("Formato de 'precipitation' incorrecto.");

            var amount = parts[0];
            var type = parts[1];
            var intensity = parts[2];

            return new PrecipitationModel(amount, type, intensity);
        }


        public class WeatherStationPrinter
        {
            // Método que imprime los datos de la estación meteorológica de forma jerárquica
            public void Print(WeatherStation station, TextWriter writer)
            {
                // Usamos TextWriter para imprimir a un flujo de texto (en este caso, StringWriter)
                PrintIndented(station.StationId, 0, writer);
                PrintIndented(station.Location, 0, writer);
                PrintIndented(station.Temperature, 0, writer);
                PrintIndented("readings", 0, writer);

                // Los elementos dentro de 'readings' deben estar al nivel 1
                PrintIndented(station.Readings.Timestamp, 1, writer);
                PrintIndented(station.Readings.WindSpeed, 1, writer);
                PrintIndented("precipitation", 1, writer);

                // Los elementos dentro de 'precipitation' deben estar al nivel 2
                PrintIndented(station.Readings.Precipitation.Amount, 2, writer);
                PrintIndented(station.Readings.Precipitation.Type, 2, writer);
                PrintIndented(station.Readings.Precipitation.Intensity, 2, writer);

                // 'altitude' debe estar nuevamente al nivel 0
                PrintIndented(station.Altitude, 0, writer);
            }

            // Imprime una línea con la indentación adecuada
            private void PrintIndented(string text, int indentLevel, TextWriter writer)
            {
                writer.WriteLine(new string(' ', indentLevel * 2) + "- " + text);
            }
        }


    }
}
