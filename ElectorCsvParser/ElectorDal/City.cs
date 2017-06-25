using System.ComponentModel.DataAnnotations.Schema;

namespace ElectorDal
{
    [Table("City")]
    public class City
    {
        public int id { get; set; }
        public string Name { get; set; }
    }
}
