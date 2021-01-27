using System;
using System.Linq;
using WeatherData;
using WeatherData.Models;
using Microsoft.EntityFrameworkCore;

namespace Project_Weather_Date
{

    class Program
    {
        static string dataFileName = "TemperaturData.csv";
        private static bool running = true;

        static void Main(string[] args)
        {
            //Checks if the database contains data
            if (DataAccess.HasData())
            {
                Console.WriteLine("Database exists and contains data.!");
            }
            else
            {
                Console.WriteLine("Database exists, but DOES NOT contain any data. Let us load data!");
                DataAccess.LoadData(dataFileName);
            }

            while (running)
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Clear();

                bool menu = true;
                Menu(menu);
                menu = ReadKey(menu);
            }
        }
        static void Menu(bool menu)
        {
            Console.WriteLine($"Welcome to Weather data!    {(menu ? "Startpage" : "Page")}\n\nPlease wait while loading data...\n");
            if (menu)
            {
                Console.WriteLine($"------------------------------------------");
                Console.WriteLine($"Information: Autumn occured at {DataAccess.CalculateAutumn().ToShortDateString()}");
                var winterDate = DataAccess.CalculateWinter();
                if (winterDate.Year != 1)
                    Console.WriteLine($"Information: Winter started at {winterDate.ToShortDateString()}");
                else
                    Console.WriteLine("Information: Winter was mild");
                Console.WriteLine($"------------------------------------------");
            }
            Console.WriteLine("\nPress one of the below keys to view more.\n\n1. Search for average temperature a specific day\n\n2. Sort average temperature per day, 3 warmest and 3 coldest\n\n3. Sort average humidity per day, 3 driest and 3 humidiest\n\n4. Sort mold risk, 3 lowest and 3 highest\n\n5. Exit");
            Console.Write("\nChoose from menu: ");
        }
        static void Exit()
        {
            Console.WriteLine("Exits");
            running = false;
        }
        static bool ReadKey(bool menu)
        {
            var validKey = false;
            while  (!validKey)
            {
                char key = Console.ReadKey(true).KeyChar;
                    switch (key)
                    {

                        case '1':
                            menu = false;
                            Console.Clear();
                            DisplayAveragesForDate(menu);
                            validKey = true;
                            break;
                        case '2':
                            menu = false;
                            Console.Clear();
                            DisplayWarmestColdest(menu);
                            validKey = true;
                        break;
                        case '3':
                            menu = false;
                            Console.Clear();
                            DisplayDryToDamp(menu);
                            validKey = true;
                        break;
                        case '4':
                            menu = false;
                            Console.Clear();
                            DisplayLowestToHighestMoldRisk(menu);
                            validKey = true;
                        break;
                        case '5':
                            menu = false;
                            Console.Clear();
                            Exit();
                            validKey = true;
                        break;
                        default:
                            break;
                    }
            }
            return menu;
        }
        
