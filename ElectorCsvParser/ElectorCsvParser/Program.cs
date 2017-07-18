using ElectorDal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectorCsvParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string city = "Москва";

            //Area Parsing
            //for(int i=0; i<4; i++)
            //{
            //    var area = new AreaParser(i+1, string.Format("area_{0}.txt", i+1));
            //    UpdateArea(i+1, area.GetStreetHouses(), city);
            //}

            //var csvFileName = "in.csv";
            //var parser = new CsvParser(csvFileName);
            //parser.Go(city);
            //UpdateProblems(city, parser.GetProblems());

            //Select By area
            var problems = GetAraeProblems().OrderBy(p=>p.AreaNumber).ToArray();
            SaveToFile(problems, "out\\problem_area.csv");
        }

        private static void SaveToFile(ICollection<ProblemData> problems, string fileName)
        {
            var strBuilder = new StringBuilder();
            char sep = ';';
            foreach(var problem in problems)
            {
                strBuilder.Append(problem.Street.FullName); strBuilder.Append(sep);

                if (problem.House != null)
                    strBuilder.Append(problem.House.Number);

                strBuilder.Append(sep);

                if(problem.House != null)
                    strBuilder.Append(problem.House.SubNumber);

                strBuilder.Append(sep);


            }

            File.WriteAllText(fileName, strBuilder.ToString());
;        }

        private static void UpdateArea(int areaNumber, IReadOnlyDictionary<Street, List<House>> streetHouses, string city)
        {
            using (var context = new ElectorContext())
            {
                var dbAreaId = context.Areas.FirstOrDefault(a => a.City.Name == city && a.AreaNumber == areaNumber).id;
                foreach (var street in streetHouses)
                {
                    var dbStreetId = AddOrUpdateStreet(city, street.Key, context);
                    foreach(var house in street.Value)
                    {
                        var dbHouseId = AddOrUpdateHouse(dbStreetId, house, context);
                        var dbStreetHouse = context.AreaStreetHouses.FirstOrDefault(h => h.idArea == dbAreaId &&
                                                                                         h.idStreet == dbStreetId &&
                                                                                         h.idHouse == dbHouseId);

                        if (dbStreetHouse != null)
                            continue;

                        context.AreaStreetHouses.Add(new AreaStreetHouse()
                        {
                            idArea = dbAreaId,
                            idStreet = dbStreetId,
                            idHouse = dbHouseId
                        });

                        context.SaveChanges();
                    }
                }
            }
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

        private static int AddOrUpdateStreet(string city, Street street, ElectorContext context)
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

        private static ICollection<ProblemData> GetAraeProblems()
        {
            var problems = new List<ProblemData>();

            using (var context = new ElectorContext())
            {
                foreach (var dbProblem in context.Problems.ToArray())
                {
                    var problem = new ProblemData()
                    {
                        id = dbProblem.id,
                        FIO = dbProblem.FIO,
                        Text = dbProblem.Text,
                        House = dbProblem.House,
                        Street = dbProblem.Street,
                        Flat = dbProblem.Flat
                    };

                    try
                    {
                        var areaNumber = context.AreaStreetHouses.FirstOrDefault(a => a.idStreet == dbProblem.Street.id && a.idHouse == dbProblem.idHouse).Area.AreaNumber;
                        problem.AreaNumber = areaNumber;
                    }
                    catch
                    {
                    }

                    problems.Add(problem);
                }

            }

            return problems;
        }
    }
}
