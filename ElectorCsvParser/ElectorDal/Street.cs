﻿using System;
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
        public int id { get; set; }
        public string FullName { get; set; }

        [ForeignKey("idCity")]
        public virtual City City { get; set; }

        public int idCity { get; set; }

        public Street() { }

        public Street(string shortName, string marker)
        {
            FullName = string.Format("{0} {1}", shortName, marker);
        }

    }
}
