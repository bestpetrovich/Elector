using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ElectorDal
{
    public class ElectorContext : DbContext
    {
        public virtual DbSet<City> Citys { get; set; }
        public virtual DbSet<Street> Streets { get; set; }
        public virtual DbSet<House> Houses { get; set; }
        public virtual DbSet<Flat> Flats { get; set; }
    }
}
