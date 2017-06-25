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

        public CsvParser(string csvFileName)
        {
            csvFileData = File.ReadAllLines(csvFileName, Encoding.UTF8);
        }

        internal Street[] GetStreets(string city)
        {
            var streets= new List<Street>();
            foreach (var str in csvFileData)
            {
                var items = str.Split(';');

                if (items.Length < 5)
                    continue;

                var street = ParseStreet(city, items[4]);
                if (street != null)
                    streets.Add(street);
            }

            return streets.ToArray();
        }

        private Street ParseStreet(string city, string str)
        {
            char[] trimChars = new[] { ' ', ',' };
            string[] markers = CreateStreetMarkers();

            var cityPos = str.IndexOf(city);
            if (cityPos < 0)
                return null;

            var addrStr = str.Substring(cityPos + city.Length).Trim(trimChars).ToUpper();

            foreach(var marker in markers)
            {
                var markerPos = addrStr.IndexOf(marker);
                if (markerPos < 0)
                    continue;

                var shortName = addrStr.Substring(0, markerPos).Trim(trimChars);
                return new Street(shortName, marker);
            }

            char[] digits = new[] { '0', '1', '2', '3', '4','5', '6', '7', '8', '9' };
            var digitPos = addrStr.IndexOfAny(digits);
            if(digitPos > 0)
            {
                return new Street(addrStr.Substring(0, digitPos).Trim(trimChars), " ");
            }

            throw new Exception(string.Format("Не найден маркер адреса строка:{0}", addrStr));           
        }

        private string[] CreateStreetMarkers()
        {
            var markers = new List<string>();

            markers.Add("пр.");
            markers.Add("просп.");
            markers.Add("ул.");
            markers.Add("бульв.");
            markers.Add("пер.");

            var outMarkers = new List<string>();
            foreach (var marker in markers)
            {
                outMarkers.Add( marker.ToUpper());                
            }

            return outMarkers.ToArray();
        }
    }
}