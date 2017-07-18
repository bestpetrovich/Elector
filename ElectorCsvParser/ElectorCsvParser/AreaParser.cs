using ElectorDal;
using System;
using System.Collections.Generic;
using System.IO;

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

                string[] markers = Markers.CreateStreetMarkers();
                bool gotStreet = false;
                foreach (var marker in markers)
                {
                    var markerPos = addrStr.IndexOf(marker);
                    if (markerPos < 0)
                        continue;

                    gotStreet = true;
                    Street street;
                    if (markerPos == 0)
                    {
                        var shortName = addrStr.Substring(markerPos+marker.Length).Trim(CsvParser.TrimChars);
                        street = new Street(shortName, marker);
                    }
                    else
                    {
                        street = CsvParser.ParseStreet(addrStr);
                    }

                    if(!_streetHouses.ContainsKey(street))
                        _streetHouses.Add(street, new List<House>());

                    break;
                }

                if (!gotStreet)
                    throw new Exception(string.Format("Cant parse Street: {0}", addrStr));
            }
        }
    }
}