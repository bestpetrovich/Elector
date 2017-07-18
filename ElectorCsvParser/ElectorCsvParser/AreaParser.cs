using ElectorDal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ElectorCsvParser
{
    internal class AreaParser
    {
        private int _areaNumber;
        private Dictionary<Street, List<House>> _streetHouses = new Dictionary<Street, List<House>>();

        public AreaParser(int areaNumber, string fileName)
        {
            _areaNumber = areaNumber;
            var lines = File.ReadAllLines(fileName);

            foreach (var str in lines)
            {
                var indx = str.IndexOf(':');
                var addrStr = str.Substring(0, indx).ToUpper();

                var street = GetStreet(addrStr);
                if (!_streetHouses.ContainsKey(street))
                    _streetHouses.Add(street, new List<House>());

                var houses = GetHouses(str.Substring(indx + 1));
                _streetHouses[street].AddRange(houses);
            }
        }

        public IReadOnlyDictionary<Street, List<House>> GetStreetHouses()
        {
            return _streetHouses;
        }

        private Street GetStreet(string addrStr)
        {
            string[] markers = Markers.CreateStreetMarkers();
            foreach (var marker in markers)
            {
                var markerPos = addrStr.IndexOf(marker);
                if (markerPos < 0)
                    continue;

                Street street;
                if (markerPos == 0)
                {
                    var shortName = addrStr.Substring(markerPos + marker.Length).Trim(CsvParser.TrimChars);
                    street = new Street(shortName, marker);
                }
                else
                {
                    street = CsvParser.ParseStreet(addrStr);
                }

                return street;
            }

            throw new Exception(string.Format("Cant parse Street: {0}", addrStr));
        }

        private List<House> GetHouses(string str)
        {
            string[] houseGroups = SplitToHouseGroups(str);
            var houses = new List<House>();
            foreach (var houseStr in houseGroups)
            {
                houses.AddRange(ParseHouses(houseStr));
            }

            return houses;
        }

        private IEnumerable<House> ParseHouses(string houseStr)
        {
            var leftBracket = houseStr.IndexOf('(');
            var rightBracket = houseStr.IndexOf(')');

            if (leftBracket < 0)
                return new List<House>() { CsvParser.ParseHouse(houseStr) };

            var number = houseStr.Substring(0, leftBracket).Trim();

            var subNumberStr = houseStr.Substring(leftBracket + 1, rightBracket - leftBracket-1);
            var items = subNumberStr.Split(',');
            var houses = new List<House>();
            var markers = Markers.CreateSubHouseMarkers();
            foreach (var str in items)
            {
                var subNumber = str.ToUpper().Trim();
                foreach (var marker in markers)
                {
                    var markerPos = subNumber.IndexOf(marker);
                    if (markerPos < 0)
                        continue;

                    subNumber = subNumber.Substring(markerPos + marker.Length).Trim();
                }

                houses.Add(new House()
                {
                    Number = number,
                    SubNumber = subNumber
                });
            }

            return houses;
        }

        private string[] SplitToHouseGroups(string str)
        {
            var houseGroups = new List<string>();


            bool gotLeftBracket = false;
            bool gotRightBracket = false;
            var groupStr = new StringBuilder();
            for (int i=0; i<str.Length; i++)
            {
                if (str[i] == '(')
                {
                    gotLeftBracket = true;
                    groupStr.Append(str[i]);
                }
                else
                if (str[i] == ')')
                {
                    gotRightBracket = true;
                    groupStr.Append(str[i]);
                }
                else if (str[i] == ',')
                {
                    if (!gotLeftBracket || (gotLeftBracket && gotRightBracket))
                    {
                        houseGroups.Add(groupStr.ToString());
                        groupStr.Clear();
                        gotLeftBracket = false;
                        gotRightBracket = false;
                    }
                    else
                        groupStr.Append(str[i]);
                }
                else
                {
                    groupStr.Append(str[i]);
                }

                if (i == str.Length - 1)
                    houseGroups.Add(groupStr.ToString());
            }

            return houseGroups.ToArray();
        }
    }
}