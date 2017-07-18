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
        private static char[] digits = new[] { '0', '1', '2', '3', '4','5', '6', '7', '8', '9' };
        public static char[] TrimChars = new[] { ' ', ',', '.' };
        private Dictionary<Street, List<House>> _houses = new Dictionary<Street, List<House>>();
        private List<Problem> _problems = new List<Problem>();

        public CsvParser(string csvFileName)
        {
            var text = File.ReadAllText(csvFileName, Encoding.UTF8);
            text = ClearLfChar(text);
            csvFileData = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        internal IReadOnlyList<Problem> GetProblems()
        {
            return _problems.AsReadOnly();
        }

        internal Street[] GetStreets()
        {
            return _houses.Keys.ToArray();
        }

        internal Dictionary<Street, List<House>> GetStreetHouses()
        {
            return _houses;
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

                var problem = GetProblem(items[3], items[4]);
                if (problem == null)
                    continue;

                _problems.Add(problem);

                var addrStr = GetAddrString(city, items[4]);
                if (string.IsNullOrEmpty(addrStr))
                    continue;

                var street = ParseStreet(addrStr);
                if (street == null)
                    continue;

                problem.Street = street;

                var houses = AddOrUpdateStreet(street);
                var house = ParseHouse(addrStr);
                if (house == null)
                    continue;

                problem.House = house;

                if (!houses.Any(h => h.Number == house.Number && h.SubNumber == house.SubNumber))
                    houses.Add(house);

                var flat = ParseFlat(addrStr);
                if (flat == null)
                    continue;

                problem.Flat = flat;

                house.AddFlat(flat);                
            }            
        }

        private Problem GetProblem(string content, string addr)
        {
            var items = addr.ToUpper().Split(',');

            if (string.IsNullOrEmpty(content) || items.Length < 2)
                return null;

            var fio = items[0].Trim('\"');
            var dig = fio.IndexOfAny(digits);
            if (dig > 0)
                fio = fio.Substring(0, dig).Trim();

            var problem = new Problem()
            {
                Text = content,
                FIO = fio
            };

            return problem;
        }

        private List<House> AddOrUpdateStreet(Street street)
        {
            List<House> items;
            var streetKey = _houses.Keys.FirstOrDefault(s => s.FullName == street.FullName);
            if (streetKey == null)
            {
                items = new List<House>();
                _houses.Add(street, items);
            }
            else
            {
                items = _houses[streetKey];
            }

            return items;
        }

        private string GetAddrString(string city, string str)
        {
            var cityPos = str.IndexOf(city);
            if (cityPos < 0)
                return null;

            return str.Substring(cityPos + city.Length).Trim(TrimChars).ToUpper();
        }

        public static Street ParseStreet(string addrStr)
        {           
            string[] markers = Markers.CreateStreetMarkers();
            foreach(var marker in markers)
            {
                var markerPos = addrStr.IndexOf(marker);
                if (markerPos < 0)
                    continue;

                string shortName = string.Empty;
                if (markerPos == 0) //Не нашли маркер улицы
                {
                    var houseMarkers = Markers.CreateHouseMarkers();
                    foreach (var house in houseMarkers) //Пробуем поиск по маркеру дома
                    {
                        var houseIndex = addrStr.IndexOf(house);
                        if (houseIndex < 0)
                            continue;

                        shortName = addrStr.Substring(markerPos + marker.Length, houseIndex - marker.Length).Trim(TrimChars);
                        return new Street(shortName, marker);
                    }

                    throw new Exception(string.Format("Не найден маркер дома. Строка: {0}", addrStr));
                }
                else
                {
                    shortName = addrStr.Substring(0, markerPos).Trim(TrimChars);
                    return new Street(shortName, marker);
                }
            }
            
            var digitPos = addrStr.IndexOfAny(digits);
            if(digitPos > 0)
            {
                return new Street(addrStr.Substring(0, digitPos).Trim(TrimChars), " ");
            }

            throw new Exception(string.Format("Не найден маркер адреса строка:{0}", addrStr));           
        }

        private House ParseHouse(string addrStr)
        {
            var house = new House();
            var houseMarkers = Markers.CreateHouseMarkers();
            foreach (var marker in houseMarkers) //Пробуем поиск по маркеру дома
            {
                var index = addrStr.IndexOf(marker);
                if (index < 0)
                    continue;

                house.Number = TextParser.GetNextWord(addrStr, index, marker);
                break;
            }

            if (string.IsNullOrEmpty(house.Number))
            {
                var digitPos = addrStr.IndexOfAny(digits);
                if (digitPos > 0)
                    house.Number = TextParser.GetNextWord(addrStr, digitPos, "");
            }

            if (string.IsNullOrEmpty(house.Number))
                return null;

            var subHouseMarkers = Markers.CreateSubHouseMarkers();
            foreach (var marker in subHouseMarkers) //Пробуем поиск по маркеру корпуса
            {
                var index = addrStr.IndexOf(marker);
                if (index < 0)
                    continue;

                house.SubNumber = TextParser.GetNextWord(addrStr, index, marker);
                break;
            }

            return house;
        }

        private Flat ParseFlat(string addrStr)
        {
            var flat = new Flat();
            var flatMarkers = Markers.CreateFlatMarkers();
            foreach (var marker in flatMarkers) //Пробуем поиск по маркеру дома
            {
                var index = addrStr.IndexOf(marker);
                if (index < 0)
                    continue;
                
                var textNumber = TextParser.GetNextWord(addrStr, index, marker);
                if (int.TryParse(textNumber, out int number))
                    flat.Number = number;
            }

            if (flat.Number == 0)
                return null;

            return flat;
        }
    }
}