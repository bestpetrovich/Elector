using ElectorDal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ElectorCsvParser
{
    internal class CsvParser
    {
        private string[] csvFileData;
        private char[] digits = new[] { '0', '1', '2', '3', '4','5', '6', '7', '8', '9' };
        private char[] trimChars = new[] { ' ', ',', '.' };
        private List<Street> streets = new List<Street>();
        private Dictionary<Street, List<House>> houses = new Dictionary<Street, List<House>>();

        public CsvParser(string csvFileName)
        {
            var text = File.ReadAllText(csvFileName, Encoding.UTF8);
            text = ClearLfChar(text);
            csvFileData = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        internal Street[] GetStreets()
        {
            return streets.ToArray();
        }

        private string ClearLfChar(string text)
        {
            var outText = new StringBuilder();

            char prevChar = '\a';
            foreach (char c in text)
            {
                if (c == '\n' && prevChar != '\r')
                    continue;

                outText.Append(c);
                prevChar = c;
            }

            return outText.ToString();
        }

        internal void Go(string city)
        {
            
            foreach (var str in csvFileData)
            {
                var items = str.Split(';');

                if (items.Length < 5)
                    continue;

                var addrStr = GetAddrString(city, items[4]);
                if (string.IsNullOrEmpty(addrStr))
                    continue;

                var street = ParseStreet(addrStr);
                if (street == null)
                    continue;
                
                if (streets.FirstOrDefault(s => s.FullName == street.FullName) == null)
                    streets.Add(street);

                var house = ParseHouse(addrStr);
                house.Street = street;
            }            
        }

        private string GetAddrString(string city, string str)
        {
            var cityPos = str.IndexOf(city);
            if (cityPos < 0)
                return null;

            return str.Substring(cityPos + city.Length).Trim(trimChars).ToUpper();
        }

        private Street ParseStreet(string addrStr)
        {           
            string[] markers = CreateStreetMarkers();
            foreach(var marker in markers)
            {
                var markerPos = addrStr.IndexOf(marker);
                if (markerPos < 0)
                    continue;

                string shortName = string.Empty;
                if (markerPos == 0) //Не нашли маркер улицы
                {
                    var houseMarkers = CreateHouseMarkers();
                    foreach (var house in houseMarkers) //Пробуем поиск по маркеру дома
                    {
                        var houseIndex = addrStr.IndexOf(house);
                        if (houseIndex < 0)
                            continue;

                        shortName = addrStr.Substring(markerPos + marker.Length, houseIndex - marker.Length).Trim(trimChars);
                        return new Street(shortName, marker);
                    }

                    throw new Exception(string.Format("Не найден маркер дома. Строка: {0}", addrStr));
                }
                else
                {
                    shortName = addrStr.Substring(0, markerPos).Trim(trimChars);
                    return new Street(shortName, marker);
                }
            }
            
            var digitPos = addrStr.IndexOfAny(digits);
            if(digitPos > 0)
            {
                return new Street(addrStr.Substring(0, digitPos).Trim(trimChars), " ");
            }

            throw new Exception(string.Format("Не найден маркер адреса строка:{0}", addrStr));           
        }

        private House ParseHouse(string addrStr)
        {
            var house = new House();
            var houseMarkers = CreateHouseMarkers();
            foreach (var marker in houseMarkers) //Пробуем поиск по маркеру дома
            {
                var houseIndex = addrStr.IndexOf(marker);
                if (houseIndex < 0)
                    continue;

                house.Number = TextParser.GetNextWord(addrStr, houseIndex, marker);
            }

            return house;
        }

        private string[] CreateStreetMarkers()
        {
            var markers = new List<string>();

            markers.Add("пр.");
            markers.Add("просп.");
            markers.Add("ул.");
            markers.Add("бульв.");
            markers.Add("бульвар");
            markers.Add("пер.");

            var outMarkers = new List<string>();
            foreach (var marker in markers)            
                outMarkers.Add( marker.ToUpper());                            

            return outMarkers.ToArray();
        }

        private string[] CreateHouseMarkers()
        {
            var markers = new List<string>();

            markers.Add("д.");
            markers.Add("дом");

            var outMarkers = new List<string>();
            foreach (var marker in markers)            
                outMarkers.Add(marker.ToUpper());            

            return outMarkers.ToArray();
        }
    }
}