using System.Collections.Generic;

namespace ElectorDal
{
    public static class Markers
    {
        public static string[] CreateStreetMarkers()
        {
            var markers = new List<string>();

            markers.Add("пр.");
            markers.Add("просп.");
            markers.Add("ул.");
            markers.Add("бульв.");
            markers.Add("бульвар");
            markers.Add("пер.");
            markers.Add("б-р");
            markers.Add("пр-т");
            markers.Add("пр-зд");
            markers.Add("пр-кт");
            markers.Add("пр-д");

            var outMarkers = new List<string>();
            foreach (var marker in markers)
                outMarkers.Add(marker.ToUpper());

            return outMarkers.ToArray();
        }

        public static string DecodeMarker(string marker)
        {
            var dic = new Dictionary<string, string>();

            dic.Add("пр.", "просп.");
            dic.Add("просп.", "просп.");
            dic.Add("пр-т", "просп.");
            dic.Add("пр-кт", "просп.");

            dic.Add("ул.", "ул.");

            dic.Add("бульв.", "бульв.");
            dic.Add("бульвар", "бульв.");
            dic.Add("б-р", "бульв.");

            dic.Add("пер.", "пер.");

            dic.Add("пр-зд", "проезд");
            dic.Add("пр-д", "проезд");

            string key = marker.ToLower();
            if(dic.ContainsKey(key))
                return dic[key];

            return string.Empty;
        }

        public static string[] CreateHouseMarkers()
        {
            var markers = new List<string>();

            markers.Add("д.");
            markers.Add("дом");
            markers.Add("КВАРТ.");

            var outMarkers = new List<string>();
            foreach (var marker in markers)
                outMarkers.Add(marker.ToUpper());

            return outMarkers.ToArray();
        }

        public static string[] CreateSubHouseMarkers()
        {
            var markers = new List<string>();

            markers.Add("корп.");
            markers.Add("корпус");
            markers.Add("к.");

            var outMarkers = new List<string>();
            foreach (var marker in markers)
                outMarkers.Add(marker.ToUpper());

            return outMarkers.ToArray();
        }

        public static string[] CreateFlatMarkers()
        {
            var markers = new List<string>();

            markers.Add("кв.");
            markers.Add("квартира");

            var outMarkers = new List<string>();
            foreach (var marker in markers)
                outMarkers.Add(marker.ToUpper());

            return outMarkers.ToArray();
        }
    }
}