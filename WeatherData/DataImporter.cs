using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using WeatherData.Models;

namespace WeatherData
{
    public class DataImporter
    {
        public static List<Measurement> ReadFromFile(string fileName)
        {
            List<Measurement> l = new List<Measurement>();
            string[] lines = File.ReadAllLines(fileName);
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                string[] parts = line.Split(',');
                var measurement = new Measurement();
                measurement.Date = DateTime.Parse(parts[0]);
                switch (parts[1])
                {
                    case "Ute":
                        measurement.Location = Locations.Outside;
                        break;
                    case "Inne":
                        measurement.Location = Locations.Inside;
                        break;
                }
                measurement.Temperature = double.Parse(parts[2], CultureInfo.InvariantCulture);
                measurement.Humidity = int.Parse(parts[3]);

                l.Add(measurement);
            }
            return l;
        }
    }
}
