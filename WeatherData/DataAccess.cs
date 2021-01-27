using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeatherData.Models;

namespace WeatherData
{
    public class DataAccess
    {
        public static bool HasData()
        {
            bool retVal = false;
            using (var db = new WeatherContext())
            {
                retVal = db.Measurements.Count() > 0;
            }
            return retVal;
        }
        public static void LoadData(string fileName)
        {
            List<Measurement> data = DataImporter.ReadFromFile(fileName);
            using (var db = new WeatherContext())
            {
                //foreach (var item in data)
                //{
                //    Measurement m = new Measurement()
                //    {
                //        Date = item.Date,
                //        Location = item.Location,
                //        Temperature = item.Temperature,
                //        Humidity = item.Humidity

                //    };
                    
                //}
                db.Measurements.AddRange(data);
                db.SaveChanges();
            }
        }

        public static double GetAverageTemperature(Locations location, DateTime date)
        {
            var startTime = date.Date; //Time is set to 0. For instance 2016-01-15 00:00
            var endTime = startTime.AddDays(1);                  //Here 2016-01-16 00:00

            using (var db = new WeatherContext()) //Opens connection of the database
            {
                var q = db.Measurements
                    .Where(x => x.Location == location && x.Date >= startTime && x.Date < endTime)
                    .Average(x => x.Temperature);
                return q;
            } //Closes connection of the database

        }
        public static List<(double Temperature, DateTime Date)> SortWarmestToColdestTemperature(Locations location)
        {
            List<(double Temperature, DateTime Date)> results = new();

            using (var db = new WeatherContext())
            {
                var q = db.Measurements
                    .Where(x => x.Location == location)
                    .GroupBy(x => x.Date.Date)
                    .Select(x => new { Date = x.Key, AverageTemp = x.Average(x => x.Temperature) })
                    .OrderByDescending(x => x.AverageTemp);

                foreach (var item in q)
                {
                    results.Add((item.AverageTemp, item.Date));
                }
            }
            return results;
        }

        public static List<(int Humidity, DateTime Date)> SortHumidityDryToDamp(Locations location)
        {
            List<(int Humidity, DateTime Date)> humidityResults = new();

            using (var db = new WeatherContext())
            {
                var q = db.Measurements
                    .Where(x => x.Location == location)
                    .GroupBy(x => x.Date.Date)
                    .Select(x => new { Date = x.Key, AverageHumidity = x.Average(x => x.Humidity) })
                    .OrderBy(x => x.AverageHumidity);

                foreach (var item in q)
                {
                    humidityResults.Add(((int)Math.Round(item.AverageHumidity), item.Date));
                }
            }
            return humidityResults;
        }

        public static List<(double MouldRisk, DateTime Date)> SortMoldRiskLowestHighest(Locations location)
        {
            List<(double MouldRisk, DateTime Date)> moldResults = new();

            using (var db = new WeatherContext())
            {
                var q = db.Measurements
                    .Where(x => x.Location == location)
                    .GroupBy(x => x.Date.Date)
                    .Select(x => new { Date = x.Key, AverageHumidity = x.Average(x => x.Humidity), AverageTemp = x.Average(x => x.Temperature) })
                    .OrderBy(x => x.Date.Date);

                foreach (var item in q)
                {
                    var mould = ((item.AverageHumidity - 78) * (item.AverageTemp / 15)) / 0.22; //The received value is between 0-100 % risk. The formula does not take into account materials
                    moldResults.Add((mould, item.Date));
                }
            }

            return moldResults.OrderBy(o => o.MouldRisk).ToList();
        }
        public static DateTime CalculateAutumn()
        {
            using (var db = new WeatherContext())
            {
                var q = db.Measurements
                    .AsEnumerable()
                    .Where(x => x.Location == Locations.Outside && x.Date.Month > 7)
                    .GroupBy(x => x.Date.Date)
                    .OrderBy(x => x.Key);

                int countDays = 0;
                DateTime autumnStart = default;

                foreach (var item in q)
                {
                    var av = item.Average(a => a.Temperature);

                    if (av < 10.0)
                    {
                        countDays++;
                        if (countDays == 1)
                        {
                            autumnStart = item.Key;
                        }
                        if (countDays == 5)
                        {
                            return autumnStart;
                        }
                    }
                    else { countDays = 0; }
                }
                return default;
            }
        }

        public static DateTime CalculateWinter()
        {
            using (var db = new WeatherContext())
            {
                var q = db.Measurements
                    .AsEnumerable()
                    .Where(x => x.Location == Locations.Outside && x.Date.Month > 8)
                    .GroupBy(x => x.Date.Date)
                    .OrderBy(x => x.Key);

                int countDays = 0;
                DateTime winterStart = default;

                foreach (var item in q)
                {
                    var av = item.Average(a => a.Temperature);

                    if (av <= 0)
                    {
                        countDays++;
                        if (countDays == 1)
                        {
                            winterStart = item.Key;
                        }
                        if (countDays == 5)
                        {
                            return winterStart;
                        }
                    }
                    else { countDays = 0; }
                }
                // If we came here, winter does not meet the requirements of temperature below 0, 5 days in a row
                return default;
            }
        }
    }
}
