using System.ComponentModel.DataAnnotations.Schema;

namespace ElectorDal
{
    [Table("Problem")]
    public class Problem
    {
        public int id { get; set; }
        public string Text { get; set; }
        public string FIO { get; set; }
            
        [ForeignKey("idStreet")]
        public virtual Street Street { get; set; }
        public int? idStreet { get; set; }

        [ForeignKey("idHouse")]
        public virtual House House { get; set; }
        public int? idHouse { get; set; }

        [ForeignKey("idFlat")]
        public virtual Flat Flat { get; set; }
        public int? idFlat { get; set; }
    }
}
