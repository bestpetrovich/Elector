using ElectorDal;

namespace ElectorCsvParser
{
    internal class ProblemData
    {
        public ProblemData()
        {
        }

        public int id { get; set; }
        public string FIO { get; set; }
        public string Text { get; set; }
        public House House { get; set; }
        public Street Street { get; set; }
        public Flat Flat { get; set; }
        public int AreaNumber { get; set; }
        public int? idHouse { get; set; }
    }
}