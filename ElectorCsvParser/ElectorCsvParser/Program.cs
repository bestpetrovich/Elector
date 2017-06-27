using ElectorDal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectorCsvParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var csvFileName = "in.csv";
            string city = "Москва";

            var parser = new CsvParser(csvFileName);
            parser.Go(city);
            var streets = parser.GetStreets();

            UpdateStreets(city, streets);

            var streetHouses = parser.GetStreetHouses();
            UpdateStreetHouses(city, streetHouses);
        }

        private static void UpdateStreets(string city, Street[] streets)
        {
            using (var context = new ElectorContext())
            {
                var dbCity = context.Citys.FirstOrDefault(c => c.Name == city);
                foreach(var item in streets)
                {
                    var dbStreet = context.Streets.FirstOrDefault(s =>
                                            s.idCity == dbCity.id &&
                                            s.FullName == item.FullName);

                    if (dbStreet != null)
                        continue;

                    item.idCity = dbCity.id;
                    context.Streets.Add(item);
                }

                context.SaveChanges();
            }
        }

        private static void UpdateStreetHouses(string city, Dictionary<Street, List<House>> streetHouses)
        {
            using (var context = new ElectorContext())
            {
                var dbCity = context.Citys.FirstOrDefault(c => c.Name == city);
                foreach(var street in streetHouses)
                {
                    var dbStreet = context.Streets.FirstOrDefault(s =>
                                            s.idCity == dbCity.id &&
                                            s.FullName == street.Key.FullName);

                    if (dbStreet == null)
                        throw new Exception("Street not found in DB");

                    var idStreet = dbStreet.id;

                    foreach(var house in street.Value)
                    {
                        var dbHouse = context.Houses.FirstOrDefault(h =>
                                        h.idStreet == idStreet &&
                                        h.Number == house.Number &&
                                        h.SubNumber == house.SubNumber
                        );

                        if (dbHouse == null)
                        {
                            house.idStreet = idStreet;
                            context.Houses.Add(house);
                            context.SaveChanges();

                            UpdateFlats(house);
                        }
                        else
                            UpdateFlats(dbHouse);                        
                    }
                }
            }
        }

        private static void UpdateFlats(House house)
        {
            using (var context = new ElectorContext())
            {
                var flats = house.GetFlats();
                foreach(var flat in flats)
                {
                    var dbFlat = context.Flats.FirstOrDefault(f =>
                                    f.idHouse == house.id &&
                                    f.Number == flat.Number
                    );

                    if(dbFlat == null)
                    {
                        flat.idHouse = house.id;
                        context.Flats.Add(flat);
                        context.SaveChanges();
                    }
                }
            }
        }
    }
}
