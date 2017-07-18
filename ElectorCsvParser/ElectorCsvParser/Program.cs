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
            //Area Parsing
            var area1 = new AreaParser(1, "area_1.txt");


            //var csvFileName = "in.csv";
            //string city = "Москва";

            //var parser = new CsvParser(csvFileName);
            //parser.Go(city);

            //UpdateProblems(city, parser.GetProblems());
        }
      
        private static void UpdateProblems(string city, IReadOnlyList<Problem> problems)
        {
            using (var context = new ElectorContext())
            {
                int total = problems.Count;
                int cntr = 0;
                foreach (var problem in problems)
                {
                    Console.WriteLine("Processinng problem {0} from {1}", cntr++, total);

                    if (problem.Street != null)
                    {
                        problem.idStreet = AddOrUpdateStreet(city, problem.Street, context);
                        if (problem.House != null)
                        {
                            problem.idHouse = AddOrUpdateHouse(problem.idStreet.Value, problem.House, context);

                            if (problem.Flat != null)
                            {
                                problem.idFlat = AddOrUpdateFlat(problem.idHouse.Value, problem.Flat, context);
                            }
                        }
                    }

                    var dbProblem = context.Problems.FirstOrDefault(p => p.idStreet == problem.idStreet &&
                                                                    p.idHouse == problem.idHouse && p.idFlat == problem.idFlat &&
                                                                    p.Text == problem.Text && p.FIO == problem.FIO);

                    if (dbProblem == null)
                    {
                        context.Problems.Add(new Problem()
                        {
                            Text = problem.Text,
                            FIO = problem.FIO,
                            idStreet = problem.idStreet,
                            idHouse = problem.idHouse,
                            idFlat = problem.idFlat
                        });
                        context.SaveChanges();
                    }
                }
            }
        }

        private static int? AddOrUpdateStreet(string city, Street street, ElectorContext context)
        {
            var dbCity = context.Citys.FirstOrDefault(c => c.Name == city);
            if (dbCity == null)
            {
                dbCity = new City() { Name = city };
                context.Citys.Add(dbCity);
                context.SaveChanges();
            }


            var dbStreet = context.Streets.FirstOrDefault(s =>
                                    s.idCity == dbCity.id &&
                                    s.FullName == street.FullName);

            if (dbStreet != null)
                return dbStreet.id;

            street.idCity = dbCity.id;
            context.Streets.Add(street);
            context.SaveChanges();

            return street.id;
        }

        private static int? AddOrUpdateHouse(int idStreet, House house, ElectorContext context)
        {
            var dbHouse = context.Houses.FirstOrDefault(h =>
                            h.idStreet == idStreet &&
                            h.Number == house.Number &&
                            h.SubNumber == house.SubNumber
            );

            if (dbHouse != null)                    
                return dbHouse.id;
                   
            house.idStreet = idStreet;
            context.Houses.Add(house);
            context.SaveChanges();
            return house.id;                    
        }

        private static int? AddOrUpdateFlat(int idHouse, Flat flat, ElectorContext context)
        {
            var dbFlat = context.Flats.FirstOrDefault(f =>
                            f.idHouse == idHouse &&
                            f.Number == flat.Number
            );

            if (dbFlat != null)
                return dbFlat.id;

            flat.idHouse = idHouse;
            context.Flats.Add(flat);
            context.SaveChanges();
            return flat.id;
        }

    }
}
