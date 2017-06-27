using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ElectorDal
{
    [Table("House")]
    public class House
    {
        public int id { get; set; }
        public string Number { get; set; }
        public string SubNumber { get; set; }

        public int idStreet { get; set; }        

        [ForeignKey("idStreet")]
        public virtual Street Street { get; set; }

        private List<Flat> _flats = new List<Flat>();
        public void AddFlat(Flat flat)
        {
            if (_flats.Any(f => f.Number == flat.Number))
                return;

            _flats.Add(flat);
        }

        public List<Flat> GetFlats()
        {
            return _flats;
        }
    }
}
