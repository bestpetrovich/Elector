using System.ComponentModel.DataAnnotations.Schema;

namespace ElectorDal
{
    [Table("Flat")]
    public class Flat
    {
        public int id { get; set; }
        public int Number { get; set; }

        public int? Entrance { get; set; }
        public int? Floor { get; set; }

        [ForeignKey("idHouse")]
        public virtual House House { get; set; }
        public int idHouse { get; set; }
    }
}
