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

            var streets = parser.GetStreets(city);

            UpdateStreets(city, streets);
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
    }
}
