using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherData.Models
{
    public class Measurement
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Locations Location { get; set; }
        public double Temperature { get; set; }
        public int Humidity { get; set; }
    }
}