        private static void DisplayAveragesForDate(bool menu)
        {
            Console.WriteLine($"You are on {(menu ? "Startpage" : "Page")} 1. Search for average temperature a specific day\n");
            var validDate = false;
            string inputDate = default;
            DateTime inputDateParse = default;
            while (!validDate)
            {

                Console.WriteLine("Enter a date in the following format: year-month-date (xxxx-xx-xx)");
                inputDate = Console.ReadLine();

                try
                {
                    inputDateParse = DateTime.Parse(inputDate);
                    validDate = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("Incorrect date format. Try again.\n");
                }
            }
            try
            {
                var result = DataAccess.GetAverageTemperature(Locations.Inside, inputDateParse);
                var result2 = DataAccess.GetAverageTemperature(Locations.Outside, inputDateParse);
                Console.WriteLine($"Average indoor temperature: {Math.Round(result, 1)}");
                Console.WriteLine($"Average outdoor temperature: {Math.Round(result2, 1)}");
            }
            catch (Exception)
            {
                Console.WriteLine("No data was found.");
            }
            Console.WriteLine("\nPress any key to get to the startpage.");
            Console.ReadKey(true);
        }
        private static void DisplayWarmestColdest(bool menu)
        {
            Console.WriteLine($"You are on {(menu ? "Startpage" : "Page")} 2. Sort average temperature per day, 3 warmest and 3 coldest\n");

            var SortResultInsideTemp = DataAccess.SortWarmestToColdestTemperature(Locations.Inside);
            var SortResultOutsideTemp = DataAccess.SortWarmestToColdestTemperature(Locations.Outside);

            //Dispay 3 warmest
            Console.WriteLine($"Sorts average INDOOR temperature per day (warmest to coldest).");
            foreach (var item in SortResultInsideTemp.Take(3))
            {
                Console.WriteLine($"{item.Date.ToShortDateString()}, {Math.Round(item.Temperature, 1)}");
            }
            Console.WriteLine("...");
            foreach (var item in SortResultInsideTemp.Skip(SortResultInsideTemp.Count() - 3))
            {
                Console.WriteLine($"{item.Date.ToShortDateString()}, {Math.Round(item.Temperature, 1)}");
            }

            //Dispay 3 coldest
            Console.WriteLine($"\nSorts average OUTDOOR temperature per day (warmest to coldest).");
            foreach (var item in SortResultOutsideTemp.Take(3))
            {
                Console.WriteLine($"{item.Date.ToShortDateString()}, {Math.Round(item.Temperature, 1)}");
            }
            Console.WriteLine("...");
            foreach (var item in SortResultOutsideTemp.Skip(SortResultOutsideTemp.Count() - 3))
            {
                Console.WriteLine($"{item.Date.ToShortDateString()}, {Math.Round(item.Temperature, 1)}");
            }
            Console.WriteLine("\nPress any key to get to the startpage.");
            Console.ReadKey(true);

        }
        private static void DisplayDryToDamp(bool menu)
        {
            Console.WriteLine($"You are on {(menu ? "Startpage" : "Page")} 3. Sort average humidity per day, 3 dryest and 3 humidiest\n");

            var SortResultInsideHumidity = DataAccess.SortHumidityDryToDamp(Locations.Inside);
            var SortResultOutsideHumidity = DataAccess.SortHumidityDryToDamp(Locations.Outside);

            //Dispay 3 dryest
            Console.WriteLine($"Sorts average INDOOR humidity per day (dryest to humidiest).");
            foreach (var item in SortResultInsideHumidity.Take(3))
            {
                Console.WriteLine($"{item.Date.ToShortDateString()}, {item.Humidity}");
            }
            Console.WriteLine("...");
            foreach (var item in SortResultInsideHumidity.Skip(SortResultInsideHumidity.Count() - 3))
            {
                Console.WriteLine($"{item.Date.ToShortDateString()}, {item.Humidity}");
            }

            //Dispay 3 humidiest
            Console.WriteLine($"\nSorts average OUTDOOR humidity per day (dryest to humidiest).");
            foreach (var item in SortResultOutsideHumidity.Take(3))
            {
                Console.WriteLine($"{item.Date.ToShortDateString()}, {item.Humidity}");
            }
            Console.WriteLine("...");
            foreach (var item in SortResultOutsideHumidity.Skip(SortResultOutsideHumidity.Count() - 3))
            {
                Console.WriteLine($"{item.Date.ToShortDateString()}, {item.Humidity}");
            }
            Console.WriteLine("\nPress any key to get to the startpage.");
            Console.ReadKey(true);
        }
        private static void DisplayLowestToHighestMoldRisk(bool menu)
        {
            Console.WriteLine($"You are on {(menu ? "Startpage" : "Page")} 4. Sort average mold risk per day, 3 lowest and 3 highest\n");
            Console.WriteLine("Description: The received value is between 0 - 100 % risk. The formula does not take into account materials.\n");
             var SortResultInsideMold = DataAccess.SortMoldRiskLowestHighest(Locations.Inside);
            var SortResultOutsideMold = DataAccess.SortMoldRiskLowestHighest(Locations.Outside);

            //Dispay 3 lowest mold risk
            Console.WriteLine($"Sorts result of INDOOR mold risk per day (lowest to highest mold risk)");
            foreach (var item in SortResultInsideMold.Take(3))
            {
                var currentMold = item.MouldRisk < 0 ? 0 : item.MouldRisk;
                Console.WriteLine($"{item.Date.ToShortDateString()} {Math.Round(currentMold, 1)}");
            }
            Console.WriteLine("...");
            foreach (var item in SortResultInsideMold.Skip(SortResultInsideMold.Count() - 3))
            {
                var currentMold = item.MouldRisk < 0 ? 0 : item.MouldRisk;
                Console.WriteLine($"{item.Date.ToShortDateString()}, {Math.Round(currentMold, 1)}");
            }

            //Dispay 3 highest mold risk
            Console.WriteLine($"\nSorts result of OUTDOOR mold risk per day (lowest to highest mold risk)");
            foreach (var item in SortResultOutsideMold.Take(3))
            {
                var currentMold = item.MouldRisk < 0 ? 0 : item.MouldRisk;
                Console.WriteLine($"{item.Date.ToShortDateString()}, {Math.Round(currentMold, 1)}");
            }
            Console.WriteLine("...");
            foreach (var item in SortResultOutsideMold.Skip(SortResultOutsideMold.Count() - 3))
            {
                var currentMold = item.MouldRisk < 0 ? 0 : item.MouldRisk;
                Console.WriteLine($"{item.Date.ToShortDateString()}, {Math.Round(currentMold, 1)}");
            }
            Console.WriteLine("\nPress any key to get to the startpage.");
            Console.ReadKey(true);

        }
    }
}
