using System.ComponentModel.DataAnnotations.Schema;

namespace ElectorDal
{
    [Table("AreaStreetHouse")]
    public class AreaStreetHouse
    {
        public int id { get; set; }

        [ForeignKey("idArea")]
        public virtual Area Area { get; set; }
        public int? idArea { get; set; }

        [ForeignKey("idHouse")]
        public virtual House House { get; set; }
        public int? idHouse { get; set; }

        [ForeignKey("idStreet")]
        public virtual Street Street { get; set; }
        public int? idStreet { get; set; }
    }
}
