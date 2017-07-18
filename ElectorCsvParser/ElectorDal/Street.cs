using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectorDal
{
    [Table("Street")]
    public class Street
    {
        private string _revertName;

        public int id { get; set; }
        public string FullName { get; set; }

        [ForeignKey("idCity")]
        public virtual City City { get; set; }

        public int idCity { get; set; }

        public Street() { }

        public Street(string shortName, string marker)
        {
            var decodedMarker = Markers.DecodeMarker(marker);

            FullName = string.Format("{0} {1}", shortName.ToUpper(), decodedMarker.ToUpper());

            CreateReverName(shortName.ToUpper(), decodedMarker.ToUpper());
        }

        private void CreateReverName(string shortName, string marker)
        {
            var words = TextParser.GetWords(shortName);
            string name;
            if (words.Length < 2)
                name = shortName;
            else
                name = string.Join(" ", words.Reverse());

            _revertName = string.Format("{0} {1}", name.ToUpper(), marker.ToUpper());
        }

        [NotMapped]
        public string GetRevertName { get { return _revertName;  } }

        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            var x = obj as Street;
            if (obj == null || GetType() != obj.GetType() || x == null)
            {
                return false;
            }

            if (idCity == x.idCity && id == x.id && FullName == x.FullName)
                return true;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + id.GetHashCode();
            hash = (hash * 7) + idCity.GetHashCode();
            hash = (hash * 7) + FullName.GetHashCode();

            return hash;
        }


    }
}
