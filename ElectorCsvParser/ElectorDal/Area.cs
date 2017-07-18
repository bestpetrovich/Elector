using System.ComponentModel.DataAnnotations.Schema;

namespace ElectorDal
{
    [Table("Area")]
    public class Area
    {
        public int id { get; set; }
        public int AreaNumber { get; set; }
        public string CompainingName { get; set; }

        [ForeignKey("idCity")]
        public virtual City City { get; set; }

        public int idCity { get; set; }
    }
}
