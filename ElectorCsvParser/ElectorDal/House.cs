using System.ComponentModel.DataAnnotations.Schema;

namespace ElectorDal
{
    [Table("House")]
    public class House
    {
        public int id { get; set; }
        public string Number { get; set; }
        public string SubNumber { get; set; }

        public int idCity { get; set; }

        [ForeignKey("idStreet")]
        public virtual Street Street { get; set; }
    }
}
